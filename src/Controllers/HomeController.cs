﻿using form_builder.Enum;
using form_builder.Extensions;
using form_builder.Models;
using form_builder.Services.FileUploadService;
using form_builder.Services.PageService;
using form_builder.Workflows;
using Microsoft.EntityFrameworkCore.Internal;
using form_builder.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace form_builder.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPageService _pageService;
        private readonly ISubmitWorkflow _submitWorkflow;
        private readonly IPaymentWorkflow _paymentWorkflow;
        private readonly IActionsWorkflow _actionsWorkflow;
        private readonly IFileUploadService _fileUploadService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IPageService pageService,
            ISubmitWorkflow submitWorkflow,
            IPaymentWorkflow paymentWorkflow,
            IFileUploadService fileUploadService,
            IHostingEnvironment hostingEnvironment, 
            IActionsWorkflow actionsWorkflow)
        {
            _pageService = pageService;
            _submitWorkflow = submitWorkflow;
            _paymentWorkflow = paymentWorkflow;
            _fileUploadService = fileUploadService;
            _hostingEnvironment = hostingEnvironment;
            _actionsWorkflow = actionsWorkflow;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Home()
        {
            if(_hostingEnvironment.EnvironmentName.ToLower() == "prod"){
                return Redirect("https://www.stockport.gov.uk");
            }

            return View("../Error/Index");
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
            var response = await _pageService.ProcessPage(form, path, subPath);
            if (response.ShouldRedirect)
                return RedirectToAction("Index", new
                {
                    path = response.TargetPage,
                    form
                });

            return View(response.ViewName, response.ViewModel);
        }

        [HttpPost]
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

            if(fileUpload != null && fileUpload.Any())
                viewModel = _fileUploadService.AddFiles(viewModel, fileUpload);

            var currentPageResult = await _pageService.ProcessRequest(form, path, viewModel, fileUpload);

            if (currentPageResult.RedirectToAction && !string.IsNullOrWhiteSpace(currentPageResult.RedirectAction))
                return RedirectToAction(currentPageResult.RedirectAction, currentPageResult.RouteValues != null ? currentPageResult.RouteValues : new { form, path });

            if (!currentPageResult.Page.IsValid || currentPageResult.UseGeneratedViewModel)
                return View(currentPageResult.ViewName, currentPageResult.ViewModel);

            if (currentPageResult.Page.HasPageActions)
                await _actionsWorkflow.Process(currentPageResult.Page);

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
            var result = await _submitWorkflow.Submit(form);

            TempData["reference"] = result;
            return RedirectToAction("Success", new
            {
                form
            });
        }

        [HttpGet]
        [Route("{form}/success")]
        public async Task<IActionResult> Success(string form)
        {
            var result = await _pageService.FinalisePageJourney(form, EBehaviourType.SubmitForm);
            
            var success = new SuccessViewModel {
                Reference = (string)TempData["reference"],
                PageContent = result.HtmlContent,
                FormAnswers = result.FormAnswers,
                FormName = result.FormName,
                StartFormUrl = result.StartFormUrl,
                FeedbackPhase = result.FeedbackPhase,
                FeedbackFormUrl = result.FeedbackFormUrl,
                PageTitle = result.PageTitle,
                BannerTitle = result.BannerTitle,
                LeadingParagraph = result.LeadingParagraph
            };

            return View(result.ViewName, success);
        }
    }
}
