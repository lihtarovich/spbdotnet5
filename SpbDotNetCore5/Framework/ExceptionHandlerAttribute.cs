using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace SpbDotNetCore5
{
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            HttpStatusCode errorCode = GetStatusCode(context.Exception.GetType());
            context.Result = new ObjectResult(context.Exception.Message)
            {
                StatusCode = (Int32)errorCode
            };
            base.OnException(context);
        }

        private HttpStatusCode GetStatusCode(Type exceptionType)
        {
            FrameworkExceptions tryParseResult;
            if (!Enum.TryParse<FrameworkExceptions>(exceptionType.Name, out tryParseResult))
            {
                return HttpStatusCode.InternalServerError;
            }
            switch (tryParseResult)
            {
                case FrameworkExceptions.UnauthorizedAccessException:
                case FrameworkExceptions.AuthenticationException:
                    return HttpStatusCode.Unauthorized;

                case FrameworkExceptions.NotImplementedException:
                    return HttpStatusCode.NotImplemented;

                case FrameworkExceptions.NotSupportedException:
                case FrameworkExceptions.InvalidOperationException:
                    return HttpStatusCode.InternalServerError;

                default:
                    return HttpStatusCode.InternalServerError;
            }
        }
    }
}