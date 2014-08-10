﻿using Easy.CMS.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Easy;
using Easy.CMS.Page;
using Easy.CMS.Layout;
using Easy.Constant;
using Easy.Extend;

namespace Easy.CMS.Filter
{
    public class WidgetAttribute : FilterAttribute, IActionFilter
    {

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var zones = new ZoneWidgetCollection();

            //Page
            string path = filterContext.RequestContext.HttpContext.Request.Path;
            var pageService = new PageService();
            var filter = new Data.DataFilter().Where("Url", OperatorType.Equal, path).Where("Status", OperatorType.Equal, (int)Constant.RecordStatus.Active);

            IEnumerable<PageEntity> pages = pageService.Get(filter);
            if (!pages.Any() && path == "/")
            {
                var homePage = pageService.Get(new Data.DataFilter().Where("ParentId", OperatorType.Equal, "0"));
                if (homePage.Any())
                {
                    filterContext.Result = new RedirectResult(homePage.First().Url);
                    return;
                }
            }
            if (pages.Any())
            {
                PageEntity page = pages.First();
                var layoutService = new LayoutService();
                Cache.StaticCache cache = new Cache.StaticCache();
                LayoutEntity layout = layoutService.Get(page.LayoutId);
                layout.Page = page;
                var widgetService = new WidgetService();
                IEnumerable<WidgetBase> widgets = widgetService.Get(new Data.DataFilter().Where<WidgetBase>(m => m.PageId, OperatorType.Equal, page.ID));

                widgets.Each(m =>
                {
                    var partDriver = Loader.CreateInstance<IWidgetPartDriver>(m.AssemblyName, m.ServiceTypeName);
                    WidgetPart part = partDriver.Display(partDriver.GetWidget(m.ID), filterContext.HttpContext);
                    if (zones.ContainsKey(part.ZoneId))
                    {
                        zones[part.ZoneId].Add(part);
                    }
                    else
                    {
                        var partCollection = new WidgetCollection { part };
                        zones.Add(part.ZoneId, partCollection);
                    }
                });

                layout.ZoneWidgets = zones;
                var viewResult = (filterContext.Result as ViewResult);
                if (viewResult != null)
                {
                    //viewResult.MasterName = "~/Modules/Easy.CMS.Page/Views/Shared/_Layout.cshtml";
                    viewResult.ViewData[LayoutEntity.LayoutKey] = layout;
                }
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult(404);
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }
    }

}
