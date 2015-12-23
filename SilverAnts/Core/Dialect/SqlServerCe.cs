using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverAnts.Core.Mapper;

namespace SilverAnts.Core.Dialect
{
    /// <summary>
    /// 
    /// </summary>
    internal class SqlServerCeDialect : DbDialect
    {
        public override string GetParamPrefix()
        {
            return "?";
        }

        public override string FormatSql(string sql)
        {
            return sql;
        }

        public override string GetPageQuerySql(int skip, int take, string sql, out string sqlCount)
        {
            throw new NotImplementedException();
        }

        public override string GetExistTableSql(string table)
        {
            throw new NotImplementedException();
        }

        public override string GetTableCtreateSql(TableMapper ti, List<ColumnMapper> columnInfos)
        {
            throw new NotImplementedException();
        }

        public override string GetTruncateSql(string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
