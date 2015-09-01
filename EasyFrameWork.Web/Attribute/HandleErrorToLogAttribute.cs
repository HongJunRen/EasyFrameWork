﻿using System.Web.Mvc;
namespace Easy.Web.Attribute
{
    public class HandleErrorToLogAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            Logger.Error(filterContext.Exception);
        }
    }
}