namespace QouToPOWebApp.Services
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using System.Collections.Generic;
    using System.Linq;
    using QouToPOWebApp.Models.MiscModels;

    public interface IBreadcrumbService
    {
        List<Breadcrumb> GetBreadcrumbs(HttpContext context);
    }

    public class BreadcrumbService : IBreadcrumbService
    {
        public List<Breadcrumb> GetBreadcrumbs(HttpContext context)
        {
            var routeData = context.GetRouteData();
            var breadcrumbs = new List<Breadcrumb>();

            // Home breadcrumb
            breadcrumbs.Add(new Breadcrumb
            {
                Title = "Home",
                Url = "/",
                IsCurrent = false
            });

            // Generate breadcrumbs based on route
            if (routeData.Values.ContainsKey("controller"))
            {
                string controller = routeData.Values["controller"].ToString();
                string action = routeData.Values["action"].ToString();
                string id = routeData.Values.ContainsKey("id") ? routeData.Values["id"].ToString() : null;


                if (controller != "Home")
                {
                    breadcrumbs.Add(new Breadcrumb
                    {
                        Title = controller,
                        Url = $"/{controller}",
                        IsCurrent = false
                    });
                }

                if (action.ToLower() != "index")
                {
                    breadcrumbs.Add(new Breadcrumb
                    {
                        Title = action,
                        Url = $"/{controller}/{action}",
                        IsCurrent = id == null
                    });

                    if (id != null)
                    {
                        breadcrumbs.Add(new Breadcrumb
                        {
                            Title = id,
                            Url = $"/{controller}/{action}/{id}",
                            IsCurrent = true
                        });
                    }
                }
                else
                {
                    breadcrumbs.Last().IsCurrent = true;
                }
            }

            return breadcrumbs;
        }

        public List<Breadcrumb> GetBreadcrumbs(string path)
        {
            var breadcrumbs = new List<Breadcrumb>();

            breadcrumbs.Add(new Breadcrumb
            {
                Title = "PO",
                Url = "",
                IsCurrent = false
            });

            if (string.IsNullOrEmpty(path)) return breadcrumbs;

            string[] parts = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            string cumulativePath = "";

            foreach (var part in parts)
            {
                cumulativePath = Path.Combine(cumulativePath, part);
                breadcrumbs.Add(new Breadcrumb
                {
                    Title = part,
                    Url = cumulativePath
                });
            }

            breadcrumbs.Last().IsCurrent = true;

            return breadcrumbs;
        }
    }
}
