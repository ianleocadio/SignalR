using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(credentials => Task.FromResult(
                    credentials.username == "Filial 1"
                    && credentials.password == "Filial1Password"));

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");

            app.UseCors(CorsConstants.AnyOrigin);

            
            app.UseAuthentication();

            app.UseRouting();
            app.UseEndpoints(ep =>
            {
                ep.MapHub<MainHub>("/ImpressaoPostoColeta");
                
            });
        }
    }
}