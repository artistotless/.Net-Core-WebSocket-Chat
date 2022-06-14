
using AdminPanel.App.Models.Abstract;
using AdminPanel.Domain.Entities;
using System.Security.Claims;

namespace AdminPanel.App.Models
{
    public class OperatorClaimModel : ClaimModel
    {
        private readonly Permission _permission;
        private readonly string _email;
        public OperatorClaimModel(string name, Role role, string email) : base(name, role.Title)
        {
            _permission = role.Permission;
            _email = email;
        }
        protected override Claim[] CreateClaims()
        {
            return new[]{
                new Claim(ClaimsIdentity.DefaultNameClaimType, Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, Role),
                new Claim("Email", _email),
                new Claim("CanCreate",_permission.CanCreate.ToString(),"bool"),
                new Claim("CanRead",_permission.CanRead.ToString(),"bool"),
                new Claim("CanUpdate",_permission.CanUpdate.ToString(),"bool"),
                new Claim("CanDelete",_permission.CanUpdate.ToString(),"bool"),
            };
        }
    }
}
