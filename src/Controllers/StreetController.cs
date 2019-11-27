﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using form_builder.Models;
using form_builder.Enum;
using form_builder.Validators;
using System.Threading.Tasks;
using System;
using StockportGovUK.AspNetCore.Gateways;
using form_builder.Helpers.PageHelpers;
using form_builder.Providers.SchemaProvider;
using form_builder.Providers.StorageProvider;
using Microsoft.Extensions.Logging;
using form_builder.Providers.Street;
using System.Linq;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Models.Addresses;
using form_builder.Helpers.Session;

namespace form_builder.Controllers
{
    public class StreetController : Controller
    {
        private readonly IDistributedCacheWrapper _distributedCache;

        private readonly IEnumerable<IElementValidator> _validators;

        private readonly ISchemaProvider _schemaProvider;

        private readonly IGateway _gateway;

        private readonly IPageHelper _pageHelper;

        private readonly ILogger<HomeController> _logger;

        private readonly IEnumerable<IStreetProvider> _streetProviders;

        private readonly ISessionHelper _sessionHelper;

        public StreetController(ILogger<HomeController> logger, IDistributedCacheWrapper distributedCache, IEnumerable<IElementValidator> validators, ISchemaProvider schemaProvider, IGateway gateway, IPageHelper pageHelper, IEnumerable<IStreetProvider> streetProviders, ISessionHelper sessionHelper)
        {
            _distributedCache = distributedCache;
            _validators = validators;
            _schemaProvider = schemaProvider;
            _gateway = gateway;
            _pageHelper = pageHelper;
            _logger = logger;
            _streetProviders = streetProviders;
            _sessionHelper = sessionHelper;
        }

        [HttpGet]
        [Route("{form}/{path}/street")]
        public async Task<IActionResult> Index(string form, string path)
        {
            try
            {
                var sessionGuid = _sessionHelper.GetSessionGuid();

                if (sessionGuid == null)
                {
                    sessionGuid = Guid.NewGuid().ToString();
                    _sessionHelper.SetSessionGuid(sessionGuid);
                }

                var baseForm = await _schemaProvider.Get<FormSchema>(form);

                if (string.IsNullOrEmpty(path))
                {
                    path = baseForm.StartPageSlug;
                }

                var page = baseForm.GetPage(path);
                if (page == null)
                {
                    return RedirectToAction("Error");
                }

                var viewModel = await _pageHelper.GenerateHtml(page, new Dictionary<string, string>(), baseForm, sessionGuid);
                viewModel.StreetStatus = "Search";
                viewModel.FormName = baseForm.FormName;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { ex = ex.Message });
            }
        }

        [HttpPost]
        [Route("{form}/{path}/street")]
        public async Task<IActionResult> Index(string form, string path, Dictionary<string, string[]> formData)
        {
            var baseForm = await _schemaProvider.Get<FormSchema>(form);

            var currentPage = baseForm.GetPage(path);

            if (currentPage == null)
            {
                return RedirectToAction("Error");
            }

            var viewModel = NormaliseFormData(formData);
            var guid = _sessionHelper.GetSessionGuid();

            var journey = viewModel["StreetStatus"];
            var addressResults = new List<AddressSearchResult>();

            currentPage.Validate(viewModel, _validators);

            if ((!currentPage.IsValid && journey == "Select") || (currentPage.IsValid && journey == "Search"))
            {
                var cachedAnswers = _distributedCache.GetString(guid.ToString());
                var convertedAnswers = cachedAnswers == null
                    ? new FormAnswers { Pages = new List<PageAnswers>() }
                    : JsonConvert.DeserializeObject<FormAnswers>(cachedAnswers);

                var addressElement = currentPage.Elements.Where(_ => _.Type == EElementType.Street).FirstOrDefault();
                var provider = _streetProviders.ToList()
                    .Where(_ => _.ProviderName == addressElement.Properties.StreetProvider)
                    .FirstOrDefault();

                if (provider == null)
                {
                    return RedirectToAction("Error", "Home", new
                    {
                        form = baseForm.BaseURL,
                        ex = $"No address provider configure for {addressElement.Properties.StreetProvider}"
                    });
                }

                var postcode = journey == "Select"
                    ? convertedAnswers.Pages.FirstOrDefault(_ => _.PageSlug == path).Answers.FirstOrDefault(_ => _.QuestionId == $"{addressElement.Properties.QuestionId}-street").Response
                    : viewModel[$"{addressElement.Properties.QuestionId}-street"];

                try
                {
                    var result = await provider.SearchAsync(postcode);
                    addressResults = result.ToList();
                }
                catch (Exception e)
                {
                    _logger.LogError($"StreetController: An exception has occured while attempting to perform postcode lookup, Exception: {e.Message}");
                    return RedirectToAction("Error", "Home", new { form = baseForm.BaseURL, });
                }
            }

            if (!currentPage.IsValid)
            {
                var formModel = await _pageHelper.GenerateHtml(currentPage, viewModel, baseForm, guid, addressResults);
                formModel.Path = currentPage.PageSlug;
                formModel.StreetStatus = journey;
                formModel.FormName = baseForm.FormName;

                return View(formModel);
            }

            _pageHelper.SaveAnswers(viewModel, guid);

            switch (journey)
            {
                case "Search":
                    try
                    {
                        var adddressViewModel = await _pageHelper.GenerateHtml(currentPage, viewModel, baseForm, guid, addressResults);
                        adddressViewModel.StreetStatus = "Select";
                        adddressViewModel.FormName = baseForm.FormName;

                        return View(adddressViewModel);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"StreetController: An exception has occured while attempting to generate Html, Exception: {e.Message}");
                        return RedirectToAction("Error", "Home", new { form = baseForm.BaseURL, });
                    };
                case "Select":
                    var behaviour = currentPage.GetNextPage(viewModel);
                    switch (behaviour.BehaviourType)
                    {
                        case EBehaviourType.GoToExternalPage:
                            return Redirect(behaviour.PageSlug);
                        case EBehaviourType.GoToPage:
                            return RedirectToAction("Index", "Home", new
                            {
                                path = behaviour.PageSlug,
                                guid,
                                form = baseForm.BaseURL
                            });
                        case EBehaviourType.SubmitForm:
                            return RedirectToAction("Submit", "Home", new
                            {
                                form = baseForm.BaseURL,
                                guid
                            });
                        default:
                            return RedirectToAction("Error", "Home", new { form = baseForm.BaseURL, });
                    }
                case "Manual":
                    break;
                default:
                    return RedirectToAction("Error", "Home", new { form = baseForm.BaseURL, });
            }

            return RedirectToAction("Error", "Home", new { form = baseForm.BaseURL, });
        }

        protected Dictionary<string, string> NormaliseFormData(Dictionary<string, string[]> formData)
        {

            var normaisedFormData = new Dictionary<string, string>();

            foreach (var item in formData)
            {
                if (item.Value.Length == 1)
                {
                    normaisedFormData.Add(item.Key, item.Value[0]);
                }
                else
                {
                    normaisedFormData.Add(item.Key, string.Join(", ", item.Value));
                }

            }

            return normaisedFormData;
        }
    }
}