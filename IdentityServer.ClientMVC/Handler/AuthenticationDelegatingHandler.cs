using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http;

namespace IdentityServer.ClientMVC.Handler
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ClientCredentialsTokenRequest _clientCredentials;
        private readonly IHttpContextAccessor _contextAccessor;
        #region grant type code
        //public AuthenticationDelegatingHandler(IHttpClientFactory httpClientFactory, ClientCredentialsTokenRequest clientCredentials)
        //{
        //    _httpClientFactory = httpClientFactory;
        //    _clientCredentials = clientCredentials;
        //}
        //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    var httpClient = _httpClientFactory.CreateClient("IDPClient");
        //    var tokenRespone = await httpClient.RequestClientCredentialsTokenAsync(_clientCredentials);
        //    if (tokenRespone.IsError)
        //    {
        //        throw new BadHttpRequestException("something went wrong with token");
        //    }
        //    request.SetBearerToken(tokenRespone.AccessToken);
        //    return await base.SendAsync(request, cancellationToken);
        //}
        #endregion

        #region grant type hybrid
        public AuthenticationDelegatingHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if(!string.IsNullOrWhiteSpace(accessToken))
            {
                request.SetBearerToken(accessToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
        #endregion
    }
}
