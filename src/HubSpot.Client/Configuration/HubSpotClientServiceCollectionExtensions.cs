using Microsoft.Extensions.DependencyInjection;
using Kralizek.Extensions.Http;
using System;
using HubSpot.Configuration;
using HubSpot;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HubSpotClientServiceCollectionExtensions
    {
        public const string HttpClientConfigurationName = "HubSpot";

        public static IServiceCollection AddHubSpotClient(this IServiceCollection services, Action<IHubSpotClientConfigurator> configuration = null)
        {
            var configurator = new HubSpotClientConfigurator();

            configuration?.Invoke(configurator);

            services.AddHttpRestClient(HttpClientConfigurationName, builder => 
            {
                builder.ConfigureHttpClient(http => 
                {
                    http.BaseAddress = HttpHubSpotClient.DefaultApiEndpoint;
                });

                builder.ConfigureSerialization(HttpHubSpotClient.ConfigureJsonSerializer);

                configurator.ApplyHttpClientBuilderConfigurations(builder);
            });

            configurator.ApplyServiceConfigurations(services);

            services.AddTransient<IHubSpotClient, HttpHubSpotClient>();

            return services;
        }

        public static HttpClient CreateRawHubSpotHttpClient(this IHttpClientFactory factory)
        {
            return factory.CreateClient(HttpClientConfigurationName);
        }

        public static HttpClient GetRawHubSpotHttpClient(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IHttpClientFactory>().CreateRawHubSpotHttpClient();
        }
    }
}