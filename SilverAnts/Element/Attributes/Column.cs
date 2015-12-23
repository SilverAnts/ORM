using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverAnts.Element
{
    /// <summary>
    /// 设定列的属性--对应数据库列名
    /// 当使用列中有空格等情况需要指定列别名,默认不使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute : Attribute
    {
        public ColumnNameAttribute()
        {

        }

        public ColumnNameAttribute(string Name)
        {
            this.Name = Name;
        }

        public string Name
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 创建-初始化时：字段的长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LengthAttribute : Attribute
    {
        public LengthAttribute()
        {

        }

        public LengthAttribute(int Size)
        {
            this.Size = Size;
        }

        public int Size
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 不进行数据库转义的列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ResultFiedAttribute : Attribute
    {

    }
}
