using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Areas.RiceMill.VM;
using SICalcWebApp.Data;
using System.Diagnostics;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
  
    public class HmaliInputController : Controller
    {
        private readonly IHMaliInputService _hmaliInputService;

        public HmaliInputController(IHMaliInputService hmaliInputService)
        {
            _hmaliInputService = hmaliInputService;
        }


        // GET: HmaliInput/Create

        //[Authorize(Roles = SD.Role_Mill_Supervisor)]
        [Authorize(Roles = SD.Role_Mill_Admin + "," + SD.Role_Mill_Supervisor + "," + SD.Role_Super_Admin)]

        public async Task<IActionResult> Create()
        {
            var model = new HmaliInput
            {
                EntryDate = DateTime.Now // Set current date
            };

            var groups = await _hmaliInputService.GetGroupsAsync();
            ViewBag.GroupId = new SelectList(groups, "GroupId", "GroupName");
            //return View(new HmaliInput());
            return View(model);
        }



        // POST: HmaliInput/Create

        [HttpPost]
        [ValidateAntiForgeryToken]

        //[Authorize(Roles = SD.Role_Mill_Supervisor)]
        [Authorize(Roles = SD.Role_Mill_Admin + "," + SD.Role_Mill_Supervisor + "," + SD.Role_Super_Admin)]
        public async Task<IActionResult> Create(HmaliInput hmaliInput)
        {
            if (ModelState.IsValid)
            {
                await _hmaliInputService.CreateHmaliInputAsync(hmaliInput);
                return RedirectToAction(nameof(Create));
            }
           

            var groups = await _hmaliInputService.GetGroupsAsync();
            ViewBag.GroupId = new SelectList(groups, "GroupId", "GroupName", hmaliInput.GroupId);
            return View(hmaliInput);
        }

        // GET: HmaliInput/GetItemsByGroup
        [HttpGet]
        public async Task<JsonResult> GetItemsByGroup(int groupId)
        {


            var items = await _hmaliInputService.GetItemsByGroupAsync(groupId);
            // Assuming items is of type IEnumerable<MillItem>
            return Json(items.Select(item => new
            {
                itemName = item.ItemName,
                itemNumber = item.ItemNumber // This should be an int
            }));



            //var items = await _hmaliInputService.GetItemsByGroupAsync(groupId);
            //var itemList = items.Select(i => new
            //{
            //    i.ItemName,
            //    i.ItemNumber
            //});

            //return Json(itemList);
        }

        // GET: HmaliInput/GetItemDetails
        [HttpGet]
        public async Task<JsonResult> GetItemDetails(int itemNumber)
        {
            try
            {
                var item = await _hmaliInputService.GetItemDetailsAsync(itemNumber);

                return Json(new
                {
                    item.ItemName,
                    item.Rate,
                    item.Capacity
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Debug.WriteLine($"Error retrieving item: {ex.Message}");
                return Json(null);
            }

      
        }

        // GET: HmaliInput/GetItemDetails
        [HttpGet]
        public async Task<JsonResult> GetItemDetailss(int itemNumber)
        {
            try
            {
                var item = await _hmaliInputService.GetItemDetailssAsync(itemNumber);

                return Json(new
                {
                    item.ItemName,
                    item.Rate,
                    item.Capacity
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Debug.WriteLine($"Error retrieving item: {ex.Message}");
                return Json(null);
            }


        }


        [Authorize(Roles = SD.Role_Mill_Admin + "," + SD.Role_Super_Admin+","+SD.Role_Checker_Admin)]

       
        public async Task<IActionResult> Search(DateTime? fromDate, DateTime? toDate, int? groupId, int? ItemNumber)
        {
            // Get all groups and convert to List<SelectListItem>
            var groups = await _hmaliInputService.GetGroupsAsync();
            var groupSelectListItems = groups.Select(g => new SelectListItem
            {
                Value = g.GroupId.ToString(),
                Text = g.GroupName
            }).ToList();

            var viewModel = new HmaliSearchViewModel
            {
                Groups = groupSelectListItems,
                Items = new List<SelectListItem>(), // Initially empty
                SearchResults = new List<HmaliInput>(),
                GroupedSummary = new Dictionary<string, Dictionary<string, decimal>>() // Change to nested Dictionary for hierarchy
            };

            // Populate Items dropdown if GroupId is selected
            if (groupId.HasValue)
            {
                var items = await _hmaliInputService.GetItemsByGroupAsync(groupId.Value);
                viewModel.Items = items.Select(i => new SelectListItem
                {
                    Value = i.ItemNumber.ToString(),
                    Text = i.ItemName
                }).ToList();
            }

            // If dates are provided, search for records
            if (fromDate.HasValue && toDate.HasValue)
            {
                IEnumerable<HmaliInput> results;

                if (groupId.HasValue && ItemNumber.HasValue)
                {
                    results = await _hmaliInputService.SearchHmaliInputsAsync(fromDate, toDate, groupId, ItemNumber);
                }
                else if (groupId.HasValue)
                {
                    results = await _hmaliInputService.SearchHmaliInputsAsync(fromDate, toDate, groupId, null);
                }
                else
                {
                    results = await _hmaliInputService.SearchHmaliInputsAsync(fromDate, toDate, null, null);
                }

                viewModel.SearchResults = results.ToList();

                //// Group by GroupName, then by ItemName to create a nested grouping
                //viewModel.GroupedSummary = viewModel.SearchResults
                //    .GroupBy(r => r.GroupName)
                //    .ToDictionary(
                //        g => g.Key,
                //        g => g.GroupBy(r => r.ItemName)
                //            .ToDictionary(
                //                i => i.Key,
                //                i => i.Sum(r => r.Rate * r.Quantity)
                //            )
                //    );

                // Calculate grand total for the search results
                viewModel.GrandTotal = viewModel.SearchResults.Sum(r => r.Rate * r.Quantity);
            }

            return View(viewModel);
        }





        [Authorize(Roles = SD.Role_Mill_Admin + "," + SD.Role_Super_Admin + "," + SD.Role_Checker_Admin)]
        public async Task<IActionResult> PivotSummary(DateTime? fromDate, DateTime? toDate)
        {
            var viewModel = new HmaliSearchViewModel
            {
                FromDate = fromDate ?? DateTime.Now.AddDays(-30), // Default to last 30 days if null
                ToDate = toDate ?? DateTime.Now,
                GroupedSummary = new Dictionary<string, Dictionary<string, decimal>>(),
                GroupedQuantities = new Dictionary<string, Dictionary<string, decimal>>(), // Nested Dictionary for quantities
                GrandTotalQuantity = 0,
            };

            if (fromDate.HasValue && toDate.HasValue)
            {
                var results = await _hmaliInputService.SearchHmaliInputsAsync(fromDate, toDate, null, null);

                // Group data by GroupName and ItemName, calculate total cost per item
                viewModel.GroupedSummary = results
                    .GroupBy(r => r.GroupName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.GroupBy(r => r.ItemName)
                            .ToDictionary(
                                i => i.Key,
                                i => i.Sum(r => r.Rate * r.Quantity) // Calculate sum of total cost per item
                            )
                    );

                // Group by GroupName, calculate total quantity for each item under each group
                viewModel.GroupedQuantities = results
         .GroupBy(r => r.GroupName)
         .ToDictionary(
             g => g.Key,
             g => g.GroupBy(r => r.ItemName)
                 .ToDictionary(
                     i => i.Key,
                     i => i.Sum(r => (decimal)r.Quantity) // Cast to decimal to avoid type mismatch
                 )
         );

                // Calculate Grand Total for all items in the date range
                viewModel.GrandTotal = results.Sum(r => r.Rate * r.Quantity);

                viewModel.GrandTotalQuantity = results.Sum(r => r.Quantity);
            }

            return View("PivotSummary", viewModel);
        }



        [Authorize(Roles = SD.Role_Mill_Admin + "," + SD.Role_Mill_Supervisor + "," + SD.Role_Super_Admin)]
        public async Task<IActionResult> List()
        {
            // Fetch the data from the service
            var hmaliInputs = await _hmaliInputService.GetAllHmaliInputsAsync();

            // Return the view with the view model data
            return View(hmaliInputs);
        }

        [Authorize(Roles = SD.Role_Mill_Admin + "," + SD.Role_Mill_Supervisor + "," + SD.Role_Super_Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var hmaliInput = await _hmaliInputService.GetHmaliInputByIdAsync(id);
            if (hmaliInput == null)
            {
                return NotFound();
            }

            var groups = await _hmaliInputService.GetGroupsAsync();
            var items = await _hmaliInputService.GetItemsByGroupAsync(hmaliInput.GroupId) ?? new List<MillItem>();

            // Populate ViewBags for dropdowns
            ViewBag.GroupId = new SelectList(groups, "GroupId", "GroupName", hmaliInput.GroupId);
            ViewBag.ItemId = new SelectList(items, "ItemId", "ItemName", hmaliInput.HmaliId);

            return View(hmaliInput);
        }

        // POST: HmaliInput/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Mill_Admin + "," + SD.Role_Mill_Supervisor + "," + SD.Role_Super_Admin)]
        public async Task<IActionResult> Edit(int id, HmaliInput hmaliInput)
        {
            if (id != hmaliInput.HmaliId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _hmaliInputService.UpdateHmaliInputAsync(hmaliInput);
                return RedirectToAction(nameof(List));
            }

            // If ModelState is invalid, repopulate the dropdowns
            var groups = await _hmaliInputService.GetGroupsAsync();
            var items = await _hmaliInputService.GetItemsByGroupAsync(hmaliInput.GroupId) ?? new List<MillItem>();

            ViewBag.GroupId = new SelectList(groups, "GroupId", "GroupName", hmaliInput.GroupId);
            ViewBag.ItemId = new SelectList(items, "ItemId", "ItemName", hmaliInput.HmaliId);

            return View(hmaliInput);
        }

        // Action to get items by group (for AJAX call)
        [HttpGet]
        public async Task<IActionResult> GetItemsByGroupp(int groupId)
        {
            var items = await _hmaliInputService.GetItemsByGroupAsync(groupId);
            if (items == null || !items.Any())
            {
                return Json(new { success = false, message = "No items found" });
            }

            return Json(items.Select(x => new { x.ItemId, x.ItemName }));
        }



        // POST: HmaliInput/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Mill_Admin + "," + SD.Role_Mill_Supervisor + "," + SD.Role_Super_Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _hmaliInputService.DeleteHmaliInputAsync(id);
            return RedirectToAction(nameof(List));
        }


    }
}
