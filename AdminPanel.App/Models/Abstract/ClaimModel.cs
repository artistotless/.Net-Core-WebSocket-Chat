using System.Security.Claims;

namespace AdminPanel.App.Models.Abstract
{
    public abstract class ClaimModel
    {
        protected string Name { get; set; }
        protected string Role { get; set; }
        protected Claim[] Claims { get; set; }

        public ClaimModel(string identificator, string role)
        {
            Name = identificator;
            Role = role;
        }

        protected abstract Claim[] CreateClaims();
        public Claim[] GetClaims()
        {
            return CreateClaims();
        }
    }
}
