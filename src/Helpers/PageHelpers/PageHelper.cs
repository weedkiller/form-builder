﻿using form_builder.Configuration;
using form_builder.Enum;
using form_builder.Helpers.ElementHelpers;
using form_builder.Models;
using form_builder.Models.Elements;
using form_builder.Models.Properties;
using form_builder.Providers.StorageProvider;
using form_builder.Services.PageService.Entities;
using form_builder.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Models.Addresses;
using StockportGovUK.NetStandard.Models.Models.Verint.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace form_builder.Helpers.PageHelpers
{
    public interface IPageHelper
    {
        void CheckForDuplicateQuestionIDs(Page page);
        Task<FormBuilderViewModel> GenerateHtml(Page page, Dictionary<string, string> viewModel, FormSchema baseForm, string guid, List<AddressSearchResult> addressSearchResults = null, List<OrganisationSearchResult> organisationSearchResults = null);
        void SaveAnswers(Dictionary<string, string> viewModel, string guid);
        Task<ProcessRequestEntity> ProcessOrganisationJourney(string journey, Page currentPage, Dictionary<string, string> viewModel, FormSchema baseForm, string guid, List<OrganisationSearchResult> organisationResults);
        Task<ProcessRequestEntity> ProcessStreetJourney(string journey, Page currentPage, Dictionary<string, string> viewModel, FormSchema baseForm, string guid, List<AddressSearchResult> addressResults);
        Task<ProcessRequestEntity> ProcessAddressJourney(string journey, Page currentPage, Dictionary<string, string> viewModel, FormSchema baseForm, string guid, List<AddressSearchResult> addressResults);
        bool hasDuplicateQuestionIDs(List<Page> pages);
    }

    public class PageHelper : IPageHelper
    {
        private readonly IViewRender _viewRender;
        private readonly IElementHelper _elementHelper;
        private readonly IDistributedCacheWrapper _distributedCache;
        private readonly DisallowedAnswerKeysConfiguration _disallowedKeys;
        private readonly IHostingEnvironment _enviroment;
        public PageHelper(IViewRender viewRender, IElementHelper elementHelper, IDistributedCacheWrapper distributedCache, IOptions<DisallowedAnswerKeysConfiguration> disallowedKeys, IHostingEnvironment enviroment)
        {
            _viewRender = viewRender;
            _elementHelper = elementHelper;
            _distributedCache = distributedCache;
            _disallowedKeys = disallowedKeys.Value;
            _enviroment = enviroment;
        }

        public void CheckForDuplicateQuestionIDs(Page page)
        {
            var numberOfDuplicates = page.Elements.GroupBy(x => x.Properties.QuestionId + x.Properties.Text + x.Type)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();

            if (numberOfDuplicates.Count > 0)
            {
                throw new Exception("Question id, text or type is not unique.");
            }
        }

        public async Task<FormBuilderViewModel> GenerateHtml(Page page, Dictionary<string, string> viewModel, FormSchema baseForm, string guid, List<AddressSearchResult> addressAndStreetSearchResults = null, List<OrganisationSearchResult> organisationSearchResults = null)
        {
            FormBuilderViewModel formModel = new FormBuilderViewModel();
            if (page.PageSlug.ToLower() != "success")
            {
                formModel.RawHTML += await _viewRender.RenderAsync("H1", new Element { Properties = new BaseProperty { Text = baseForm.FormName } });
            }
            formModel.FeedbackForm = baseForm.FeedbackForm;

            CheckForDuplicateQuestionIDs(page);

            foreach (var element in page.Elements)
            {
                formModel.RawHTML += await element.RenderAsync(_viewRender, _elementHelper, guid, addressAndStreetSearchResults, organisationSearchResults, viewModel, page, baseForm, _enviroment);
            }

            return formModel;
        }

        public void SaveAnswers(Dictionary<string, string> viewModel, string guid)
        {
            var formData = _distributedCache.GetString(guid);
            var convertedAnswers = new FormAnswers { Pages = new List<PageAnswers>() };

            if (!string.IsNullOrEmpty(formData))
            {
                convertedAnswers = JsonConvert.DeserializeObject<FormAnswers>(formData);
            }

            if (convertedAnswers.Pages != null && convertedAnswers.Pages.Any(_ => _.PageSlug == viewModel["Path"].ToLower()))
            {
                convertedAnswers.Pages = convertedAnswers.Pages.Where(_ => _.PageSlug != viewModel["Path"].ToLower()).ToList();
            }

            var answers = new List<Answers>();

            foreach (var item in viewModel)
            {
                if (!_disallowedKeys.DisallowedAnswerKeys.Contains(item.Key))
                {
                    answers.Add(new Answers { QuestionId = item.Key, Response = item.Value });
                }
            }

            convertedAnswers.Pages.Add(new PageAnswers
            {
                PageSlug = viewModel["Path"].ToLower(),
                Answers = answers
            });

            convertedAnswers.Path = viewModel["Path"];

            _distributedCache.SetStringAsync(guid, JsonConvert.SerializeObject(convertedAnswers));
        }

        public async Task<ProcessRequestEntity> ProcessStreetJourney(string journey, Page currentPage, Dictionary<string, string> viewModel, FormSchema baseForm, string guid, List<AddressSearchResult> addressResults)
        {
            switch (journey)
            {
                case "Search":
                    try
                    {
                        var streetViewModel = await GenerateHtml(currentPage, viewModel, baseForm, guid, addressResults, null);
                        streetViewModel.StreetStatus = "Select";
                        streetViewModel.FormName = baseForm.FormName;
                        streetViewModel.PageTitle = currentPage.Title;

                        return new ProcessRequestEntity
                        {
                            Page = currentPage,
                            ViewModel = streetViewModel,
                            UseGeneratedViewModel = true,
                            ViewName = "../Street/Index"
                        };
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException($"PageHelper.ProcessStreetJourney: An exception has occured while attempting to generate Html, Exception: {e.Message}");
                    };
                case "Select":
                    return new ProcessRequestEntity
                    {
                        Page = currentPage
                    };
                default:
                    throw new ApplicationException($"PageHelper.ProcessStreetJourney: Unknown journey type");
            }
        }
        
        public async Task<ProcessRequestEntity> ProcessAddressJourney(string journey, Page currentPage, Dictionary<string, string> viewModel, FormSchema baseForm, string guid, List<AddressSearchResult> addressResults)
        {
            switch (journey)
            {
                case "Search":
                    try
                    {
                        var adddressViewModel = await GenerateHtml(currentPage, viewModel, baseForm, guid, addressResults, null);
                        adddressViewModel.AddressStatus = "Select";
                        adddressViewModel.FormName = baseForm.FormName;
                        adddressViewModel.PageTitle = currentPage.Title;

                        return new ProcessRequestEntity
                        {
                            Page = currentPage,
                            ViewModel = adddressViewModel,
                            UseGeneratedViewModel = true,
                            ViewName = "../Address/Index"
                        };
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException($"PageHelper.ProcessAddressJourney: An exception has occured while attempting to generate Html, Exception: {e.Message}");
                    };
                case "Select":
                    return new ProcessRequestEntity
                    {
                        Page = currentPage
                    };
                default:
                    throw new ApplicationException($"PageHelper.ProcessAddressJourney: Unknown journey type");
            }
        }

        public async Task<ProcessRequestEntity> ProcessOrganisationJourney(string journey, Page currentPage, Dictionary<string, string> viewModel, FormSchema baseForm, string guid, List<OrganisationSearchResult> organisationResults)
        {
            switch (journey)
            {
                case "Search":
                    try
                    {
                        var organisationViewModel = await GenerateHtml(currentPage, viewModel, baseForm, guid, null, organisationResults);
                        organisationViewModel.OrganisationStatus = "Select";
                        organisationViewModel.FormName = baseForm.FormName;
                        organisationViewModel.PageTitle = currentPage.Title;

                        return new ProcessRequestEntity
                        {
                            Page = currentPage,
                            ViewModel = organisationViewModel,
                            UseGeneratedViewModel = true,
                            ViewName = "../Organisation/Index"
                        };
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException($"PageHelper.ProcessOrganisationJourney: An exception has occured while attempting to generate Html, Exception: {e.Message}");
                    };
                case "Select":
                    return new ProcessRequestEntity
                    {
                        Page = currentPage
                    };
                default:
                    throw new ApplicationException($"PageHelper.ProcessOrganisationJourney: Unknown journey type");
            }
        }

        public bool hasDuplicateQuestionIDs(List<Page> pages)
        {
            bool duplicateFound = false;
            List<string> qIds = new List<string>();
            foreach (var page in pages)
            {
                foreach (var element in page.Elements)
                {
                    if (
                        element.Type == EElementType.Address
                        || element.Type == EElementType.Textbox
                        || element.Type == EElementType.Textarea
                        || element.Type == EElementType.Select
                        || element.Type == EElementType.Radio
                        || element.Type == EElementType.Street
                        || element.Type == EElementType.Checkbox
                        || element.Type == EElementType.DateInput
                        || element.Type == EElementType.TimeInput
                        || element.Type == EElementType.Organisation
                        )
                    {
                        qIds.Add(element.Properties.QuestionId);
                    }
                }
            }

            var hashSet = new HashSet<string>();
            foreach(var id in qIds)
            {
                if (!hashSet.Add(id))
                {
                    duplicateFound = true;
                    return duplicateFound;
                }
            }

            return duplicateFound;
        }
    }
}