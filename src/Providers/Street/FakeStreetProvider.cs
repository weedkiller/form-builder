﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Addresses;

namespace form_builder.Providers.Street
{
    public class FakeStreetProvider : IStreetProvider
    {
        public string ProviderName { get => "FakeStreet"; }
        public async Task<IEnumerable<AddressSearchResult>> SearchAsync(string street)
        {
            return new List<AddressSearchResult> {
                new AddressSearchResult {
                    Name = "Green lane",
                    UniqueId = "123456789012"
                },
                 new AddressSearchResult {
                    Name = "Green road",
                    UniqueId = "098765432109"
                },
                 new AddressSearchResult {
                    Name = "Green street",
                    UniqueId = "564737838937"
                }
            };
        }
    }
}