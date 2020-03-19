﻿using form_builder.Enum;
using form_builder.Extensions;
using form_builder.Helpers;
using form_builder.Helpers.ElementHelpers;
using form_builder.ViewModels;
using Microsoft.AspNetCore.Hosting;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Verint.Lookup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace form_builder.Models.Elements
{
    public class AddressManual : Element
    {
        public AddressManual()
        {
            Type = EElementType.AddressManual;
        }

        public override string GenerateFieldsetProperties(){
            if(!string.IsNullOrWhiteSpace(Properties.AddressManualHint)){
                return $"aria-describedby = {Properties.QuestionId}-hint";
            }
            
            return string.Empty;
        }
        
        protected void SetAddressProperties(Dictionary<string, dynamic> viewModel, string searchTerm)
        {
            Properties.AddressManualAddressLine1 = viewModel.FirstOrDefault(_ => _.Key.Contains("AddressManualAddressLine1")).Value;
            Properties.AddressManualAddressLine2 = viewModel.FirstOrDefault(_ => _.Key.Contains("AddressManualAddressLine2")).Value;
            Properties.AddressManualAddressTown = viewModel.FirstOrDefault(_ => _.Key.Contains("AddressManualAddressTown")).Value;
            Properties.AddressManualAddressPostcode = viewModel.FirstOrDefault(_ => _.Key.Contains("AddressManualAddressPostcode")).Value ?? searchTerm;           
        }

        public override async Task<string> RenderAsync(IViewRender viewRender, IElementHelper elementHelper, string guid, List<AddressSearchResult> addressSearchResults, List<OrganisationSearchResult> organisationResults, Dictionary<string, dynamic> viewModel, Page page, FormSchema formSchema, IHostingEnvironment environment)
        {

            if (viewModel.ContainsKey("AddressStatus"))
            {
                viewModel.Remove("AddressStatus");
            };

            viewModel.Add("AddressStatus", "Manual");
            Properties.Value = elementHelper.CurrentValue(this, viewModel, page.PageSlug, guid, "-postcode");
            
            SetAddressProperties(viewModel, Properties.Value);

            var searchResultsCount =  elementHelper.GetFormDataValue(guid, $"{Properties.QuestionId}-srcount");

            var isValid = int.TryParse(searchResultsCount.ToString(), out int output);

            if (isValid && output == 0)
            {

                Properties.DisplayNoResultsIAG = true;
            }

            var viewElement = new ElementViewModel
            {
                Element = this,
                ManualAddressURL = $"{environment.EnvironmentName.ToReturnUrlPrefix()}/{formSchema.BaseURL}/{page.PageSlug}"
        };

            return await viewRender.RenderAsync(Type.ToString(), viewElement);
        }
    }
}
