using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BrainBay.Model.Responses;
using System.Collections;

namespace BrainBay.API.NewFolder
{
    public class FromDatabaseHeaderFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                if (objectResult.Value is ICachedResult singleCacheAware)
                {
                    context.HttpContext.Response.Headers["from-database"] = singleCacheAware.FromCache ? "false" : "true";
                    return;
                }

                if (objectResult.Value is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is ICachedResult cacheItem)
                        {
                            context.HttpContext.Response.Headers["from-database"] = cacheItem.FromCache ? "false" : "true";
                            return;
                        }
                    }
                }
            }
        }
    }
}
