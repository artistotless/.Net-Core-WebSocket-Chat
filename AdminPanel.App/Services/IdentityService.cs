using AdminPanel.App.Models.Abstract;
using System.Security.Claims;

namespace AdminPanel.App.Services
{
    public class IdentityService
    {
        public static ClaimsIdentity Create(ClaimModel model)
        {
            return new ClaimsIdentity(model.GetClaims(), "Cookies");
        }
    }
}
