﻿using form_builder.Enum;
using form_builder.Extensions;
using form_builder.Helpers;
using form_builder.Helpers.ElementHelpers;
using form_builder.ViewModels;
using Microsoft.AspNetCore.Hosting;
using StockportGovUK.NetStandard.Models.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace form_builder.Models.Elements
{
    public class AddressManual : Element, IElement
    {
        public AddressManual()
        {
            Type = EElementType.AddressManual;
        }

        protected void SetAddressProperties(Dictionary<string, string> viewModel)
        {
            Properties.AddressManualAddressLine1 = viewModel.FirstOrDefault(_ => _.Key.Contains("AddressManualAddressLine1")).Value;
            Properties.AddressManualAddressLine2 = viewModel.FirstOrDefault(_ => _.Key.Contains("AddressManualAddressLine2")).Value;
            Properties.AddressManualAddressTown = viewModel.FirstOrDefault(_ => _.Key.Contains("AddressManualAddressTown")).Value;
            Properties.AddressManualAddressPostcode = viewModel.FirstOrDefault(_ => _.Key.Contains("AddressManualAddressPostcode")).Value;
        }


        public override Task<string> RenderAsync(IViewRender viewRender, IElementHelper elementHelper, string guid, List<AddressSearchResult> addressSearchResults, Dictionary<string, string> viewModel, Page page, FormSchema formSchema, IHostingEnvironment environment)
        {

            SetAddressProperties(viewModel);
            return viewRender.RenderAsync(Type.ToString(), this, null);
        }

    }
}
