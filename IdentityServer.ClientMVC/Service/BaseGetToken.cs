using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

namespace IdentityServer.ClientMVC.Service
{
    public class BaseGetToken
    {
        private readonly IHttpClientFactory _factory;

        public BaseGetToken(IHttpClientFactory factory)
        {
            _factory = factory;
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
    }
}
