using EY.ITP.API.DBContext;
using EY.ITP.API.Interfaces.Providers;
using EY.ITP.API.Interfaces.Services;
using EY.ITP.API.Providers;
using EY.ITP.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace EY.ITP.API
{
    public class Startup
    {
        public Startup(IConfigurationRoot configuration) 
        {
            Configuration = configuration;
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region Services
            services.AddScoped<IFileValidationService, FileValidationService>();
            services.AddScoped<IAccountTypeService, AccountTypeService>();
            services.AddScoped<IDisclosuresService, DisclosuresService>();
            #endregion

            #region Providers
            services.AddScoped<IFileValidationProvider, FileValidationProvider>();
            services.AddScoped<IAccountTypeProvider, AccountTypeProvider>();
            services.AddScoped<IDisclosuresProvider, DisclosuresProvider>();
            #endregion

            #region DbContext
            services.AddDbContext<CoreDBContext>(
                options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<CommonDBContext>(
                options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection")));
            #endregion
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
