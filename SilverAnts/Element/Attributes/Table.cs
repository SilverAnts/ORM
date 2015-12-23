using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverAnts.Element
{
    /// <summary>
    /// 表特性-记录表名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 设定表名
        /// </summary>
        /// <param name="name"></param>
        public TableNameAttribute(string name)
        {
            Value = name;
        }
        /// <summary>
        /// 表名
        /// </summary>
        public string Value { get; private set; }
    }


    /// <summary>
    /// 主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        /// 主键数据
        /// </summary>
        public Generator Generator { get; set; }
        public string Name { get; set; }

        public PrimaryKeyAttribute(string name)
        {
            this.Name = name;
            this.Generator = Generator.Native;
        }
        public PrimaryKeyAttribute(string name, Generator generator)
        {
            this.Name = name;
            this.Generator = generator;
        }
    }
}
