﻿using Easy.HTML;
using Easy.HTML.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Easy.Data;
using Easy.Models;

namespace Easy.MetaData
{
    public interface IDataViewMetaData
    {
        Dictionary<string, HtmlTagBase> HtmlTags { get; }
        Dictionary<string, PropertyDataInfo> PropertyDataConfig { get; }
        Dictionary<string, PropertyInfo> Properties { get; }
        List<Relation> DataRelations { get; }
        Type TargetType { get; }
        /// <summary>
        /// 表名
        /// </summary>
        string Table { get; }
        /// <summary>
        /// 表别名
        /// </summary>
        string Alias { get; }
        /// <summary>
        /// 数据库（列名）主键
        /// </summary>
        List<PrimaryKey> Primarykey { get; }
        DataFilter DataAccess(DataFilter filter);
        IUser User { get; }
    }
}
