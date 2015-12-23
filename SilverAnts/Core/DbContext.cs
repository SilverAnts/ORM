using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection.Emit;
using SilverAnts.Element;
using SilverAnts.Core.Dialect;
using SilverAnts.Core.Mapper;

namespace SilverAnts.Core
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    internal sealed class DbContext
    {
        //数据库连接
        private static string _providerName;
        private static DbProviderFactory _dbProviderFactory;
        private static string _connectionString;
        private static DbDialect _sqlDialect = null;

        #region 初始化
        /// <summary>
        /// 初始化 -- 
        /// </summary>
        /// <param name="connectionStringName"></param>
        internal DbContext(string connectionStringName = "DataServer")
        {
            ConnectionStringSettings config = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (config == null)
            {
                Logger.WriteLog("数据库连接未配置", null, "未配置名为", connectionStringName);
                throw new Exception("数据库连接未配置");
            }
            _connectionString = config.ConnectionString;
            _providerName = config.ProviderName;
            //默认
            if (_providerName == "")
                _providerName = "System.Data.SqlClient";
            //初始化数据库--方言
            _sqlDialect = CreateDialect(_providerName);

        }
        /// <summary>
        /// 初始化数据库方言
        /// </summary>
        /// <param name="_providerName"></param>
        /// <returns></returns>
        private static DbDialect CreateDialect(string _providerName)
        {
            try
            {
                //初始化数据库驱动
                _dbProviderFactory = DbProviderFactories.GetFactory(_providerName);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("初始化失败", ex, "请检查-->", "是否将当前版本dll放入程序运行目录！");
            }
            //方言
            switch (_providerName)
            {
                case "System.Data.SqlClient":
                    return new SqlServerDialect();
                case "System.Data.SqlServerCe":
                    return new SqlServerCeDialect();
                case "MySql.Data.MySqlClient":
                    return new MySqlDialect();
                case "Oracle.DataAccess.Client":
                    return new OracleDialect();
                case "System.Data.SQLite":
                    _connectionString = _connectionString.Replace("|Path|", Environment.CurrentDirectory);
                    return new SqliteDialect();
                default:
                    return new SqlServerDialect();

            }
        }

        #endregion

        #region 据库连接

        /// <summary>
        /// 创建一个数据库连接
        /// </summary>
        /// <returns></returns>
        private IDbConnection CreateConnection()
        {
            IDbConnection connection = null;
            try
            {
                connection = _dbProviderFactory.CreateConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
            }
            catch (Exception ex)
            {
                connection.Close();
                connection.Dispose();
                Logger.WriteLog("打开数据库连接失败", ex, "open connection", _connectionString);
                throw new Exception("创建数据库连接失败，无法继续");
            }
            return connection;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 执行sql - 返回第一行-第一列值
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private object ExecuteScalar(Query query, object parameters)
        {
            //执行-使用using
            using (IDbConnection conn = CreateConnection())
            {
                using (IDbTransaction tran = conn.BeginTransaction())
                {
                    using (IDbCommand cmd = Command.PrepareCommand(conn, tran, query))
                    {
                        try
                        {
                            if (query.IsMulti)
                            {
                                var retVal = new List<object>();
                                bool isFirst = true;
                                foreach (var param in (IEnumerable)parameters)
                                {
                                    if (!isFirst)
                                    {
                                        cmd.Parameters.Clear();
                                    }
                                    else
                                    {
                                        isFirst = false;
                                    }
                                    query.ParamReader(cmd, param);
                                    retVal.Add(cmd.ExecuteScalar());
                                }
                                tran.Commit();
                                return retVal;
                            }
                            else
                            {
                                object retVal;
                                query.ParamReader(cmd, parameters);
                                retVal = cmd.ExecuteScalar();
                                tran.Commit();
                                return retVal;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("执行数据库语句失败-（ExecuteScalar）", ex, cmd.CommandText, string.Join(",", (from IDataParameter parameter in cmd.Parameters select parameter.ParameterName + "=" + parameter.Value).ToArray()));
                            return -1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行sql - 返回影响条数
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private int ExecuteNonQuery(Query query, object parameters)
        {
            var retVal = 0;
            //执行-使用using
            using (IDbConnection conn = CreateConnection())
            {
                using (IDbTransaction tran = conn.BeginTransaction())
                {
                    using (IDbCommand cmd = Command.PrepareCommand(conn, tran, query))
                    {
                        try
                        {
                            if (query.IsMulti)
                            {
                                bool isFirst = true;
                                foreach (var param in (IEnumerable)parameters)
                                {
                                    if (!isFirst)
                                    {
                                        cmd.Parameters.Clear();
                                    }
                                    else
                                    {
                                        isFirst = false;
                                    }
                                    query.ParamReader(cmd, param);
                                    retVal += cmd.ExecuteNonQuery();
                                }
                                tran.Commit();
                            }
                            else
                            {
                                query.ParamReader(cmd, parameters);
                                retVal = cmd.ExecuteNonQuery();
                                tran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("执行数据库语句失败-（ExecuteNonQuery）", ex, cmd.CommandText, string.Join(",", (from IDataParameter parameter in cmd.Parameters select parameter.ParameterName + "=" + parameter.Value).ToArray()));
                            retVal = -1;
                        }

                        return retVal;
                    }
                }
            }
        }
        /// <summary>
        /// 执行sql - 返回IDataReader
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private IDataReader ExecuteReader(Query query, object parameters)
        {
            IDataReader dr = null;
            //执行-使用using
            IDbConnection conn = CreateConnection();
            using (IDbCommand cmd = Command.PrepareCommand(conn, null, query))
            {
                try
                {
                    query.ParamReader(cmd, parameters);
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("插入数据失败", ex, cmd.CommandText, string.Join(",", (from IDataParameter parameter in cmd.Parameters select parameter.ParameterName + "=" + parameter.Value).ToArray()));
                }
                return dr;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal int ExecuteNonQuery(string sql, object parameters)
        {
            //创建一个查询
            var id = new Unique(sql, _connectionString, parameters == null ? null : parameters.GetType(), null);
            var query = Command.CreateQuery(id, _sqlDialect);
            //调用
            return ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal object ExecuteScalar(string sql, object parameters)
        {
            //创建一个查询
            var id = new Unique(sql, _connectionString, parameters == null ? null : parameters.GetType(), null);
            var query = Command.CreateQuery(id, _sqlDialect);
            //调用
            return ExecuteScalar(query, parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal IDataReader ExecuteReader(string sql, object parameters)
        {
            //创建一个查询
            var id = new Unique(sql, _connectionString, parameters == null ? null : parameters.GetType(), null);
            var query = Command.CreateQuery(id, _sqlDialect);
            //调用
            return ExecuteReader(query, parameters);
        }
        #endregion

        #region C-新增
        /// <summary>
        /// 添加 - 单个/多个 -（同一类型）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">可以是一个Poco,也可以是IEnumrate<Poco></param>
        /// <returns></returns>
        internal object Insert<TEntity>(object entity)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            return Insert(tm, entity);
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="tm"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private object Insert(TableMapper tm, object entity)
        {
            //列-成员
            var columnsMaper = CacheMapper.GetColumnsMapper(tm.PocoType);
            //列名
            var columnNames = columnsMaper.Where(c => c.ResultColumn == false).Select(o => o.ColumnName).ToList();
            //使用-数据库自增时
            if (tm.Generator == Generator.Native)
            {
                columnNames.Remove(tm.PrimaryKey);
            }
            //SQL
            var sql = _sqlDialect.GetInsertSql(tm.TableName, tm.PrimaryKey, columnNames);
            //创建一个查询
            var id = new Unique(sql, _connectionString, entity.GetType(), tm.PocoType);
            var query = Command.CreateQuery(id, _sqlDialect);
            //执行
            return ExecuteScalar(query, entity);
        }
        #endregion

        #region R-读取
        /// <summary>
        /// 根据主键获得对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        internal TEntity GetSingleEntity<TEntity>(object uid)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            var where = string.Format(" WHERE {0}={1}{0}", tm.PrimaryKey, _sqlDialect.GetParamPrefix());
            return GetSingleEntity<TEntity>(where, new KeyValuePair<string, object>(tm.PrimaryKey, uid));
        }
        /// <summary>
        /// 根据where条件获得对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal TEntity GetSingleEntity<TEntity>(string where, object parameters)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            var sql = _sqlDialect.GetSelectTopSql(tm.TableName, where, 1);
            return Query<TEntity>(sql, parameters).FirstOrDefault();
        }
        /// <summary>
        ///  根据where条件获得List
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal IEnumerable<TEntity> GetEntityList<TEntity>(string where, object parameters)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            var sql = _sqlDialect.GetSelectSql(tm.TableName, where);
            return Query<TEntity>(sql, parameters);
        }

        internal IEnumerable<TEntity> GetEntityList<TEntity>(string where, object parameters, int skip, int take)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            var sql = _sqlDialect.GetSelectSql(tm.TableName, where);
            var sqlCount = "";
            var pageSql = _sqlDialect.GetPageQuerySql(skip, take, sql, out sqlCount);
            return Query<TEntity>(pageSql, parameters);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        internal IEnumerable<TEntity> GetEntityList<TEntity>(string where, object parameters, int pageIndex, int pageSize, out int recordCount)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            var sql = _sqlDialect.GetSelectSql(tm.TableName, where);
            var sqlCount = "";
            var pageSql = _sqlDialect.GetPageQuerySql((pageIndex - 1) * pageSize, pageSize, sql, out sqlCount);
            recordCount =Convert.ToInt32(ExecuteScalar(sqlCount, parameters));
            return Query<TEntity>(pageSql, parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal IEnumerable<TEnity> Query<TEnity>(string sql, object parameters)
        {
            var type=typeof(TEnity);
            //创建一个查询
            var id = new Unique(sql, _connectionString, (parameters == null ? null : parameters.GetType()), type);
            var query = Command.CreateQuery(id, _sqlDialect);
            //DataReader
            var dr = ExecuteReader(query, parameters);
            //空值
            if (dr == null)
            {
                yield break;
            }
            using (dr)
            {
                if (query.Deserializer == null)
                {
                    query.Deserializer = EMapper.GetDeserializer(type, dr);
                }
                while (true)
                {
                    TEnity poco;
                    try
                    {
                        if (!dr.Read())
                            yield break;
                        poco = (TEnity)query.Deserializer(dr);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("查询数据失败", ex, query.Sql, "实例化失败");
                        yield break;
                    }
                    yield return poco;
                }
            }
        }

        #endregion

        #region U-修改

        /// <summary>
        /// 更新 - 单个/多个
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal int Update<TEntity>(object entity)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            string where = string.Format(" WHERE {0} ={1}{0}", tm.PrimaryKey, _sqlDialect.GetParamPrefix());
            return Update(tm, entity, where);
        }
        /// <summary>
        /// 更新 - 单个/多个
        /// </summary>
        /// <param name="tm"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal int Update<TEntity>(string where, object entity)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            return Update(tm, entity, where);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="tm"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private int Update(TableMapper tm, object entity, string where)
        {
            //列-成员
            var columnsMaper = CacheMapper.GetColumnsMapper(tm.PocoType);
            //列名
            var columnNames = columnsMaper.Where(c => c.ResultColumn == false).Select(o => o.ColumnName).ToList();
            //使用-数据库自增时
            if (tm.Generator == Generator.Native)
            {
                columnNames.Remove(tm.PrimaryKey);
            }
            //SQL
            var sql = _sqlDialect.GetUpdateSql(tm.TableName, columnNames, where);
            //创建一个查询
            var id = new Unique(sql, _connectionString, entity.GetType(), tm.PocoType);
            var query = Command.CreateQuery(id, _sqlDialect);
            //执行
            return ExecuteNonQuery(query, entity);
        }
        #endregion

        #region D-删除
        /// <summary>
        /// 删除 - 主键值 - 单个/多个
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="uid"></param>
        /// <returns></returns>
        internal int Delete<TEntity>(object uid)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            string where = string.Format(" WHERE {0} ={1}{0}", tm.PrimaryKey, _sqlDialect.GetParamPrefix());
            return Delete(tm, where, uid);
        }
        /// <summary>
        /// 删除 - 其他
        /// </summary>
        /// <param name="where"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal int Delete<TEntity>(string where, object paramaters)
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            return Delete(tm, where, paramaters);
        }
        /// <summary>
        /// 清空表内容
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        internal int Delete<TEntity>()
        {
            TableMapper tm = CacheMapper.GetTableMapper(typeof(TEntity));
            //SQL
            var sql = _sqlDialect.GetTruncateSql(tm.TableName);
            //执行
            return ExecuteNonQuery(sql, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tm"></param>
        /// <param name="where"></param>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        private int Delete(TableMapper tm, string where, object paramaters)
        {
            //SQL
            var sql = _sqlDialect.GetDeleteSql(tm.TableName, where);
            //创建一个查询
            var id = new Unique(sql, _connectionString, paramaters.GetType(), tm.PocoType);
            var query = Command.CreateQuery(id, _sqlDialect);
            //执行
            return ExecuteNonQuery(query, paramaters);
        }
        #endregion

        #region 初始化表-codefirst
        internal void Initializer(Type t)
        {

            var tm = CacheMapper.GetTableMapper(t);
            //创建表
            string sql = _sqlDialect.GetExistTableSql(tm.TableName);
            if (!(Convert.ToInt32(ExecuteScalar(sql, null)) > 0))
            {
                var columnsMaper = CacheMapper.GetColumnsMapper(t);
                var columnInfos = columnsMaper.Where(c => c.ResultColumn == false).ToList();
                try
                {
                    var createSql = _sqlDialect.GetTableCtreateSql(tm, columnInfos);
                    ExecuteNonQuery(createSql, null);
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("初始化表错误", ex, "创建表SQL错误->", "检查：类成员属性");
                }
            }
            //修改表？？
        }
        #endregion


       
    }
}