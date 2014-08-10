﻿using Easy.HTML.jsTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace Easy.Web.HTML
{
    public class Tree<T> where T : class
    {
        private readonly ViewContext _viewContex;
        private readonly Easy.HTML.jsTree.Tree<T> _tree;
        public Tree(ViewContext viewContex)
        {
            this._viewContex = viewContex;
            _tree = new Easy.HTML.jsTree.Tree<T>();
        }

        public Tree<T> Name(string name)
        {
            _tree.Name(name);
            return this;
        }
        public Tree<T> Source(string url)
        {
            var controller = _viewContex.Controller as System.Web.Mvc.Controller;
            if (controller != null)
                _tree.Source(controller.Url.Content(url));
            return this;
        }

        public Tree<T> Source(string action, string controller)
        {
            var controller1 = _viewContex.Controller as System.Web.Mvc.Controller;
            if (controller1 != null)
                _tree.Source(controller1.Url.Action(action, controller));
            return this;
        }

        public Tree<T> Source(string action, string controller, object routeValues)
        {
            var controller1 = _viewContex.Controller as System.Web.Mvc.Controller;
            if (controller1 != null)
                _tree.Source(controller1.Url.Action(action, controller, routeValues));
            return this;
        }
        public Tree<T> AddPlugin(string plugin)
        {
            _tree.AddPlugin(plugin);
            return this;
        }
        public Tree<T> AddContextMenuItem(ContextmenuItem item)
        {
            _tree.AddContextMenuItem(item);
            return this;
        }
        public override string ToString()
        {
            using (var writer = new HtmlTextWriter(_viewContex.Writer))
            {
                writer.Write(_tree.ToString());
            }
            return string.Empty;
        }
        public MvcHtmlString ToHtmlString()
        {
            return new MvcHtmlString(_tree.ToString());
        }
        public Tree<T> On(string events,string fun)
        {
            _tree.On(events,fun);
            return this;
        }
    }
}
