using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Configuration;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.Core.Events;
using IgniteCachingDistributed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace IgniteCachingTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddDistributedIgniteCache(option =>
            //{
            //    option.Endpoints = new string[]
            //        {
            //            "localhost:11211",
            //            "localhost:47100",
            //            "localhost:47500",
            //            "localhost:49112"
            //        };
            //    option.PersistenceEnabled = true;
            //});

            IgniteConfiguration customeConfiguration = new IgniteConfiguration
            {
                DiscoverySpi = new TcpDiscoverySpi
                {
                    IpFinder = new TcpDiscoveryStaticIpFinder
                    {
                        Endpoints = new string[]
                        {
                            "localhost:11211",
                            "localhost:47100",
                            "localhost:47500",
                            "localhost:49112"
                        }
                    },
                    SocketTimeout = TimeSpan.FromSeconds(0.3)
                },
                IncludedEventTypes = EventType.CacheAll
            };
            customeConfiguration.DataStorageConfiguration = new DataStorageConfiguration
            {
                DefaultDataRegionConfiguration = new DataRegionConfiguration
                {
                    Name = "defaultRegion",
                    PersistenceEnabled = true
                },
                DataRegionConfigurations = new[]
                    {
                            new DataRegionConfiguration
                            {
                                // Persistence is off by default.
                                Name = "inMemoryRegion"
                            }
                        }
            };
            customeConfiguration.CacheConfiguration = new[]
            {
                    new CacheConfiguration
                    {
                        // Default data region has persistence enabled.
                        Name = "persistentCache"
                    },
                    new CacheConfiguration
                    {
                        Name = "inMemoryOnlyCache",
                        DataRegionName = "inMemoryRegion"
                    }
                };

            services.AddDistributedIgniteCache(option => 
            { 
                option.Configuration = customeConfiguration;
                option.SetActive = true;
            });

           
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
