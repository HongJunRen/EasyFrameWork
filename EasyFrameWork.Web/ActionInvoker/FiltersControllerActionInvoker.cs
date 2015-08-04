﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Easy.Extend;
using Easy.Web.Filter;
using Microsoft.Practices.ServiceLocation;

namespace Easy.Web.ActionInvoker
{
    public class FiltersControllerActionInvoker : ControllerActionInvoker
    {
        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filterInfos = new List<FilterInfo> { base.GetFilters(controllerContext, actionDescriptor) };
            filterInfos.AddRange(ServiceLocator.Current.GetAllInstances<IConfigureFilter>()
                .Select(m => m.Registry.GetMatched(controllerContext, actionDescriptor)));
            var filterInfo = new FilterInfo();
            filterInfos.Each(m =>
            {
                m.ActionFilters.Each(filterInfo.ActionFilters.Add);
                m.AuthorizationFilters.Each(filterInfo.AuthorizationFilters.Add);
                m.ExceptionFilters.Each(filterInfo.ExceptionFilters.Add);
                m.ResultFilters.Each(filterInfo.ResultFilters.Add);
            });
            return filterInfo;
        }
    }
}