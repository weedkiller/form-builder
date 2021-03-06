﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using form_builder.Attributes;
using form_builder.Builders;
using form_builder.Enum;
using form_builder.Extensions;
using form_builder.Models;
using form_builder.Services.FileUploadService;
using form_builder.Services.PageService;
using form_builder.ViewModels;
using form_builder.Workflows.ActionsWorkflow;
using form_builder.Workflows.PaymentWorkflow;
using form_builder.Workflows.SubmitWorkflow;
using form_builder.Workflows.SuccessWorkflow;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace form_builder.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPageService _pageService;
        private readonly ISubmitWorkflow _submitWorkflow;
        private readonly IPaymentWorkflow _paymentWorkflow;
        private readonly IActionsWorkflow _actionsWorkflow;
        private readonly ISuccessWorkflow _successWorkflow;
        private readonly IFileUploadService _fileUploadService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(IPageService pageService,
            ISubmitWorkflow submitWorkflow,
            IPaymentWorkflow paymentWorkflow,
            IFileUploadService fileUploadService,
            IWebHostEnvironment hostingEnvironment,
            IActionsWorkflow actionsWorkflow,
            ISuccessWorkflow successWorkflow)
        {
            _pageService = pageService;
            _submitWorkflow = submitWorkflow;
            _paymentWorkflow = paymentWorkflow;
            _fileUploadService = fileUploadService;
            _hostingEnvironment = hostingEnvironment;
            _actionsWorkflow = actionsWorkflow;
            _successWorkflow = successWorkflow;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Home()
        {
            if (_hostingEnvironment.EnvironmentName.ToLower().Equals("prod"))
                return Redirect("https://www.stockport.gov.uk");

            return RedirectToAction("Index", "Error");
        }

        [HttpGet]
        [Route("{form}")]
        [Route("{form}/{path}")]
        [Route("{form}/{path}/{subPath}")]
        public async Task<IActionResult> Index(
            string form,
            string path,
            string subPath = "")
        {
            var queryParamters = Request.Query;
            var response = await _pageService.ProcessPage(form, path, subPath, queryParamters);

            if (response == null)
                return RedirectToAction("NotFound", "Error");

            if (response.ShouldRedirect)
            {
                var routeValuesDictionary = new RouteValueDictionaryBuilder()
                    .WithValue("path", response.TargetPage)
                    .WithValue("form", form)
                    .WithQueryValues(queryParamters)
                    .Build();

                return RedirectToAction("Index", routeValuesDictionary);
            }

            return View(response.ViewName, response.ViewModel);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        [Route("{form}")]
        [Route("{form}/{path}")]
        [Route("{form}/{path}/{subPath}")]
        public async Task<IActionResult> Index(
            string form,
            string path,
            Dictionary<string, string[]> formData,
            IEnumerable<CustomFormFile> fileUpload,
            string subPath = "")
        {
            var viewModel = formData.ToNormaliseDictionary(subPath);

            if (fileUpload != null && fileUpload.Any())
                viewModel = _fileUploadService.AddFiles(viewModel, fileUpload);

            var currentPageResult = await _pageService.ProcessRequest(form, path, viewModel, fileUpload, ModelState.IsValid);

            if (currentPageResult.RedirectToAction && !string.IsNullOrWhiteSpace(currentPageResult.RedirectAction))
                return RedirectToAction(currentPageResult.RedirectAction, currentPageResult.RouteValues ?? new { form, path });

            if (!currentPageResult.Page.IsValid || currentPageResult.UseGeneratedViewModel)
                return View(currentPageResult.ViewName, currentPageResult.ViewModel);

            if (currentPageResult.Page.HasPageActionsPostValues)
                await _actionsWorkflow.Process(currentPageResult.Page.PageActions.Where(_ => _.Properties.HttpActionType == EHttpActionType.Post).ToList(), null, form);

            var behaviour = _pageService.GetBehaviour(currentPageResult);

            switch (behaviour.BehaviourType)
            {
                case EBehaviourType.GoToExternalPage:
                    return Redirect(behaviour.PageSlug);

                case EBehaviourType.GoToPage:
                    return RedirectToAction("Index", new
                    {
                        path = behaviour.PageSlug
                    });

                case EBehaviourType.SubmitForm:
                    return RedirectToAction("Submit", new
                    {
                        form
                    });

                case EBehaviourType.SubmitAndPay:
                    var result = await _paymentWorkflow.Submit(form, path);
                    return Redirect(result);

                default:
                    throw new ApplicationException($"The provided behaviour type '{behaviour.BehaviourType}' is not valid");
            }
        }

        [HttpGet]
        [Route("{form}/submit")]
        public async Task<IActionResult> Submit(string form)
        {
            await _submitWorkflow.Submit(form);

            return RedirectToAction("Success", new
            {
                form
            });
        }

        [HttpGet]
        [Route("{form}/success")]
        public async Task<IActionResult> Success(string form)
        {
            var result = await _successWorkflow.Process(EBehaviourType.SubmitForm, form);

            var success = new SuccessViewModel
            {
                Reference = result.CaseReference,
                PageContent = result.HtmlContent,
                FormAnswers = result.FormAnswers,
                FormName = result.FormName,
                StartPageUrl = result.StartPageUrl,
                FeedbackPhase = result.FeedbackPhase,
                FeedbackForm = result.FeedbackFormUrl,
                PageTitle = result.PageTitle,
                BannerTitle = result.BannerTitle,
                LeadingParagraph = result.LeadingParagraph,
                DisplayBreadcrumbs = result.DisplayBreadcrumbs,
                Breadcrumbs = result.Breadcrumbs
            };

            return View(result.ViewName, success);
        }
    }
}
