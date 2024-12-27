
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Data;
using SICalcWebApp.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("SICS");

// Register services, for example, Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.AddScoped<ISortexService, SortexService>();

builder.Services.AddScoped<IMillingProcessService, MillingProcessService>();
builder.Services.AddScoped<IMachineProcessService, MachineProcessService>();
builder.Services.AddScoped<IDryerService, DryerService>();
builder.Services.AddScoped<ITPDInfoService, TPDInfoService>();
builder.Services.AddScoped<IFCService, FCService>();
builder.Services.AddScoped<IFCInfoService, FCInfoService>();
builder.Services.AddScoped<IIronTypeService, IronTypeService>();
builder.Services.AddScoped<IInputOperandsService, InputOperandsService>();
builder.Services.AddScoped<IPriceMaterial, PriceMaterialSer>();
builder.Services.AddScoped<IGroupMillService, GroupMillService>();
builder.Services.AddScoped<IMillItemService, MillItemService>();
builder.Services.AddScoped<IHMaliInputService, HMaliInputService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IMasterMillPlant,MasterMillPlant>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure you have authentication middleware
app.UseAuthorization();

app.MapRazorPages();

// Redirect the root URL to the login page


app.MapGet("/", context =>
{
    if (context.User.Identity?.IsAuthenticated ?? false)
    {
        context.Response.Redirect("/Home/Index"); // Redirect to a home page if authenticated
    }
    else
    {
        context.Response.Redirect("/Identity/Account/Login");
    }
    return Task.CompletedTask; // Ensure method returns a Task
});
//app.MapGet("/", async context =>
//{
//    context.Response.Redirect("/Identity/Account/Login");
//});

// Area routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default route (optional, can be adjusted if needed)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();












//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.EntityFrameworkCore;
//using SICalcWebApp.Areas.RiceMill.Services;
//using SICalcWebApp.Data;
//using SICalcWebApp.Repository;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var connectionString = builder.Configuration.GetConnectionString("SICS");

//// Register services, for example, Entity Framework Core
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

//builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

////// Register services, for example, Entity Framework Core
////builder.Services.AddDbContext<ApplicationDbContext>(options =>
////    options.UseSqlServer(connectionString, sqlServerOptions =>
////    {
////        sqlServerOptions.CommandTimeout(180); // Timeout of 180 seconds
////        sqlServerOptions.EnableRetryOnFailure(
////            maxRetryCount: 5,                    // Retry up to 5 times
////            maxRetryDelay: TimeSpan.FromSeconds(30), // Wait 30 seconds between retries
////            errorNumbersToAdd: null);            // Retry on all transient SQL errors
////    })
////    // Optional: Add detailed logging for debugging SQL operations
////    .EnableSensitiveDataLogging()                // Show parameter values in logs (use cautiously)
////    .LogTo(Console.WriteLine,                    // Output logs to console
////           Microsoft.Extensions.Logging.LogLevel.Information) // Set log level to Information or higher
////);


//builder.Services.AddRazorPages();
//builder.Services.AddScoped<ITPDInfoService, TPDInfoService>();
//builder.Services.AddScoped<IFCService,FCService>();
//builder.Services.AddScoped<IFCInfoService, FCInfoService>();
//builder.Services.AddScoped<IIronTypeService,IronTypeService>();

//builder.Services.AddScoped<IInputOperandsService, InputOperandsService>();

//builder.Services.AddScoped<IPriceMaterial,PriceMaterialSer>();

//builder.Services.AddScoped<IGroupMillService, GroupMillService>();

//builder.Services.AddScoped< IMillItemService, MillItemService>();

//builder.Services.AddScoped<IHMaliInputService, HMaliInputService>();


//builder.Services.AddScoped<IEmailSender,EmailSender>();




//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();
//app.UseAuthorization();

//app.MapRazorPages();

//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");





//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
