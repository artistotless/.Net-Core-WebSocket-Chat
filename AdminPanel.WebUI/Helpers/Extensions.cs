using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AdminPanel.WebUI.Helpers
{
    public static class ControllerExtension
    {
        public static string ShortName(this Controller ext, string controllerFullName)
        {
            return controllerFullName.Replace("Controller", string.Empty);
        }
    }

    public static class PolicyExtension
    {
        public static void AddPermissionPolicies(this IServiceCollection services)
        {
            string[] permissions = { "CanCreate", "CanRead", "CanUpdate", "CanDelete" };
            foreach (string p in permissions)
            {
                services.AddAuthorization(config => config.AddPolicy(p, x => x.RequireClaim(p, "True")));
            }
        }
    }
}
