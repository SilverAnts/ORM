using System;
using SilverAnts.Element;


namespace SilverAnts.Core.Mapper
{
    /// <summary>
    /// 实体类->表 一一对应
    /// </summary>
    internal class TableMapper
    {
        /// <summary>
        /// Poco类 类型
        /// </summary>
        public Type PocoType
        {
            get;
            set;
        }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get;
            set;
        }

        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey
        {
            get;
            set;
        }

        /// <summary>
        /// 主键-生成方式
        /// </summary>
        public Generator Generator
        {
            get;
            set;
        }
        /// <summary>
        /// 从实体类获得表对象
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static TableMapper FromPoco(Type t)
        {
            var ti = new TableMapper();
            ti.PocoType = t;
            //获得表名
            var a = t.GetCustomAttributes(typeof(TableNameAttribute), true);
            ti.TableName = a.Length == 0 ? t.Name : (a[0] as TableNameAttribute).Value;

            //获得主键
            a = t.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
            ti.PrimaryKey = a.Length == 0 ? "ID" : (a[0] as PrimaryKeyAttribute).Name;
            //主键索引方式
            ti.Generator = a.Length == 0 ? Generator.Native : (a[0] as PrimaryKeyAttribute).Generator;
            return ti;
        }
    }

}
