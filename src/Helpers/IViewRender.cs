﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace form_builder.Helpers
{
    public interface IViewRender
    {
        Task<string> RenderAsync<TModel>(string viewName, TModel model, Dictionary<string, object> viewData = null);
    }
}
