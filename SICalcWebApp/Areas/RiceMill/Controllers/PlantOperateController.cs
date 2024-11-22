using Microsoft.AspNetCore.Mvc;
using SICalcWebApp.Areas.RiceMill.Models;
using SICalcWebApp.Areas.RiceMill.Services;

namespace SICalcWebApp.Areas.RiceMill.Controllers
{
    [Area("RiceMill")]
    public class PlantOperateController : Controller
    {
        private readonly IMasterMillPlant _service;

        public PlantOperateController(IMasterMillPlant service)
        {
            _service = service;
        }

        //--------This Section For Staff
        public async Task<IActionResult> StaffList()
        {
            var staff = await _service.GetAllStaffAsync();
            return View(staff);
        }

        public IActionResult AddStaff()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStaff(Staff staff)
        {
            if (ModelState.IsValid)
            {
                await _service.AddStaffAsync(staff);
                return RedirectToAction(nameof(StaffList));
            }
            return View(staff);
        }

        public async Task<IActionResult> EditStaff(int id)
        {
            var staff = await _service.GetStaffByIdAsync(id);
            if (staff == null) return NotFound();
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(Staff staff)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateStaffAsync(staff);
                return RedirectToAction(nameof(StaffList));
            }
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            try
            {
                // Call your service to delete the staff record
                await _service.DeleteStaffAsync(id);

                // Return success response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Json(new { success = false, message = ex.Message });
            }
        }

    //--------This Section For Paddy Type Methods
        public async Task<IActionResult> PaddyTypeList()
        {
            var paddyTypes = await _service.GetAllPaddyTypesAsync();
            return View(paddyTypes);
        }

        public IActionResult AddPaddyType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPaddyType(PaddyType paddyType)
        {
            if (ModelState.IsValid)
            {
                await _service.AddPaddyTypeAsync(paddyType);
                return RedirectToAction(nameof(PaddyTypeList));
            }
            return View(paddyType);
        }

        public async Task<IActionResult> EditPaddyType(int id)
        {
            var paddyType = await _service.GetPaddyTypeByIdAsync(id);
            if (paddyType == null) return NotFound();
            return View(paddyType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPaddyType(PaddyType paddyType)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdatePaddyTypeAsync(paddyType);
                return RedirectToAction(nameof(PaddyTypeList));
            }
            return View(paddyType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePaddyType(int id)
        {
            //await _service.DeletePaddyTypeAsync(id);
            //return RedirectToAction(nameof(PaddyTypeList));


            try
            {
                // Call your service to delete the staff record
                await _service.DeletePaddyTypeAsync(id);

                // Return success response
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return Json(new { success = false, message = ex.Message });
            }
        }



        //--------This Section For Feeding Bunker




        public async Task<IActionResult> ListFeedB()
        {
            var bunkers = await _service.GetAllAsync<FeedingBunker>();
            return View(bunkers);
        }





        public IActionResult AddFeedBunker()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedBunker(FeedingBunker bunker)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(bunker);
                return RedirectToAction(nameof(AddFeedBunker));
            }
            return View(bunker);
        }








        public async Task<IActionResult> EditFeedBunker(int id)
        {
            var bunker = await _service.GetByIdAsync<FeedingBunker>(id);
            if (bunker == null)
            {
                return NotFound();
            }
            return View(bunker);
        }

        [HttpPost]
        public async Task<IActionResult> EditFeedBunker(FeedingBunker bunker)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(bunker);
                return RedirectToAction(nameof(ListFeedB));
            }
            return View(bunker);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var bunker = await _service.GetByIdAsync<FeedingBunker>(id);
            if (bunker == null)
            {
                return NotFound();
            }
            return View(bunker);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedingBunker = await _service.GetByIdAsync<FeedingBunker>(id);
            if (feedingBunker == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync<FeedingBunker>(id);
            return RedirectToAction(nameof(ListFeedB));
        }


        //--------This Section For Arwa Usna 



        public async Task<IActionResult> ListPMethod()
        {
            var processes = await _service.GetAllAsync<ProcessMethod>();
            return View(processes);
        }





        public IActionResult AddPMethod()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPMethod(ProcessMethod process)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(process);
                return RedirectToAction(nameof(ListPMethod));
            }
            return View(process);
        }








        public async Task<IActionResult> EditPMethod(int id)
        {
            var process = await _service.GetByIdAsync<ProcessMethod>(id);
            if (process == null)
            {
                return NotFound();
            }
            return View(process);
        }

        [HttpPost]
        public async Task<IActionResult> EditPMethod(ProcessMethod process)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(process);
                return RedirectToAction(nameof(ListPMethod));
            }
            return View(process);
        }

    

        [HttpPost]
        public async Task<IActionResult> DeletePMethod(int id)
        {
            var process = await _service.GetByIdAsync<ProcessMethod>(id);
            if (process == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync<ProcessMethod>(id);
            return RedirectToAction(nameof(ListPMethod));
        }







        //--------This Section For Handi



        public async Task<IActionResult> ListHandi()
        {
            var processes = await _service.GetAllAsync<TypeOfHandi>();
            return View(processes);
        }





        public IActionResult AddHandi()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddHandi(TypeOfHandi process)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(process);
                return RedirectToAction(nameof(ListHandi));
            }
            return View(process);
        }








        public async Task<IActionResult> EditHandi(int id)
        {
            var process = await _service.GetByIdAsync<TypeOfHandi>(id);
            if (process == null)
            {
                return NotFound();
            }
            return View(process);
        }

        [HttpPost]
        public async Task<IActionResult> EditHandi(TypeOfHandi process)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(process);
                return RedirectToAction(nameof(ListHandi));
            }
            return View(process);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteHandi(int id)
        {
            var process = await _service.GetByIdAsync<TypeOfHandi>(id);
            if (process == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync<TypeOfHandi>(id);
            return RedirectToAction(nameof(ListHandi));
        }




















        //--------This Section For Mill Bunker



        public async Task<IActionResult> ListMillBunekr()
        {
            var processes = await _service.GetAllAsync<MillBunker>();
            return View(processes);
        }





        public IActionResult AddMillBunekr()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMillBunekr(MillBunker process)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(process);
                return RedirectToAction(nameof(ListMillBunekr));
            }
            return View(process);
        }


        public async Task<IActionResult> EditMillBunekr(int id)
        {
            var process = await _service.GetByIdAsync<MillBunker>(id);
            if (process == null)
            {
                return NotFound();
            }
            return View(process);
        }

        [HttpPost]
        public async Task<IActionResult> EditMillBunekr(MillBunker process)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(process);
                return RedirectToAction(nameof(ListMillBunekr));
            }
            return View(process);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteMillBunekr(int id)
        {
            var process = await _service.GetByIdAsync<MillBunker>(id);
            if (process == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync<MillBunker>(id);
            return RedirectToAction(nameof(ListMillBunekr));
        }









        //--------This Section For Sortex Bunker



        public async Task<IActionResult> ListSortexBunker()
        {
            var processes = await _service.GetAllAsync<SortexBunker>();
            return View(processes);
        }





        public IActionResult AddSortexBunker()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSortexBunker(SortexBunker process)
        {
            if (ModelState.IsValid)
            {
                await _service.AddAsync(process);
                return RedirectToAction(nameof(ListSortexBunker));
            }
            return View(process);
        }


        public async Task<IActionResult> EditSortexBunker(int id)
        {
            var process = await _service.GetByIdAsync<SortexBunker>(id);
            if (process == null)
            {
                return NotFound();
            }
            return View(process);
        }

        [HttpPost]
        public async Task<IActionResult> EditSortexBunker(SortexBunker process)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(process);
                return RedirectToAction(nameof(ListSortexBunker));
            }
            return View(process);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteSortexBunker(int id)
        {
            var process = await _service.GetByIdAsync<SortexBunker>(id);
            if (process == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync<SortexBunker>(id);
            return RedirectToAction(nameof(ListSortexBunker));
        }








    }
}
