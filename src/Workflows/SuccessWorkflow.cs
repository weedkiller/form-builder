﻿using System.Linq;
using System.Threading.Tasks;
using form_builder.Enum;
using form_builder.Factories.Schema;
using form_builder.Services.ActionService;
using form_builder.Services.PageService;
using form_builder.Services.PageService.Entities;

namespace form_builder.Workflows
{
    public interface ISuccessWorkflow
    {
        Task<SuccessPageEntity> Process(string form, string caseRef);
    }

    public class SuccessWorkflow : ISuccessWorkflow
    {
        private readonly IPageService _pageService;
        private readonly ISchemaFactory _schemaFactory;
        private readonly IActionService _actionService;

        public SuccessWorkflow(IPageService pageService, ISchemaFactory schemaFactory, IActionService actionService)
        {
            _pageService = pageService;
            _schemaFactory = schemaFactory;
            _actionService = actionService;
        }

        public async Task<SuccessPageEntity> Process(string form, string caseRef)
        {
            var baseForm = await _schemaFactory.Build(form);

            if (baseForm.FormActions != null && baseForm.FormActions.Any())
            {
                await _actionService.Process(baseForm, caseRef);
            }

            var result = await _pageService.FinalisePageJourney(form, EBehaviourType.SubmitForm);
            return result;
        }
    }
}