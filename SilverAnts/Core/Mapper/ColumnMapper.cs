using System;
using System.Reflection;
using System.Linq.Expressions;
using SilverAnts.Element;

namespace SilverAnts.Core.Mapper
{
    /// <summary>
    /// 实体类（成员）->表（列）一一对应
    /// </summary>
    internal class ColumnMapper
    {
        /// <summary>
        /// 成员属性类型
        /// </summary>
        public Type ColumnType
        {
            get;
            set;
        }
        /// <summary>
        /// 成员/列名
        /// </summary>
        public string ColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// 成员属性
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get;
            set;
        }

       
        /// <summary>
        /// 
        /// </summary>
        public MethodInfo Getter { set; get; }
        /// <summary>
        /// 是否是结果列（不进行insert update)
        /// </summary>
        public bool ResultColumn
        {
            get;
            set;
        }

        public static ColumnMapper FromProperty(PropertyInfo pi)
        {
            var ci = new ColumnMapper();
            //获得列名
            //--1.使用属性名
            ci.ColumnName = pi.Name;
            //--2.使用属性特性
            var colAttrs = pi.GetCustomAttributes(typeof(ColumnNameAttribute), true);
            if (colAttrs.Length > 0)
            {
                var colattr = (ColumnNameAttribute)colAttrs[0];
                ci.ColumnName = colattr.Name == null ? pi.Name : colattr.Name;
            }
            //是否进行insert update
            ci.ResultColumn = pi.GetCustomAttributes(typeof(ResultFiedAttribute), true).Length == 0 ? false : true;
            ci.Getter = pi.DeclaringType.GetProperty(ci.ColumnName).GetGetMethod();
            ci.ColumnType = pi.PropertyType;
            ci.PropertyInfo = pi;
            return ci;
        }
    }
}
