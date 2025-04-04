namespace SICalcWebApp.Data
{
    // Middleware to extract tenant from the URL
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
        {
            var tenant = GetTenantFromUrl(context.Request.Path.Value);
            //Console.WriteLine($"Middleware extracted tenant: {tenant}");
            var tenantFromSession = context.Session.GetString("Tenant");

            //Console.WriteLine($"Tenant from URL: {tenant}, Tenant from session: {tenantFromSession}");

            if (!string.IsNullOrEmpty(tenantFromSession) && tenantFromSession != tenant)
            {
                // If session tenant and URL tenant mismatch, redirect to login
                context.Response.Redirect($"/{tenantFromSession}/Identity/Account/Login");
                return;
            }
            tenantProvider.SetTenant(tenant);
            await _next(context);
        }

        private string GetTenantFromUrl(string path)
        {
            var segments = path?.Split('/');
            var tenant = segments != null && segments.Length > 1 ? segments[1] : "Default";
            //Console.WriteLine($"Tenant extracted: {tenant} from path: {path}");
            return tenant;
        }
    }

    // Interface to manage tenant information
    public interface ITenantProvider
    {
        string GetTenant();
        void SetTenant(string tenant);
    }

    // TenantProvider implementation using IHttpContextAccessor
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenant()
        {
            return _httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString() ?? "Default";
        }

        public void SetTenant(string tenant)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Items["Tenant"] = tenant;
            }
        }
    }
}
