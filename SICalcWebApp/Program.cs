using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SICalcWebApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using SICalcWebApp.Areas.RiceMill.Services;
using SICalcWebApp.Repository;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AddAreaFolderRouteModelConvention(
            areaName: "Identity",
            folderPath: "/Account",
            model =>
            {
                foreach (var selector in model.Selectors)
                {
                    var originalTemplate = selector.AttributeRouteModel.Template;
                    selector.AttributeRouteModel.Template = "{tenant}/" + originalTemplate;
                }
            });
    });


// Add session services
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Required for GDPR compliance
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set timeout duration
});



// Register HTTP Context Accessor (required to access tenant from URL)
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITenantProvider, TenantProvider>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SICS");
    //Console.WriteLine($"Using single database connection: {connectionString}");
    options.UseSqlServer(connectionString);
});




//// this code for multitenant if create many database on azure and use multitenant then this used 
//builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
//{
//    var tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
//    var tenant = tenantProvider.GetTenant();

//    Console.WriteLine($"Configuring DbContext for tenant: {tenant}");

//    var connectionString = tenant switch
//    {
//        "tenant1" => builder.Configuration.GetConnectionString("Tenant1ConnectionString"),
//        "shreeji" => builder.Configuration.GetConnectionString("Tenant2ConnectionString"),
//        _ => builder.Configuration.GetConnectionString("DefaultConnectionString"),
//    };

//    Console.WriteLine($"Using connection string: {connectionString}");
//    options.UseSqlServer(connectionString);
//});


// Register Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add other services

builder.Services.AddScoped<IMillQualityService, MillQualityService>();

builder.Services.AddScoped<IBatchProcessReportService, BatchProcessReportService>();
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
builder.Services.AddScoped<IMasterMillPlant, MasterMillPlant>();

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


// Add session middleware
app.UseSession();

app.UseMiddleware<TenantMiddleware>();
// Middleware for Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Routing setup
app.MapRazorPages();







//app.MapControllerRoute(
//    name: "tenantAndArea",
//    pattern: "{tenant}/{area:exists}/{controller=Home}/{action=Index}/{id?}");




//app.MapControllerRoute(
//    name: "tenant",
//    pattern: "{tenant}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "tenantAndArea",
    pattern: "{tenant}/{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "tenant",
    pattern: "{tenant}/{controller=Home}/{action=Index}/{id?}");



app.MapGet("/", context =>
{
    var tenant = "Default"; // Default tenant
    context.Response.Redirect($"/{tenant}/Identity/Account/Login");
    return Task.CompletedTask;
});



app.Run();

