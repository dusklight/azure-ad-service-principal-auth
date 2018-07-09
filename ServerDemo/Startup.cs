using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ServerDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            string tenantId = "[Your Azure Active Directory TenantID]";

            string issuer = $"https://sts.windows.net/{tenantId}/";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://login.microsoftonline.com/common/";
                    options.SaveToken = true;

                    // (Note: See Azure SDK source code for default values on TokenValidationParameters)
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,

                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,

                        ValidateAudience = true,

                        // Not clear on which value this should be -- the client's or the server's?  Using the server for now.
                        ValidAudiences = new string[]
                        {
                            // The Application ID registered in Azure Active Directory for this API/Server Application.
                            "[Application ID of the API/Server]",

                            // The API/Server Application's URI set when it was registered in Azure Active Directory.
                            // It doesn't have to be a functioning URL, it's more like a namespace.
                            "https://[Your AD Tenant Name].onmicrosoft.com/ServerDemo"

                            // You don't have to specify both above, it can be just one, just make sure the Client sends in the value being used.
                        },

                    };
                });

            services.AddAuthorization(configure =>
            {
                // This policy verifies that the issuer claim "appid" that was returned from Azure Active Directory matches the
                // Client's Application/Service Principal ID specified.  Add this policy to the Authorize Attribute when you only
                // want the Application to be able to access the API (and not anyone else who are part of the AD).
                configure.AddPolicy("IsValidClientUser", policy => policy.RequireClaim("appid", "[Your Client's Application/Service Principal ID]"));
            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication(); // Do not forget this line!

            app.UseMvc();
        }
    }
}
