﻿using Easy.Constant;
using Easy.Extend;
using Easy.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Easy.Data
{

    public class DataFilter
    {
        void init()
        {
            Conditions = new List<Condition>();
            ConditionGroups = new List<ConditionGroup>();
            Orders = new OrderCollection();
        }
        public DataFilter()
        {
            init();
        }
        public DataFilter(List<string> updateProperties)
        {
            init();
            this.UpdateProperties = updateProperties;
        }
        public List<string> UpdateProperties { get; set; }

        public List<ConditionGroup> ConditionGroups
        {
            get;
            set;
        }
        public List<Condition> Conditions
        {
            get;
            set;
        }
        public OrderCollection Orders
        {
            get;
            set;
        }

        #region 条件
        public DataFilter Where(Condition condition)
        {
            Conditions.Add(condition);
            return this;
        }
        public DataFilter Where(string property, OperatorType operatorType, object value)
        {
            return Where(new Condition(property, operatorType, value));
        }
        public DataFilter Where<T>(Expression<Func<T, object>> expression, OperatorType operatorType, object value)
        {
            string property = Common.GetLinqExpressionText(expression);
            DataConfigureAttribute attribute = DataConfigureAttribute.GetAttribute<T>();
            if (attribute != null && attribute.MetaData.PropertyDataConfig.ContainsKey(property))
            {
                string propertyMap = attribute.MetaData.PropertyDataConfig[property].ColumnName;
                if (!string.IsNullOrEmpty(propertyMap))
                    property = propertyMap;
            }
            return Where(new Condition(property, operatorType, value));
        }
        public DataFilter Where(string condition)
        {
            return Where(new Condition(condition, ConditionType.And));
        }
        public DataFilter Where(string condition, ConditionType conditionType)
        {
            return Where(new Condition(condition, conditionType));
        }
        public DataFilter Where(ConditionGroup conditionGroup)
        {
            this.ConditionGroups.Add(conditionGroup);
            return this;
        }
        #endregion

        #region 排序
        public DataFilter OrderBy(Order item)
        {
            if (!Orders.Exists(item.Property))
            {
                Orders.Add(item);
            }
            return this;
        }

        public DataFilter OrderBy(string property, OrderType order)
        {
            if (!Orders.Exists(property))
            {
                Orders.Add(new Order(property, order));
            }
            return this;
        }
        #endregion

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < ConditionGroups.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(ConditionGroups[i].ToString(true));
                }
                else
                {
                    builder.Append(ConditionGroups[i]);
                }
            }
            for (int i = 0; i < Conditions.Count; i++)
            {
                if (i > 0 || builder.Length > 0)
                {
                    builder.Append(Conditions[i].ToString(true));
                }
                else
                {
                    builder.Append(Conditions[i]);
                }
            }
            return builder.ToString();
        }
        public string GetOrderString()
        {
            var builder = new StringBuilder();
            foreach (var item in Orders)
            {
                if (builder.Length == 0)
                {
                    builder.Append(" ORDER BY ");
                    builder.Append(item);
                }
                else
                {
                    builder.AppendFormat(",{0} ", item);
                }
            }
            return builder.ToString();
        }
        public string GetContraryOrderString()
        {
            var builder = new StringBuilder();
            foreach (var item in Orders)
            {
                if (builder.Length == 0)
                {
                    builder.Append(" ORDER BY ");
                    builder.Append(item.ToString(true));
                }
                else
                {
                    builder.AppendFormat(",{0} ", item.ToString(true));
                }
            }
            return builder.ToString();
        }
        public List<KeyValuePair<string, object>> GetParameterValues()
        {
            var values = new List<KeyValuePair<string, object>>();
            ConditionGroups.ForEach(m => values.AddRange(m.GetKeyAndValue()));
            Conditions.Where(m => m.Value != null).Each(m =>
            {
                var keyvalue = m.GetKeyAndValue();
                if (keyvalue.Key.IsNotNullAndWhiteSpace())
                {
                    values.Add(keyvalue);
                }
            });
            return values;
        }
    }
}
