using System.Threading.Tasks;
using form_builder.Exceptions;
using form_builder.Helpers.Session;
using form_builder.Services.PayService;
using form_builder.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace form_builder.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPayService _payService;
        private readonly ISessionHelper _sessionHelper;

        public PaymentController(IPayService payService, ISessionHelper sessionHelper)
        {
            _payService = payService;
            _sessionHelper = sessionHelper;
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
        public IActionResult PaymentSuccess(string form, [FromQuery] string reference)
        {
            var paymentSuccessViewModel = new PaymentViewModel
            {
                Reference = reference,
                FormName = form,
                PageTitle = "Success",
                StartFormUrl = $"https://{Request.Host}/{form}"
            };

            return View("./Index", paymentSuccessViewModel);
        }

        [HttpGet]
        [Route("{form}/payment-failure")]
        public async Task<IActionResult> PaymentFailure(string form, [FromQuery] string reference)
        {
            var sessionGuid = _sessionHelper.GetSessionGuid();
            var path = "payment";
            var url = await _payService.ProcessPayment(form, path, reference, sessionGuid);
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
            var path = "payment";
            var url = await _payService.ProcessPayment(form, path, reference, sessionGuid);
            var paymentDeclinedViewModel = new PaymentFailureViewModel
            {
                FormName = form,
                PageTitle = "Declined",
                Reference = reference,
                PaymentUrl = url.ToString(),
                StartFormUrl = $"https://{Request.Host}/{form}"
            };

            return View("./Declined", paymentDeclinedViewModel);
        }

        [HttpGet]
        [Route("{form}/payment-summary")]
        public async Task<IActionResult> PaymentSummary(string form)
        {
            var paymentInfo = await _payService.GetFormPaymentInformation(form);

            var paymentSummaryViewModel = new PaymentSummaryViewModel
            {
                FormName = form,
                PageTitle = "Summary",
                Amount = paymentInfo.Settings.Amount, 
                Description = paymentInfo.Settings.Description,
                StartFormUrl = $"https://{Request.Host}/{form}"
            };

            return View("./Summary", paymentSummaryViewModel);
        }
    }
}