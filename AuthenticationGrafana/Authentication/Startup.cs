using Joonasw.AspNetCore.SecurityHeaders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Authentication.Configuration;
using Authentication.Data;
using Authentication.Interfaces;
using Authentication.Models;
using Authentication.Quickstart.Account;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using System.Linq;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Mvc;
using X509Helper;
using Microsoft.AspNetCore.Rewrite;

namespace Authentication
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

            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                //options.SslPort = 5000; //kestrel
                //options.Filters.Add(new RequireHttpsAttribute()); //kestrel
            });
            //.SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            //kestrel
            //services.AddAntiforgery(options =>
            //{
            //    options.Cookie.Name = "_af";
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            //    options.HeaderName = "X-XSRF-TOKEN";
            //});

            services.Configure<MySettingsModel>(Configuration.GetSection("MySettings"));

            services.AddScoped<IGrafanaHelper, GrafanaHelper>();
            services.AddScoped<IGrafRole, GrafRoleHelper>();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var signinCredential = X509.GetCertificate("7948D0F59B78BCF6B165C84BE61834DEFD96AEA0");
            var validationKey = X509.GetCertificate("2BF154F3D1F8BC6743238ACB22AED7C31AA909B1");
            services.AddIdentityServer()
                //.AddDeveloperSigningCredential()
                .AddAspNetIdentity<ApplicationUser>()
                .AddSigningCredential(signinCredential) //self signed localhost cert
                .AddValidationKey(validationKey) //self signed localhost cert

                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(store =>
                {
                    store.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    store.EnableTokenCleanup = true;
                    store.TokenCleanupInterval = 1800;
                })
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                });
                //.AddAspNetIdentity<ApplicationUser>();


            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            services.AddCsp(nonceByteAmount: 32);

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            //this will intialize the identityserver configuration db objects
            //InitializeIdentityServerDatabase(app);
            //InitializeAspIdentityDbObjects(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //kestrel
            app.UseRewriter(new RewriteOptions().AddRedirectToHttps());

            app.UseIdentityServer();

            app.UseCsp(csp =>
            {
                csp.AllowScripts.FromSelf().AddNonce();
            });

            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        private void InitializeIdentityServerDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Clients.Get())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Resources.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Resources.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

        private void InitializeAspIdentityDbObjects(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var applicationContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                applicationContext.Database.Migrate();
            }
        }
    }
}
