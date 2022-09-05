using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Common
{
    public static class ConsulServiceExtension
    {
        public static IServiceCollection AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(_ => new ConsulClient(consulConfig =>
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

            if (!(app.Properties["server.Features"] is FeatureCollection))
            {
                return app;
            }

           
            var serviceAddresses = configuration.GetSection("ConsulConfig:ServiceAddress").Get<IList<string>>();
            var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");
            foreach (var address in serviceAddresses)
            {
                var split = address.Split(":");
                var serviceIp = split[0];
                var servicePort = int.Parse(split[1]);
                var serviceId = $"{serviceName}-{servicePort}";
                var registration = new AgentServiceRegistration()
                {
                    ID = serviceId,
                    Name = serviceName,
                    Address = serviceIp,
                    Port = servicePort,

                    Check = new AgentCheckRegistration()
                    {
                        HTTP = $"http://{serviceIp}:{servicePort}/health",
                        Interval = TimeSpan.FromSeconds(10)
                    }
                };

                consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

                lifetime.ApplicationStopping.Register(() =>
                {
                    consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
                });
            }
          

            return app;

        }
    }
}
