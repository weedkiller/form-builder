using System.Threading.Tasks;
using form_builder.Enum;
using form_builder.Exceptions;
using form_builder.Helpers.Session;
using form_builder.Services.MappingService;
using form_builder.Services.PageService;
using form_builder.Services.PayService;
using form_builder.ViewModels;
using form_builder.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace form_builder.Controllers.Payment
{
    public class PaymentController : Controller
    {
        private readonly IPayService _payService;
        private readonly IPageService _pageService;
        private readonly ISessionHelper _sessionHelper;
        private readonly IMappingService _mappingService;
        private readonly ISuccessWorkflow _successWorkflow;

        public PaymentController(IPayService payService,IPageService pageService, ISessionHelper sessionHelper, IMappingService mappingService, ISuccessWorkflow successWorkflow)
        {
            _payService = payService;
            _pageService = pageService;
            _sessionHelper = sessionHelper;
            _mappingService = mappingService;
            _successWorkflow = successWorkflow;
        }

        [HttpGet]
        [Route("{form}/{path}/payment-response")]
        public async Task<IActionResult> HandlePaymentResponse(string form, string path, [FromQuery]string responseCode, [FromQuery]string callingAppTxnRef)
        {
            try
            {
                var reference = await _payService.ProcessPaymentResponse(form, responseCode, callingAppTxnRef);

                return RedirectToAction("PaymentSuccess", new
                {
                    form,
                    reference
                });
            }
            catch (PaymentFailureException)
            {
                return RedirectToAction("PaymentFailure", new
                {
                    form,
                    reference = callingAppTxnRef
                });
            }
            catch (PaymentDeclinedException)
            {
                return RedirectToAction("PaymentDeclined", new
                {
                    form,
                    reference = callingAppTxnRef
                });
            }
        }

        [HttpGet]
        [Route("{form}/payment-success")]
        public async Task<IActionResult> PaymentSuccess(string form, [FromQuery] string reference)
        {
            var result = await _successWorkflow.Process(form, reference);

            var success = new SuccessViewModel {
                Reference = reference,
                PageContent = result.HtmlContent,
                FormName = result.FormName,
                StartFormUrl = result.StartFormUrl,
                PageTitle = result.PageTitle,
                BannerTitle = result.BannerTitle,
                LeadingParagraph = result.LeadingParagraph
            };

            return View("../Home/Success", success);
        }

        [HttpGet]
        [Route("{form}/payment-failure")]
        public async Task<IActionResult> PaymentFailure(string form, [FromQuery] string reference)
        {
            var sessionGuid = _sessionHelper.GetSessionGuid();
            var data = await _mappingService.Map(sessionGuid, form);
            var url = await _payService.ProcessPayment(data, form, "payment", reference, sessionGuid);

            var paymentFailureViewModel = new PaymentFailureViewModel
            {
                FormName = form,
                PageTitle = "Failure",
                Reference = reference,
                PaymentUrl = url,
                StartFormUrl = $"https://{Request.Host}/{form}"
            };

            return View("./Failure", paymentFailureViewModel);
        }

        [HttpGet]
        [Route("{form}/payment-declined")]
        public async Task<IActionResult> PaymentDeclined(string form, [FromQuery] string reference)
        {
            var sessionGuid = _sessionHelper.GetSessionGuid();
            var data = await _mappingService.Map(sessionGuid, form);
            var url = await _payService.ProcessPayment(data, form, "payment", reference, sessionGuid);
            var paymentDeclinedViewModel = new PaymentFailureViewModel
            {
                FormName = form,
                PageTitle = "Declined",
                Reference = reference,
                PaymentUrl = url,
                StartFormUrl = $"https://{Request.Host}/{form}"
            };

            return View("./Declined", paymentDeclinedViewModel);
        }
    }
}