using System;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common
{
    public static class ConsulServiceExtension
    {
        public static IServiceCollection AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = configuration.GetValue<string>("ConsulConfig:ConsulHost");
                consulConfig.Address = new Uri(address);
            }));
            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            if (!(app.Properties["server.Features"] is FeatureCollection features))
            {
                return app;
            }


            var servicePort = int.Parse(configuration.GetValue<string>("ConsulConfig:ServicePort"));
       
            var serviceIp = "192.168.43.152";
            var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");
            var serviceId = $"{serviceName}-{servicePort}";


          
            var registration = new AgentServiceRegistration()
            {
                ID = $"{serviceId}_{serviceName}_{servicePort}",
                Name = serviceName,
                Address = serviceIp,
                Port = servicePort,

                Check = new AgentCheckRegistration()
                {
                    HTTP = $"http://{serviceIp}:{servicePort}/health",
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            });

            return app;

        }
    }
}
