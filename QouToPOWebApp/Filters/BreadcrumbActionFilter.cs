namespace QouToPOWebApp.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using QouToPOWebApp.Services;

    public class BreadcrumbActionFilter : IActionFilter
    {
        private readonly IBreadcrumbService _breadcrumbService;

        public BreadcrumbActionFilter(IBreadcrumbService breadcrumbService)
        {
            _breadcrumbService = breadcrumbService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller controller)
            {
                var breadcrumbs = _breadcrumbService.GetBreadcrumbs(context.HttpContext);
                controller.ViewData["Breadcrumbs"] = breadcrumbs;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
    }
}
