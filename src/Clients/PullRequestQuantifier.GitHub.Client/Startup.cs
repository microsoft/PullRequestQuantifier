namespace PullRequestQuantifier.GitHub.Client
***REMOVED***
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    public class Startup
    ***REMOVED***
        public Startup(IConfiguration configuration)
        ***REMOVED***
            Configuration = configuration;
***REMOVED***

        public IConfiguration Configuration ***REMOVED*** get; ***REMOVED***

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        ***REMOVED***
            services.AddControllers();
            services.AddSwaggerGen(c => ***REMOVED*** c.SwaggerDoc("v1", new OpenApiInfo ***REMOVED*** Title = "PullRequestQuantifier.GitHub.Client", Version = "v1" ***REMOVED***); ***REMOVED***);
***REMOVED***

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        ***REMOVED***
            if (env.IsDevelopment())
            ***REMOVED***
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PullRequestQuantifier.GitHub.Client v1"));
    ***REMOVED***

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => ***REMOVED*** endpoints.MapControllers(); ***REMOVED***);
***REMOVED***
***REMOVED***
***REMOVED***
