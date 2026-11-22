using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyNhaThuoc.API.Services;

public sealed class BusinessRuleExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not BusinessRuleException exception)
            return;

        context.Result = new BadRequestObjectResult(new { message = exception.Message });
        context.ExceptionHandled = true;
    }
}
