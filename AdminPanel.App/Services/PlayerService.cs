using AdminPanel.App.Helpers;
using AdminPanel.App.Models;
using AdminPanel.App.Models.Abstract;
using AdminPanel.Domain.Abstract;
using AdminPanel.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdminPanel.App.Services
{

    public class PlayerService
    {
        private readonly IPlayersRepository _playersRepository;

        public PlayerService(IPlayersRepository playersRepository)
        {
            _playersRepository = playersRepository;
        }

        public async Task<Response<string>> RegisterAsync(string name, string hardWareInformation)
        {
            var response = new Response<string>();
            try
            {
                var tokenResponse = await GenerateToken(hardWareInformation);
                var token = tokenResponse.Result;
                var player = await _playersRepository.GetByTokenAsync(token);

                if (player == null)
                {
                    await _playersRepository.AddAsync(new Player()
                    {
                        Name = name,
                        Token = token,
                    });
                }
                response.Result = token;
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<bool>> ExistAsync(string token)
        {
            var response = new Response<bool>();
            try
            {
                var player = await _playersRepository.GetByTokenAsync(token);
                if (player != null)
                {
                    response.Result = true;
                }
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<Player>> GetByTokenAsync(string token)
        {
            var response = new Response<Player>();
            try
            {
                response.Result = await _playersRepository.GetByTokenAsync(token);
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<Player>> GetByIDAsync(int id)
        {
            var response = new Response<Player>();
            try
            {
                response.Result = await _playersRepository.Players
                 .FirstOrDefaultAsync(x => x.PlayerID == id);
            }
            catch
            {
                Error.SetError(response, Error.DB_FAIL);
            }
            return response;
        }

        public async Task<Response<Player>> GetByContextAsync(HttpContext context)
        {
            var response = new Response<Player>();
            string token = string.Empty;
            try
            {
                token = context.Request.QueryString.Value.Replace("?token=", string.Empty);
                token = System.Web.HttpUtility.UrlDecode(token, System.Text.Encoding.UTF8).Replace("\"", "");
            }
            catch
            {
                Error.SetError(response, Error.USER_NOT_FOUND);
            }
            return await GetByTokenAsync(token);
        }

        private async Task<Response<string>> GenerateToken(string data)
        {
            var response = new Response<string>();
            try
            {
                response.Result = await SHA256Cryptor.CreateHashAsync(data + "token");
            }
            catch
            {
                Error.SetError(response, Error.UNKNOWN);
            }

            return response;
        }

    }
}
