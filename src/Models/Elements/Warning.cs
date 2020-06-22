﻿using form_builder.Enum;
using form_builder.Helpers;
using form_builder.Helpers.ElementHelpers;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace form_builder.Models.Elements
{
    public class Warning : Element
    {
        public Warning()
        {
            Type = EElementType.InlineAlert;
        }

        public override Task<string> RenderAsync(IViewRender viewRender,
            IElementHelper elementHelper,
            string guid,
            Dictionary<string, dynamic> viewModel,
            Page page,
            FormSchema formSchema,
            IHostingEnvironment environment,
            List<object> results = null)
        {
            elementHelper.CheckIfLabelAndTextEmpty(this);
            return viewRender.RenderAsync(Type.ToString(), this);
        }
    }
}