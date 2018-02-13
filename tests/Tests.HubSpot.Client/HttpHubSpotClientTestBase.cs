﻿using System.Net.Http;
using HubSpot;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using WorldDomination.Net.Http;

namespace Tests
{
    public abstract class HttpHubSpotClientTestBase
    {
        protected static HttpContent EmptyObject => new StringContent("{}");

        protected static HttpContent EmptyArray => new StringContent("[]");

        protected static HttpContent Object<T>(T obj) => new StringContent(JsonConvert.SerializeObject(obj, HttpHubSpotClient.SerializerSettings));

        protected HttpHubSpotClient CreateClient(params HttpMessageOptions[] options)
        {
            if (options == null || options.Length == 0)
            {
                options = new[]
                {
                    new HttpMessageOptions()
                };
            }

            var handler = new FakeHttpMessageHandler(options);
            var authenticator = new TestHubSpotAuthenticator(handler);

            var client = new HttpHubSpotClient(authenticator, Mock.Of<ILogger<HttpHubSpotClient>>());

            return client;
        }

    }
}