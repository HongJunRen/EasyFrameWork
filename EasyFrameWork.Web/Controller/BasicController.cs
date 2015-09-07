﻿using Easy.Data;
using Easy.HTML.Grid;
using Easy.Models;
using Easy.RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Easy.Web.Extend;
using Easy.Constant;
using Easy.HTML.Tags;
using Easy.MetaData;

namespace Easy.Web.Controller
{
    /// <summary>
    /// 基本控制器，增删改查
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimarykey">主键类型</typeparam>
    /// <typeparam name="TService">Service类型</typeparam>
    public class BasicController<TEntity, TPrimarykey, TService> : System.Web.Mvc.Controller
        where TEntity : EditorEntity
        where TService : IService<TEntity>
    {
        /// <summary>
        /// 缩略图宽
        /// </summary>
        public int? ImageThumbWidth { get; set; }
        /// <summary>
        /// 缩略图高
        /// </summary>
        public int? ImageThumbHeight { get; set; }
        /// <summary>
        /// 业务Service
        /// </summary>
        public TService Service;
        public BasicController(TService service)
        {
            Service = service;
        }
        protected IImage UpLoadImage(IImage entity)
        {
            if (!string.IsNullOrEmpty(entity.ImageUrl) && string.IsNullOrEmpty(entity.ImageThumbUrl))
            {
                entity.ImageThumbUrl = entity.ImageUrl;
            }
            string filePath = Request.SaveImage();
            if (!string.IsNullOrEmpty(filePath))
            {
                entity.ImageUrl = filePath;
                string fileName = ImageUnity.SetThumb(Server.MapPath(filePath), ImageThumbWidth ?? 64, ImageThumbHeight ?? 64);
                entity.ImageThumbUrl = filePath.Replace(System.IO.Path.GetFileName(filePath), fileName);
            }
            if (string.IsNullOrEmpty(entity.ImageUrl) || string.IsNullOrEmpty(entity.ImageThumbUrl))
            {
                entity.ImageUrl = string.Empty;
                entity.ImageThumbUrl = string.Empty;
            }
            return entity;
        }

        protected object[] GetPrimaryKeys(TEntity entity)
        {
            var primaryKey = MetaData.DataConfigureAttribute.GetAttribute<TEntity>().MetaData.Primarykey;
            object[] primaryKeys = new object[primaryKey.Count];
            for (int i = 0; i < primaryKey.Count; i++)
            {
                primaryKeys[i] = Reflection.ClassAction.GetPropertyValue<TEntity>(entity, primaryKey[i]);
            }
            return primaryKeys;
        }

        public virtual ActionResult Index()
        {
            return View();
        }
        public virtual ActionResult Create()
        {
            var entity = Activator.CreateInstance<TEntity>();
            entity.Status = (int)RecordStatus.Active;
            ViewBag.Title = "添加";
            return View(entity);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Create(TEntity entity)
        {
            if (ModelState.IsValid)
            {
                var image = entity as IImage;
                if (image != null)
                {
                    UpLoadImage(image);
                }
                Service.Add(entity);
                return RedirectToAction("Index");
            }
            return View(entity);
        }
        public virtual ActionResult Edit(TPrimarykey Id)
        {
            TEntity entity = Service.Get(Id);
            return View(entity);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(TEntity entity)
        {
            if (entity.ActionType == ActionType.Delete)
            {
                Service.Delete(GetPrimaryKeys(entity));
                return RedirectToAction("Index");
            }

            ViewBag.Title = entity.Title;
            if (ModelState.IsValid)
            {
                var image = entity as IImage;
                if (image != null)
                {
                    UpLoadImage(image);
                }
                Service.Update(entity);
                return RedirectToAction("Index");
            }
            return View(entity);
        }

        [HttpPost]
        public virtual JsonResult Delete(string ids)
        {
            try
            {
                string[] id = ids.Split(',');
                List<object> listIds = new List<object>();
                string primary = DataConfigureAttribute.GetAttribute<TEntity>().MetaData.Primarykey[0];

                bool isString = typeof(TEntity).GetProperty(primary).PropertyType.Name.ToLower().Equals("string");
                foreach (string item in id)
                {
                    long test = 0;
                    if (!isString && long.TryParse(item, out test))
                    {
                        listIds.Add(test);
                    }
                    else
                    {
                        listIds.Add(item);
                    }
                }
                int result = Service.Delete(new DataFilter().Where(primary, OperatorType.In, listIds));
                if (result > 0)
                {
                    return Json(new AjaxResult { Status = AjaxStatus.Normal, Message = ids });
                }
                else
                {
                    return Json(new AjaxResult { Status = AjaxStatus.Warn, Message = "未删除任何数据！" });
                }
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = AjaxStatus.Error, Message = ex.Message });
            }
        }
        [HttpPost]
        public virtual JsonResult GetList()
        {
            GridData data = new GridData(Request.Form, (tag) =>
            {
                if (tag is DropDownListHtmlTag &&
                    (tag as DropDownListHtmlTag).SourceType == SourceType.ViewData &&
                    ViewData.ContainsKey((tag as DropDownListHtmlTag).SourceKey))
                {
                    return ViewData[(tag as DropDownListHtmlTag).SourceKey] as Dictionary<string, string>;
                }
                return null;
            });
            var filter = data.GetDataFilter<TEntity>();
            var pagin = data.GetPagination();
            return Json(data.GetJsonDataForGrid<TEntity>(Service.Get(filter, pagin), pagin));
        }
    }
}
