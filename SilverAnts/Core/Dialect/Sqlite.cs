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
    internal class SqliteDialect : DbDialect
    {
        public SqliteDialect()
        {
            _DataTypeMapper = new ConcurrentDictionary<Type, string>();
            //由于Sqlite数据库的亲缘性，此处使用mysql的类型进行处理-
            //:tag,因3.0以后 interger 变为int64 --此处。。。在entity时使用long
            _DataTypeMapper[typeof(int)] = "INTEGER";
            _DataTypeMapper[typeof(float)] = "FLOAT";
            _DataTypeMapper[typeof(double)] = "DOUBLE";
            _DataTypeMapper[typeof(decimal)] = "DECIMAL(18,2)";
            _DataTypeMapper[typeof(bool)] = "TINYINT(1)";
            _DataTypeMapper[typeof(string)] = "VARCHAR(50)";
            _DataTypeMapper[typeof(char)] = "CHAR(50)";
            _DataTypeMapper[typeof(Guid)] = "CHAR(36)";
            //此处Datatime 因为位数的问题会转换失败
            _DataTypeMapper[typeof(DateTime)] = "DATETIME";
            _DataTypeMapper[typeof(byte[])] = "BLOB";

            _DataTypeMapper[typeof(int?)] = "INT";
            _DataTypeMapper[typeof(float?)] = "FLOAR";
            _DataTypeMapper[typeof(double?)] = "DOUBLE";
            _DataTypeMapper[typeof(decimal?)] = "DECIMAL(18,2)";
            _DataTypeMapper[typeof(bool?)] = "TINYINT(1)";
            _DataTypeMapper[typeof(DateTime?)] = "DATETIME";
            /*
            //此处INTEGER 的类型是int64，故换位INT
            _DataTypeMapper[typeof(int)] = "INT";
            _DataTypeMapper[typeof(float)] = "REAL";
            _DataTypeMapper[typeof(double)] = "REAL";
            _DataTypeMapper[typeof(decimal)] = "REAL";
            _DataTypeMapper[typeof(bool)] = "INTEGER";
            _DataTypeMapper[typeof(string)] = "TEXT";
            _DataTypeMapper[typeof(char)] = "TEXT";
            _DataTypeMapper[typeof(Guid)] = "TEXT";
            _DataTypeMapper[typeof(DateTime)] = "TEXT";
            _DataTypeMapper[typeof(byte[])] = "BLOB";

            _DataTypeMapper[typeof(int?)] = "INTEGER";
            _DataTypeMapper[typeof(float?)] = "REAL";
            _DataTypeMapper[typeof(double?)] = "REAL";
            _DataTypeMapper[typeof(decimal?)] = "REAL";
            _DataTypeMapper[typeof(bool?)] = "INTEGER";
            _DataTypeMapper[typeof(DateTime?)] = "TEXT";
             * */
        }
        public override string GetParamPrefix()
        {
            return "@";
        }

        public override string FormatSql(string sql)
        {
            return sql;
            //return sql.Replace(@"@", @"?");
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
            return string.Format("SELECT Count(*) T_Count FROM sqlite_master  WHERE type='table' AND name='{0}'", table);
        }
        public override string GetSelectTopSql(string tableName, string where, int top)
        {
            return string.Format("SELECT * FROM {0} {1} LIMIT 0,{2}", tableName, where, top);
        }
        public override string GetTableCtreateSql(TableMapper ti, List<ColumnMapper> columnInfos)
        {
            var colSqls = new List<string>();
            foreach (var ci in columnInfos)
            {
                if (ci.ColumnName == ti.PrimaryKey)
                {
                    colSqls.Add(string.Format("[{0}] {1} {2} {3} NOT NULL", ci.ColumnName, _DataTypeMapper[ci.ColumnType], "PRIMARY KEY", ti.Generator == Generator.Native ? "AUTOINCREMENT" : "")); ;
                }
                else
                {
                    colSqls.Add(string.Format("[{0}] {1}  NULL", ci.ColumnName, _DataTypeMapper[ci.ColumnType]));
                }
            }
            return string.Format("CREATE TABLE {0}\n(\n{1}\n)", ti.TableName, string.Join(",\n", colSqls));
        }

        public override string GetInsertReturnVal()
        {
            return ";\nSELECT last_insert_rowid();";
        }

        public override string GetTruncateSql(string tableName)
        {
            return string.Format("DELETE FROM {0};DELETE FROM sqlite_sequence WHERE name = '{0}'", tableName);
        }
    }
}
