﻿using System.Threading.Tasks;
using form_builder.Enum;
using form_builder.Models;
using form_builder.Services.PageService.Entities;

namespace form_builder.ContentFactory.SuccessPageFactory
{
    public interface ISuccessPageFactory
    {
        Task<SuccessPageEntity> Build(string form, FormSchema baseForm, string sessionGuid, FormAnswers formAnswers, EBehaviourType behaviourType);
    }
}
