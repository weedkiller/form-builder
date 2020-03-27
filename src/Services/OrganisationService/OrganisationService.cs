﻿using form_builder.Enum;
using form_builder.Helpers.PageHelpers;
using form_builder.Models;
using form_builder.Providers.StorageProvider;
using form_builder.Services.PageService.Entities;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Models.Verint.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportGovUK.NetStandard.Models.Enums;
using StockportGovUK.NetStandard.Gateways.OrganisationServiceGateway;
using StockportGovUK.NetStandard.Models.Organisation;

namespace form_builder.Services.OrganisationService
{
    public interface IOrganisationService
    {
        Task<ProcessRequestEntity> ProcesssOrganisation(Dictionary<string, dynamic> viewModel, Page currentPage, FormSchema baseForm, string guid, string path);
    }

    public class OrganisationService : IOrganisationService
    {
        private readonly IDistributedCacheWrapper _distributedCache;
        private readonly IPageHelper _pageHelper;
        private readonly IOrganisationServiceGateway _organisationServiceGateway;
        public OrganisationService(IDistributedCacheWrapper distributedCache, IOrganisationServiceGateway organisationServiceGateway, IPageHelper pageHelper)
        {
            _distributedCache = distributedCache;
            _pageHelper = pageHelper;
            _organisationServiceGateway = organisationServiceGateway;
        }

        public async Task<ProcessRequestEntity> ProcesssOrganisation(Dictionary<string, dynamic> viewModel, Page currentPage, FormSchema baseForm, string guid, string path)
        {
            var journey = (string)viewModel["OrganisationStatus"];
            var organisationResults = new List<OrganisationSearchResult>();

            if ((!currentPage.IsValid && journey == "Select") || (currentPage.IsValid && journey == "Search"))
            {
                var cachedAnswers = _distributedCache.GetString(guid);
                var convertedAnswers = cachedAnswers == null
                    ? new FormAnswers { Pages = new List<PageAnswers>() }
                    : JsonConvert.DeserializeObject<FormAnswers>(cachedAnswers);

                var organisationElement = currentPage.Elements.Where(_ => _.Type == EElementType.Organisation).FirstOrDefault();

                var searchTerm = journey == "Select"
                    ? (string)convertedAnswers.Pages.FirstOrDefault(_ => _.PageSlug == path).Answers.FirstOrDefault(_ => _.QuestionId == $"{organisationElement.Properties.QuestionId}-organisation-searchterm").Response
                    : (string)viewModel[$"{organisationElement.Properties.QuestionId}-organisation-searchterm"];

                var organisation = journey != "Select"
                    ? string.Empty
                    : (string)viewModel[$"{organisationElement.Properties.QuestionId}-organisation"];

                var emptySearchTerm = string.IsNullOrEmpty(searchTerm);
                var emptyOrganisation = string.IsNullOrEmpty(organisation);

                if (currentPage.IsValid && organisationElement.Properties.Optional && emptySearchTerm)
                {
                    _pageHelper.SaveAnswers(viewModel, guid, baseForm.BaseURL, null, currentPage.IsValid);
                    return new ProcessRequestEntity
                    {
                        Page = currentPage
                    };
                }

                if (currentPage.IsValid && organisationElement.Properties.Optional && emptyOrganisation && !emptySearchTerm && journey == "Select")
                {
                    _pageHelper.SaveAnswers(viewModel, guid, baseForm.BaseURL, null, currentPage.IsValid);
                    return new ProcessRequestEntity
                    {
                        Page = currentPage
                    };
                }

                var provider = EOrganisationProvider.Unknown;

                try
                {
                    provider = (EOrganisationProvider) System.Enum.Parse(typeof(EOrganisationProvider),
                        organisationElement.Properties.OrganisationProvider, true);
                }
                catch (Exception e)
                {
                    throw new ApplicationException($"No organisation provider configured for {organisationElement.Properties.OrganisationProvider}");
                }

                try
                {
                    var result = await _organisationServiceGateway.SearchAsync(new OrganisationSearch {OrganisationProvider = provider, SearchTerm = searchTerm});
                    organisationResults = result.ResponseContent.ToList();
                }
                catch (Exception e)
                {
                    throw new ApplicationException($"OrganisationService.ProcesssOrganisation:: An exception has occured while attempting to perform organisation lookup, Exception: {e.Message}");
                }
            }

            if (!currentPage.IsValid)
            {
                var formModel = await _pageHelper.GenerateHtml(currentPage, viewModel, baseForm, guid, null, organisationResults);
                formModel.Path = currentPage.PageSlug;
                formModel.OrganisationStatus = journey;
                formModel.FormName = baseForm.FormName;
                formModel.PageTitle = currentPage.Title;

                return new ProcessRequestEntity
                {
                    Page = currentPage,
                    ViewModel = formModel,
                    ViewName = "../Organisation/Index"
                };
            }

            _pageHelper.SaveAnswers(viewModel, guid, baseForm.BaseURL, null, currentPage.IsValid);
            return await _pageHelper.ProcessOrganisationJourney(journey, currentPage, viewModel, baseForm, guid, organisationResults);
        }
    }
}
