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
    /// 
    /// </summary>
    internal class SqlServerDialect : DbDialect
    {

        public SqlServerDialect()
        {
            _DataTypeMapper = new ConcurrentDictionary<Type, string>();
            _DataTypeMapper[typeof(int)] = "INT";
            _DataTypeMapper[typeof(float)] = "FLOAT";
            _DataTypeMapper[typeof(double)] = "FLOAT";
            _DataTypeMapper[typeof(decimal)] = "DECIMAL(18,2)";
            _DataTypeMapper[typeof(bool)] = "BIT";
            _DataTypeMapper[typeof(string)] = "NVARCHAR(50)";
            _DataTypeMapper[typeof(char)] = "CHAR(50)";
            _DataTypeMapper[typeof(Guid)] = "CHAR(36)";
            _DataTypeMapper[typeof(DateTime)] = "DATETIME";
            _DataTypeMapper[typeof(byte[])] = "BINARY";

            _DataTypeMapper[typeof(int?)] = "INT";
            _DataTypeMapper[typeof(float?)] = "FLOAT";
            _DataTypeMapper[typeof(double?)] = "FLOAT";
            _DataTypeMapper[typeof(decimal?)] = "DECIMAL(18,2)";
            _DataTypeMapper[typeof(bool?)] = "BIT";
            _DataTypeMapper[typeof(DateTime?)] = "DATETIME";

        }


        public override string FormatSql(string sql)
        {
            return sql;
        }

        public override string GetParamPrefix()
        {
            return "@";
        }

        public override string GetPageQuerySql(int skip, int take, string sql, out string sqlCount)
        {
            var sqlPart = GetSqlPart(sql);
            sqlCount = sqlPart.SqlCount;
            //存在distinct
            if (rxDistinct.IsMatch(sqlPart.SqlNoSelect))
            {
                sqlPart.SqlNoSelect = "peta_inner.* FROM (SELECT " + sqlPart.SqlNoSelect + ") peta_inner";
            }
            return string.Format(" SELECT * FROM ( SELECT ROW_NUMBER() OVER({0}) rn,{1}) as data_1 Where rn>{2} AND rn<={3} ",
                       sqlPart.SqlOrderBy == "" ? "ORDER BY (SELECT NULL)" : sqlPart.SqlOrderBy, sqlPart.SqlNoSelect, skip, skip + take);
        }

        public override string GetExistTableSql(string table)
        {
            return string.Format("SELECT COUNT (*) AS T_Count FROM	sysobjects WHERE name = '{0}' AND type = 'U'", table);
        }

        public override string GetTableCtreateSql(TableMapper ti, List<ColumnMapper> columnInfos)
        {
            var colSqls = new List<string>();
            foreach (var ci in columnInfos)
            {
                if (ci.ColumnName == ti.PrimaryKey)
                {
                    colSqls.Add(string.Format("[{0}] {1} {2} {3} NOT NULL", ci.ColumnName, _DataTypeMapper[ci.ColumnType], "PRIMARY KEY", ti.Generator == Generator.Native ? "IDENTITY(1,1)" : "")); ;
                }
                else
                {
                    colSqls.Add(string.Format("[{0}] {1} NULL", ci.ColumnName, _DataTypeMapper[ci.ColumnType]));
                }
            }
            return string.Format("CREATE TABLE {0}\n(\n{1}\n)", ti.TableName, string.Join(",\n", colSqls));
        }

        public override string GetInsertSql(string tableName, string primarykey, List<string> columnNames)
        {
            var format = "INSERT INTO {0} ({1}) OUTPUT INSERTED.[{2}] VALUES ({3})";
            return string.Format(format,
                    tableName,
                    string.Join(",", columnNames),
                    primarykey,
                    string.Join(",", columnNames.Select(p => GetParamPrefix() + p)));
        }

        public override string GetTruncateSql(string tableName)
        {
            return string.Format("TRUNCATE TABLE {0}", tableName);
        }
    }
}
