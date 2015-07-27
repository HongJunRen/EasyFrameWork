﻿using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;

namespace Easy.Web.ControllerActivator
{
    public class EasyControllerActivator : IControllerActivator
    {

        public IController Create(System.Web.Routing.RequestContext requestContext, System.Type controllerType)
        {
            return ServiceLocator.Current.GetInstance(controllerType) as IController;
        }
    }
}