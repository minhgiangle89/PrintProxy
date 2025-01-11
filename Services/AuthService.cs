using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PrintProxy.Models;
using PrintProxy.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrintProxy.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PrintProxyClient> _logger;
        private readonly string _serverUrl;
        private readonly string _authUser;
        private readonly string _authPass;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _m2mServerUrl;
        public AuthService(IConfiguration configuration, ILogger<PrintProxyClient> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _configuration = configuration;
            //_serverUrl = configuration["AuthServerUrl"];
            //_authUser = configuration["AuthName"];
            //_authPass = configuration["AuthPass"];

            _m2mServerUrl = configuration["AuthM2MServerUrl"];
            _clientId = configuration["ClientID"];
            _clientSecret = configuration["ClientSecret"];
        }

        public async Task<string> LoginAsync()
        {
            try
            {
                //var loginRequest = new
                //{
                //    userName = _authUser,
                //    password = _authPass
                //};

                var loginM2MRequest = new
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                };


                var jsonContent = JsonConvert.SerializeObject(loginM2MRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_m2mServerUrl, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                    if (loginResponse.isSuccess && !string.IsNullOrEmpty(loginResponse.result.token))
                    {
                        _logger.LogInformation("Login successful");
                        return loginResponse.result.token;
                    }
                    else
                    {
                        _logger.LogError($"Login failed: {loginResponse.message}");
                        throw new Exception($"Login failed: {loginResponse.message}");
                    }
                }
                else
                {
                    _logger.LogError($"HTTP request failed with status: {response.StatusCode}");
                    throw new Exception($"Login request failed with status: {response.StatusCode}");
                }

            }
            catch (Exception)
            {
                throw new Exception("Login failed");
            }     
        }
    }
}
