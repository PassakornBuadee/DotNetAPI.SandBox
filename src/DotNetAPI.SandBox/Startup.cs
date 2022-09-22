using DotNetAPI.SandBox.DBContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DotNetAPI.SandBox
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //Separate DI Container for use in Startup Only
            IServiceCollection appSettingsServiceCollections = new ServiceCollection();
            appSettingsServiceCollections.AddOptions<AppSettings>()
                .Bind(Configuration.GetSection(nameof(AppSettings)))
                .Configure(m => {
                    m.Configure();
                });

            var appSettingsServiceProvider = appSettingsServiceCollections.BuildServiceProvider();
            AppSettingOptions = appSettingsServiceProvider.GetRequiredService<IOptionsMonitor<AppSettings>>();
        }

        public IConfiguration Configuration { get; }
        public IOptionsMonitor<AppSettings>? AppSettingOptions { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            if(AppSettingOptions == null) throw new NullReferenceException(nameof(AppSettingOptions));

            //Change to Scope due to capture change of current value of each request
            //Pros: no need to update existed DI constructures to IOptionMonitor
            services.AddScoped<AppSettings>(m => AppSettingOptions.CurrentValue);

            services.AddDbContext<SandBoxDbContext>(o => {
                if (AppSettingOptions.CurrentValue.ConnectionStrings?.SandBoxConnectionString?.GetValue() != null)
                {
                    o.UseSqlServer(AppSettingOptions.CurrentValue.ConnectionStrings.SandBoxConnectionString.GetValue());
                }
            }, ServiceLifetime.Transient);

            services.AddMvcCore();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new()
                {
                    Title = "API SandBox",
                    Version = "v1"
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
                                                $"API SandBox v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class Initializer
    {
        private IOptionsMonitor<AppSettings> _options;
        public Initializer(IOptionsMonitor<AppSettings>? options)
        {
            if(options == null) throw new ArgumentNullException(nameof(options));
            _options = options;
        }

        public IOptionsMonitor<AppSettings> GetOptions()
        {
            return _options;
        }
    }

}
