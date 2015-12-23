using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverAnts.Element
{
    /// <summary>
    /// 主键 -生成类型
    /// </summary>
    public enum Generator
    {
        /// <summary>
        /// GUID
        /// </summary>
        Guid,
        /// <summary>
        /// 默认-数据库自带方式
        /// </summary>
        Native,
        /// <summary>
        /// 序列-程序编写全局的序列使用
        /// </summary>
        Sequence,
        /// <summary>
        /// 程序赋值
        /// </summary>
        Assigned
    }
}
