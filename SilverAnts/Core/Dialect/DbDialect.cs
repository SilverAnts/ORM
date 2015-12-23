using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using SilverAnts.Core.Mapper;
using System.Collections.Concurrent;

namespace SilverAnts.Core.Dialect
{
    /// <summary>
    /// 拆分-SQL
    /// </summary>
    internal class SqlPart
    {
        public string Sql { get; set; }
        public string SqlCount { get; set; }
        public string SqlOrderBy { get; set; }
        public string SqlNoSelect { get; set; }
    }

    /// <summary>
    /// 数据库方言
    /// 生成SQL
    /// </summary>
    internal abstract class DbDialect
    {
        #region 公共
        public ConcurrentDictionary<Type, string> _DataTypeMapper;

        #region 正则-拆分sql
        //查询列
        public readonly static Regex rxColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        //排序
        public readonly static Regex rxOrderBy = new Regex(@"\bORDER\s+BY\s+(?!.*?(?:\)|\s+)AS\s)(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*", RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        //distinct
        public readonly static Regex rxDistinct = new Regex(@"\ADISTINCT\s", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
        
        public SqlPart GetSqlPart(string sql)
        {
            //拆列
            var mc = rxColumns.Match(sql);
            //拆order by
            var mo = rxOrderBy.Match(sql);
            // Count(*)
            Group g = mc.Groups[1];
            var tempSql = mo.Success ? sql.Substring(g.Index).Replace(mo.Value, "") : sql.Substring(g.Index);
            return new SqlPart()
            {
                Sql = sql,
                //不含Select
                SqlNoSelect = tempSql,
                //
                SqlCount = "SELECT COUNT(" + g.Value.Trim() + ") AS RowsCount " + tempSql.Replace(g.Value, ""),
                // Order by
                SqlOrderBy = mo.Value
            };
        }
        
        #endregion

        #region 公共方法
        public virtual string GetInsertSql(string tableName,string primarykey, List<string> columnNames)
        {
            var format = "INSERT INTO {0} ({1}) VALUES ({2}) {3}";
            return string.Format(format,
                    tableName,
                    string.Join(",", columnNames),
                    string.Join(",", columnNames.Select(p => GetParamPrefix() + p)),
                    GetInsertReturnVal());
        }

        public virtual string GetInsertReturnVal()
        {
            return ";\nSELECT @@IDENTITY AS NewID;";
        }

        public virtual string GetUpdateSql(string tableName, IEnumerable<string> columnNames, string where)
        {
            var format = "UPDATE {0} SET {1} {2}";
            return string.Format(format,
                    tableName,
                    string.Join(",", columnNames.Select(name => name + "=" + GetParamPrefix() + name)),
                    where,
                    GetParamPrefix());
        }

        public virtual string GetDeleteSql(string tableName, string where)
        {
            var format = "DELETE FROM {0} WHERE {1}";
            return string.Format(format,
                    tableName,
                    where);
        }

        public virtual string GetSelectSql(string tableName, string where)
        {
            return string.Format("SELECT * FROM {0} {1}", tableName, where);
        }

        public virtual string GetSelectTopSql(string tableName, string where, int top)
        {
            return string.Format("SELECT TOP {2} * FROM {0} {1}", tableName, where, top);
        }
        public bool HasParamater(string sql, string paramName)
        {

            //return sql.Contains("@"+paramName);

            return Regex.IsMatch(sql, @"[?@:]" + paramName + "([^a-z0-9_]+|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
        }

        #endregion

        #endregion

        #region 重写方法
        /// <summary>
        /// 格式化--使用参数查询时，替换？ @  :
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public abstract string FormatSql(string sql);
        /// <summary>
        /// 获得参数前缀
        /// </summary>
        /// <returns></returns>
        public abstract string GetParamPrefix();
        /// <summary>
        /// 获得分页查询
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="sql"></param>
        /// <param name="sqlCount"></param>
        /// <returns></returns>
        public abstract string GetPageQuerySql(int skip, int take, string sql, out string sqlCount);
        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract string GetExistTableSql(string table);
        /// <summary>
        /// 获得表创建语句
        /// </summary>
        /// <param name="ti"></param>
        /// <param name="columnInfos"></param>
        /// <returns></returns>
        public abstract string GetTableCtreateSql(TableMapper ti, List<ColumnMapper> columnInfos);
        /// <summary>
        /// 清空表数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract string GetTruncateSql(string tableName);
        #endregion

        
    }
}
