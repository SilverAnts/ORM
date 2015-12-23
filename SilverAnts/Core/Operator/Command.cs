using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Concurrent;
using System.Collections;
using SilverAnts.Core.Mapper;
using SilverAnts.Core.Dialect;

namespace SilverAnts.Core
{

    /// <summary>
    /// 
    /// </summary>
    internal class Query
    {
        public string Sql { get; set; }
        public int Frequency { get; set; }
        public CommandType CommandType { get; set; }
        public bool IsMulti { get; set; }
        public Func<IDataReader, object> Deserializer { get; set; }
        public Action<IDbCommand, object> ParamReader { get; set; }
        public object Result { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Command
    {
        //执行错误超时时间
        static int? _commandTimeout = 200;

        #region 创建查询
        protected internal static IDbCommand PrepareCommand(IDbConnection conn, Query query)
        {
            return PrepareCommand(conn, null, query);
        }
        protected internal static IDbCommand PrepareCommand(IDbConnection conn, IDbTransaction tran, Query query)
        {
            IDbCommand cmd = conn.CreateCommand();
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.CommandTimeout = _commandTimeout.HasValue ? 0 : _commandTimeout.Value;
            cmd.CommandType = query.CommandType;
            cmd.CommandText = query.Sql;
            return cmd;
        }
        #endregion

        /// <summary>
        /// 查询参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        protected internal static IDbDataParameter AddParam(IDbCommand cmd, string paramName, object value, DbDialect dialect)
        {
            IDbDataParameter parm = cmd.CreateParameter();
            parm.ParameterName = paramName;
            if (value == null)
            {
                parm.Value = DBNull.Value;
            }
            else
            {
                //空间存储未启用-2008 以上特性SqlGeography,SqlGeometry
                var valueType = value.GetType();
                if (valueType.IsEnum)
                {
                    parm.Value = (int)value;
                }
                //GUID
                if (valueType == typeof(Guid))
                {
                    parm.Value = value.ToString();
                    parm.DbType = DbType.String;
                    parm.Size = 40;
                }
                //
                if (valueType == typeof(string))
                {
                    parm.Value = value;
                    //长度-默认设定200
                    parm.Size = Math.Max((value as string).Length, 200);	 
                }
                else
                {
                    parm.DbType = CacheMapper.GetDbType(valueType);
                    parm.Value = value;
                }
            }
            cmd.Parameters.Add(parm);
            return parm;
        }


        #region 查询缓存
        //Sql语句-执行缓存
        static ConcurrentDictionary<Unique, Query> _queryCache = new ConcurrentDictionary<Unique, Query>();
        /// <summary>
        /// 缓存方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dialect"></param>
        /// <returns></returns>
        public static Query CreateQuery(Unique key, DbDialect dialect)
        {
            Query qc;
            if (!_queryCache.TryGetValue(key, out qc))
            {
                qc = new Query();
                //text
                qc.CommandType = CommandType.Text;
                //sql
                qc.Sql = dialect.FormatSql(key.Sql);
                if (key.ParametersType != null)
                {
                    //multi
                    qc.IsMulti = (key.ParametersType.IsGenericType && !(key.ParametersType == typeof(KeyValuePair<string, object>[]))) ? true : false;
                    //分支
                    if (key.ParametersType == typeof(KeyValuePair<string, object>))
                    {
                        qc.ParamReader = (cmd, obj) =>
                        {
                            var kvp = (KeyValuePair<string, object>)obj;
                            //-不验证
                            //if (dialect.HasParamater(key.Sql, kvp.Key))
                            //{
                                AddParam(cmd, kvp.Key, kvp.Value, dialect);
                            //}
                        };
                    }
                    else if (key.ParametersType == typeof(KeyValuePair<string, object>[]))
                    {
                        //键值对- list
                        qc.ParamReader = (cmd, obj) =>
                        {
                            var keyValues = obj as IEnumerable<KeyValuePair<string, object>>;
                            //-不验证
                            //keyValues = keyValues.Where(o => dialect.HasParamater(key.Sql, o.Key));
                            foreach (var kvp in keyValues)
                            {
                                AddParam(cmd, kvp.Key, kvp.Value, dialect);
                            }
                        };
                    }
                    else
                    {
                        //对象，则反射(此处可以更优化
                        qc.ParamReader = (cmd, obj) =>
                        {
                            var columns = CacheMapper.GetColumnsMapper(obj).Where(o => o.ResultColumn == false);
                            //-不验证
                            //columns = columns.Where(o => dialect.HasParamater(key.Sql, o.ColumnName)).ToList();
                            foreach (var c in columns)
                            {
                                AddParam(cmd, c.ColumnName, c.Getter.Invoke(obj, null), dialect);
                            }
                        };
                    }
                }
                else
                {
                    qc.ParamReader = (cmd, obj) =>
                    { };
                }
                _queryCache[key] = qc;
            }
            qc.Frequency += 1;
            return qc;
        }
        #endregion



    }


    public class Unique : IEquatable<Unique>
    {
        /// <summary>
        /// 
        /// </summary>
        public int HashCode { get; private set; }
        public string Sql { get; private set; }
        public Type Type { get; private set; }
        public string ConnString { get; private set; }
        public Type ParametersType { get; private set; }

        /// <summary>
        /// 唯一执行ID
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connectionString"></param>
        /// <param name="parametersType"></param>
        /// <param name="type"></param>
        public Unique(string sql, string connectionString, Type parametersType, Type type)
        {
            this.ConnString = connectionString;
            this.Sql = sql;
            //实体类，or key-Values
            this.Type = type;
            this.ParametersType = parametersType;
            unchecked
            {
                HashCode = 19;
                HashCode = HashCode * 23 + (type == null ? 0 : type.GetHashCode());
                HashCode = HashCode * 23 + (connectionString == null ? 0 : connectionString.GetHashCode());
                HashCode = HashCode * 23 + (sql == null ? 0 : sql.GetHashCode());
                HashCode = HashCode * 23 + (parametersType == null ? 0 : parametersType.GetHashCode());
            }
        }

        public override int GetHashCode()
        {
            return HashCode;
        }
        public bool Equals(Unique other)
        {
            return
                other != null &&
                Type == other.Type &&
                Sql == other.Sql &&
                ConnString == other.ConnString &&
                ParametersType == other.ParametersType;
        }
    }
}
