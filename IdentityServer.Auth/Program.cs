using IdentityServer.Auth;
using IdentityServer4.Models;
using IdentityServer4.Test;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// no need to add full config, require apiscope and client
builder.Services.AddIdentityServer()
    .AddInMemoryClients(Config.Clients) // which client can be use the resource
    .AddInMemoryIdentityResources(Config.IdentityResources) // user information, eg id,email,...
 //   .AddInMemoryApiResources(Config.ApiResources) // which part was protected
    .AddInMemoryApiScopes(Config.ApiScopes)     // what client was allowed to do
    .AddTestUsers(Config.TestUsers)
    .AddDeveloperSigningCredential();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();
// app.MapGet("/", () => "Hello World!");

app.MapDefaultControllerRoute();

app.Run();
