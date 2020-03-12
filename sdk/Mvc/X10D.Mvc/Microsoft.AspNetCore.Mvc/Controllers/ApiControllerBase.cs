using Microsoft.AspNetCore.Http;
using System;
using X10D.Infrastructure;
using X10D.Mvc.Formats;
using X10D.Mvc.Services;

namespace Microsoft.AspNetCore.Mvc
{
    [ApiRoute]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        [FromRoute]
        public string Format { get; set; } = Constants.DefaultFormat;

        protected IActivator Activator { get; }
        public ApiControllerBase(IActivator activator)
        {
            Activator = activator;
        }

        protected new IActionResult Ok(object result = null, string description = null)
        {
            return Response(true, result, StatusCodes.Status200OK, description);
        }

        protected new IActionResult Unauthorized(object result = null, string description = null)
        {
            return Response(false, result, StatusCodes.Status401Unauthorized, description);
        }

        protected new IActionResult Forbid(object result = null, string description = null)
        {
            return Response(false, result, StatusCodes.Status403Forbidden, description);
        }

        protected new IActionResult NotFound(object result = null, string description = null)
        {
            return Response(false, result, StatusCodes.Status404NotFound, description);
        }

        protected IActionResult Response(bool ok, object result = null, int? status_code = null, string description = null)
        {
            var formatType = Activator.GetServiceOrCreateInstance<IApiResponseFactory>().GetFormatType(Format);

            if (formatType == null)
            {
                return base.NotFound("The expected response format is not supported");
            }

            var response = Activator.CreateEmpty<IApiResponse>(formatType);

            var okProp = formatType.GetProperty(nameof(IApiResponse.ok));
            okProp.SetValue(response, ok);

            var resultProp = formatType.GetProperty(nameof(IApiResponse.result));
            resultProp.SetValue(response, result);

            var statusCodeProp = formatType.GetProperty(nameof(IApiResponse.status_code));
            statusCodeProp.SetValue(response, ok ? StatusCodes.Status200OK : status_code ?? StatusCodes.Status400BadRequest);

            var descriptionProp = formatType.GetProperty(nameof(IApiResponse.description));
            descriptionProp.SetValue(response, description);

            var requestTimeProp = formatType.GetProperty(nameof(IApiResponse.request_time));
            if (HttpContext.Items.ContainsKey(Constants.RequestStartedOn) &&
                HttpContext.Items[Constants.RequestStartedOn] is DateTime stamp)
            {
                requestTimeProp.SetValue(response, (DateTime.Now - stamp).TotalMilliseconds);
            }

            return StatusCode(response.status_code, response.Pack().ToString());
        }
    }
}
