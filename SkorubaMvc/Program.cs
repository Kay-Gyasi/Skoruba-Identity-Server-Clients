using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SkorubaMvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// TODO:: Use discovery document endpoints to implement logout, reset password functionality

// In order to communicate with identity server we need to use OpendIdConnect
// Loading the configuration from the appsettings.json file and passing it to the TokenService class
builder.Services.Configure<IdentityServerSettings>(builder.Configuration.GetSection("IdentityServerSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "cookie";
    options.DefaultChallengeScheme = "oidc";
}).AddCookie("cookie")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["TestInteractiveServiceSettings:AuthorityUrl"];
        options.ClientId = builder.Configuration["TestInteractiveServiceSettings:ClientId"];
        options.ClientSecret = builder.Configuration["TestInteractiveServiceSettings:ClientSecret"];
        options.Scope.Add(builder.Configuration["TestInteractiveServiceSettings:Scopes:0"]);
        options.Scope.Add(builder.Configuration["TestInteractiveServiceSettings:Scopes:1"]);
        options.Scope.Add(builder.Configuration["TestInteractiveServiceSettings:Scopes:2"]);

        options.ResponseType = "code"; // Matches grant type set in identity server for this client
        options.UsePkce = true; // Proof key for code exchange. It is a mechanism for protecting the communication in the
        // background while exchanging our information to get the token
        options.ResponseMode = "query"; // Sets the response to return in a query string
        options.SaveTokens = true; // Saves authentication response in cookie
        options.GetClaimsFromUserInfoEndpoint = true;
        options.RequireHttpsMetadata = true;

        options.Events = new OpenIdConnectEvents
        {
            OnAccessDenied = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/");
                return Task.CompletedTask;
            }
        };        

    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
