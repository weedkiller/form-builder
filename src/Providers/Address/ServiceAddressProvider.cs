﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StockportGovUK.NetStandard.Gateways.AddressService;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Enums;

namespace form_builder.Providers.Address
{
    public class ServiceAddressProvider : IAddressProvider
    {
        public string ProviderName => "CRM";

        private readonly IAddressServiceGateway _addressServiceGateway;
        private readonly ILogger<IAddressProvider> _logger;
        public ServiceAddressProvider(IAddressServiceGateway addressServiceGateway, ILogger<IAddressProvider> logger)
        {
            _addressServiceGateway = addressServiceGateway;
            _logger = logger;
        }

        public async Task<IEnumerable<AddressSearchResult>> SearchAsync(string streetOrPostcode)
        {
            var response = await _addressServiceGateway.SearchAsync(new AddressSearch { AddressProvider = EAddressProvider.CRM, SearchTerm = streetOrPostcode });

            return response.ResponseContent;
        }
    }
}