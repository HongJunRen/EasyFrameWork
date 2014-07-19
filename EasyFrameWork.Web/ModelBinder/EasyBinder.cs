﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Easy.Web.ModelBinder
{
    public class EasyBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType.IsInterface || bindingContext.ModelType.IsAbstract)
            {
                return Easy.Reflection.ClassAction.GetModel(Easy.Loader.GetType(bindingContext.ModelType), controllerContext.RequestContext.HttpContext.Request.Form);
            }
            else
            {
                DefaultModelBinder binder = new DefaultModelBinder();
                return binder.BindModel(controllerContext, bindingContext);
            }
        }
    }

}