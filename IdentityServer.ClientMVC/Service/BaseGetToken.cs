using IdentityModel.Client;
using IdentityServer.ClientMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using static IdentityModel.OidcConstants;

namespace IdentityServer.ClientMVC.Service
{
    public class BaseGetToken
    {
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _httpContext;

        public BaseGetToken(IHttpClientFactory factory, IHttpContextAccessor httpContext)
        {
            _factory = factory;
            _httpContext = httpContext;
        }
        public async Task GetDataByToken()
        {
            // 1 - to get token from identity server, we need provide IS configuration
            // 2 - send request
            // 3 - deserialize object

            //way 1

#region Way 1
            // 1 retrieve api credentials 
            var apiCredential = new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:5005/connect/token",
                ClientId = "movieClient",
                ClientSecret = "secret",
                Scope = "movieAPI"
            };

            // workflow example, not optimize and clean
            var client = new HttpClient();

            // no need but may check the identity server is working fine by reach the discovery document
            var discovery = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
            if (discovery.IsError)
            {
                // throw http 500
                throw new BadHttpRequestException("identity server is not working", 500);
            }

            // authenticate and get an access token form server
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiCredential);
            if (tokenResponse.IsError)
            {
                throw new BadHttpRequestException("can not get access token", 500);
            }

            // 2 send request
            var apiClient = new HttpClient();

            // set token 
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("https://localhost:7267/api/movies");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            //  List<T> data = JsonConvert.DeserializeObject<T>(content);
            #endregion

            // way 2
            #region Way 2
            var httpClient = _factory.CreateClient("MovieAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/movies/");
            var response2 = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response2.EnsureSuccessStatusCode();

         //   var content2 = await response2.Content.ReadAsStringAsync();
            //  List<T> data = JsonConvert.DeserializeObject<T>(content);

            #endregion
        }

        public async Task<UserInfoViewModel> GetUserInfo()
        {
            var idpClient = _factory.CreateClient("IDPClient");
            var metaDataRespone = await idpClient.GetDiscoveryDocumentAsync();
            if (metaDataRespone.IsError)
            {
                // throw http 500
                throw new HttpRequestException("identity server is not working");
            }
            var accessToken = await _httpContext.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var userInfoResponse = await idpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataRespone.UserInfoEndpoint,
                Token = accessToken
            });
            if (userInfoResponse.IsError)
            {
                // throw http 500
                throw new HttpRequestException("something went wrong when get user info");
            }
            
            var userInfoDictionary = new Dictionary<string, string>();
            foreach(var claim in userInfoResponse.Claims)
            {
                userInfoDictionary.Add(claim.Type, claim.Value);
            }
            return new UserInfoViewModel(userInfoDictionary);
        }
    }
}
