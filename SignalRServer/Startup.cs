using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SignalRServer.Authentication;
using SignalRServer.Caller.Controllers;
using SignalRServer.Caller.Models;
using SignalRServer.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // Ajustar para ambiente de implantação com arquivo de configuração
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowAnyOrigin();
            }));

            #region Método temporário para verificar autenticação
            Task<bool> Authenticate(string username, string password)
            {
                return Task.FromResult(
                        (username == "Filial 1" && password == "Filial1Password") 
                     || (username == "Filial 2" && password == "Filial2Password")
                     || (username == "Filial 3" && password == "Filial3Password")
                    );
            }
            #endregion

            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(credentials => Authenticate(credentials.username, credentials.password));

            services.AddAuthorization();

            services.AddSingleton<IUserIdProvider, NameUserIdProvider>()
                .AddSingleton<ImprimeCallerController>((provider) => 
                    new ImprimeCallerController(provider.GetRequiredService<ILogger<ImprimeCallerController>>()));

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(3);
            });

        }

        public void Configure(IApplicationBuilder app)
        {
            
            app.UseRouting();

            app.UseCors("CorsPolicy");
            app.UseCors(CorsConstants.AnyOrigin);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(ep =>
            {
                ep.MapHub<MainHub>("/ImpressaoPostoColeta");
                
            });
        }
    }
}