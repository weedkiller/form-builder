﻿using form_builder.Enum;
using form_builder.Helpers;
using form_builder.Helpers.ElementHelpers;
using Microsoft.AspNetCore.Hosting;
using StockportGovUK.NetStandard.Models.Addresses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace form_builder.Models.Elements
{
    public class TimeInput : Element
    {
        public TimeInput()
        {
            Type = EElementType.TimeInput;
        }

        public override Task<string> RenderAsync(IViewRender viewRender, IElementHelper elementHelper, string guid, List<AddressSearchResult> addressSearchResults, Dictionary<string, string> viewModel, Page page, FormSchema formSchema, IHostingEnvironment environment)
        {
            Properties.Hours = elementHelper.CurrentValue(this, viewModel, page.PageSlug, guid, "-hours");
            Properties.Minutes = elementHelper.CurrentValue(this, viewModel, page.PageSlug, guid, "-minutes");
            Properties.AmPm = elementHelper.CurrentValue(this, viewModel, page.PageSlug, guid, "-ampm");
            elementHelper.CheckForQuestionId(this);
            elementHelper.CheckForLabel(this);
            return viewRender.RenderAsync(Type.ToString(), this);
        }
    }
}