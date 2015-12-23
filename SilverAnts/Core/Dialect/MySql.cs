using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SilverAnts.Element;
using SilverAnts.Core.Mapper;

namespace SilverAnts.Core.Dialect
{
    /// <summary>
    /// mysql 数据库
    /// </summary>
    internal class MySqlDialect : DbDialect
    {
        public MySqlDialect()
        {
            _DataTypeMapper = new ConcurrentDictionary<Type, string>();
            _DataTypeMapper[typeof(int)] = "INT";
            _DataTypeMapper[typeof(float)] = "FLOAT";
            _DataTypeMapper[typeof(double)] = "DOUBLE";
            _DataTypeMapper[typeof(decimal)] = "DECIMAL(18,2)";
            _DataTypeMapper[typeof(bool)] = "TINYINT(1)";
            _DataTypeMapper[typeof(string)] = "VARCHAR(50)";
            _DataTypeMapper[typeof(char)] = "CHAR(50)";
            _DataTypeMapper[typeof(Guid)] = "CHAR(36)";
            _DataTypeMapper[typeof(DateTime)] = "DATETIME";
            _DataTypeMapper[typeof(byte[])] = "BLOB";

            _DataTypeMapper[typeof(int?)] = "INT";
            _DataTypeMapper[typeof(float?)] = "FLOAR";
            _DataTypeMapper[typeof(double?)] = "DOUBLE";
            _DataTypeMapper[typeof(decimal?)] = "DECIMAL(18,2)";
            _DataTypeMapper[typeof(bool?)] = "TINYINT(1)";
            _DataTypeMapper[typeof(DateTime?)] = "DATETIME";
        }
        public override string GetParamPrefix()
        {
            return "?";
        }

        public override string FormatSql(string sql)
        {
            //return sql;
            return sql.Replace(@"@", @"?");
        }
        public override string GetSelectTopSql(string tableName, string where, int top)
        {
            return string.Format("SELECT * FROM {0} {1} LIMIT 0,{2}", tableName, where, top);
        }
        public override string GetPageQuerySql(int skip, int take, string sql, out string sqlCount)
        {
            var sqlPart = GetSqlPart(sql);
            sqlCount = sqlPart.SqlCount;
            return string.Format("{0} LIMIT {1} OFFSET {2}",
                  sqlPart.Sql, skip + take, skip);
        }

        public override string GetExistTableSql(string table)
        {
            return string.Format("SELECT Count(*) T_Count FROM information_schema.TABLES WHERE table_name ='{0}'", table);
        }

        public override string GetTableCtreateSql(TableMapper ti, List<ColumnMapper> columnInfos)
        {
            var colSqls = new List<string>();
            foreach (var ci in columnInfos)
            {
                if (ci.ColumnName == ti.PrimaryKey)
                {
                    colSqls.Add(string.Format("`{0}` {1} {2} {3} NOT NULL", ci.ColumnName, _DataTypeMapper[ci.ColumnType], "PRIMARY KEY", ti.Generator == Generator.Native ? "AUTO_INCREMENT" : "")); ;
                }
                else
                {
                    colSqls.Add(string.Format("`{0}` {1}  NULL", ci.ColumnName, _DataTypeMapper[ci.ColumnType]));
                }
            }
            return string.Format("CREATE TABLE {0}\n(\n{1}\n)", ti.TableName, string.Join(",\n", colSqls));
        }

        public override string GetInsertReturnVal()
        {
            return ";\nSELECT LAST_INSERT_ID() AS NewID;";
        }

        public override string GetTruncateSql(string tableName)
        {
            return string.Format("TRUNCATE TABLE {0}", tableName);
        }
    }
}
