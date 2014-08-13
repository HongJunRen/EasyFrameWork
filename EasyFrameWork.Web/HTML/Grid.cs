﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Easy.HTML.Grid;

namespace Easy.Web.HTML
{
    public class Grid<T> : EasyGrid<T> where T : class
    {
        private readonly ViewContext _viewContex;
        public Grid(ViewContext viewContex)
        {
            this._viewContex = viewContex;
            string area = string.Empty;
            string getList = string.Empty;
            string delete = string.Empty;
            var controller = this._viewContex.Controller as System.Web.Mvc.Controller;
            getList = controller.Url.Action("GetList");
            delete = controller.Url.Action("Delete");
            base.DataSource(getList);
            base.DeleteUrl(delete);
        }
        public override EasyGrid<T> DataSource(string url)
        {
            var controller = this._viewContex.Controller as System.Web.Mvc.Controller;
            if (controller != null)
                return base.DataSource(controller.Url.Content(url));
            return this;
        }

        public EasyGrid<T> DataSource(string action, string controller)
        {
            var contro = this._viewContex.Controller as System.Web.Mvc.Controller;
            if (contro != null)
                return base.DataSource(contro.Url.Action(action, controller));
            return this;
        }
        public EasyGrid<T> DataSource(string action, string controller, object routeValues)
        {
            var contro = this._viewContex.Controller as System.Web.Mvc.Controller;
            if (contro != null)
                return base.DataSource(contro.Url.Action(action, controller, routeValues));
            return this;
        }
        public override EasyGrid<T> DeleteUrl(string url)
        {
            var controller = this._viewContex.Controller as System.Web.Mvc.Controller;
            if (controller != null)
                return base.DeleteUrl(controller.Url.Content(url));
            return this;
        }

        public EasyGrid<T> DeleteUrl(string action, string controller)
        {
            var contro = this._viewContex.Controller as System.Web.Mvc.Controller;
            if (contro != null)
                return base.DeleteUrl(contro.Url.Action(action, controller));
            return this;
        }
        public EasyGrid<T> DeleteUrl(string action, string controller, object routeValues)
        {
            var contro = this._viewContex.Controller as System.Web.Mvc.Controller;
            if (contro != null)
                return base.DeleteUrl(contro.Url.Action(action, controller, routeValues));
            return this;
        }

        public override string ToString()
        {
            using (var writer = new HtmlTextWriter(_viewContex.Writer))
            {
                writer.Write(base.ToString());
            }
            return string.Empty;
        }
        public MvcHtmlString ToHtmlString()
        {
            return new MvcHtmlString(base.ToString());
        }
    }
}
