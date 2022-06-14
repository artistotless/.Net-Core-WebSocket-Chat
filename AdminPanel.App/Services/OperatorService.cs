using AdminPanel.App.Helpers;
using AdminPanel.App.Models;
using AdminPanel.App.Models.Abstract;
using AdminPanel.Domain.Abstract;
using AdminPanel.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdminPanel.App.Services
{
    public class OperatorService
    {
        private readonly IOperatorsRepository _operatorsRepository;
        private readonly IRolesRepository _rolesRepository;

        public OperatorService(IOperatorsRepository operatorsRepository, IRolesRepository rolesRepository)
        {
            _operatorsRepository = operatorsRepository;
            _rolesRepository = rolesRepository;
        }

        public async Task<Response<ClaimsIdentity>> LoginAsync(string email, string password)
        {
            var response = new Response<ClaimsIdentity>();
            try
            {
                var @operator = await _operatorsRepository.GetByEmailAsync(email);
                if (@operator == null)
                {
                    Error.SetError(response, Error.USER_NOT_FOUND);
                    return response;
                }

                if (@operator.Password != await SHA256Cryptor.CreateHashAsync(password))
                {
                    Error.SetError(response, Error.PASSWORD_INVALID);
                    return response;
                }

                var claimModel = new OperatorClaimModel(@operator.Name, @operator.Role, @operator.Email);
                var identity = IdentityService.Create(claimModel);
                response.Result = identity;

            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<ClaimsIdentity>> RegisterAsync(string email, string name, string password)
        {
            var response = new Response<ClaimsIdentity>();
            try
            {
                var @operator = await _operatorsRepository.GetByEmailAsync(email);
                if (@operator != null)
                {
                    Error.SetError(response, Error.USER_EXIST);
                    return response;
                }

                var role = await _rolesRepository.GetByIDAsync(1);

                await _operatorsRepository.AddAsync(new Operator()
                {
                    Email = email,
                    Name = name,
                    Password = await SHA256Cryptor.CreateHashAsync(password),
                    Role = role
                });


                var claimModel = new OperatorClaimModel(name, role, email);
                var identity = IdentityService.Create(claimModel);
                response.Result = identity;
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<Operator>> GetByEmailAsync(string email)
        {
            var response = new Response<Operator>();
            try
            {
                response.Result = await _operatorsRepository.GetByEmailAsync(email);
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<Operator>> GetByContextAsync(HttpContext context)
        {
            var response = new Response<Operator>();
            string email;
            try
            {
                email = context.User.FindFirst("Email").Value;
            }
            catch
            {
                Error.SetError(response, Error.USER_NOT_FOUND);
                return response;
            }
            return await GetByEmailAsync(email);
        }
    }
}

