﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HubSpot.Model.Companies;
using HubSpot.Model.Contacts;
using HubSpot.Model.CRM.Associations;
using HubSpot.Model.Deals;
using HubSpot.Model.Lists;
using HubSpot.Model.Owners;
using HubSpot.Utils;
using Kralizek.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HubSpot
{
    public partial class HttpHubSpotClient : HttpRestClient, IHubSpotClient
    {
        private readonly ILogger<HttpHubSpotClient> _logger;

        public HttpHubSpotClient(HubSpotAuthenticator authenticator, ILogger<HttpHubSpotClient> logger) : base(CreateClient(authenticator), SerializerSettings, logger)
        {
            if (authenticator == null) throw new ArgumentNullException(nameof(authenticator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private static HttpClient CreateClient(HubSpotAuthenticator authenticator)
        {
            return new HttpClient(authenticator) { BaseAddress = authenticator.ServiceUri };
        }

        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Converters = new JsonConverter[]
            {
                new UnixEpochConverter(),
                new StringEnumConverter()
            }
        };

        public IHubSpotContactClient Contacts => this;

        public IHubSpotCompanyClient Companies => this;

        public IHubSpotDealClient Deals => this;

        public IHubSpotListClient Lists => this;

        public IHubSpotOwnerClient Owners => this;

        public IHubSpotCrmClient Crm => this;
    }
}