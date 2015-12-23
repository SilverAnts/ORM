using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverAnts.Core;
using System.Linq.Expressions;
using System.Data;

namespace SilverAnts.Utilities
{
    /// <summary>
    /// 数据库访问封装 - 实体类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataAccess<T> where T : class
    {
        #region 初始化
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private static DbContext _dbContext;

        //默认
        public DataAccess()
        {
            //默认配置数据库连接名
            _dbContext = new DbContext("DataServer");
            _dbContext.Initializer(typeof(T));
        }

        public DataAccess(string dbCofig)
        {
            _dbContext = new DbContext(dbCofig);
            _dbContext.Initializer(typeof(T));

        }
        #endregion

        #region 添加、修改、删除
        /// <summary>
        /// 添加 - 单个/多个
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public object Insert(object entity)
        {
            return _dbContext.Insert<T>(entity);
        }

        /// <summary>
        /// 更新 - 单个/多个
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(object entity)
        {
            return _dbContext.Update<T>(entity);
        }
        /// <summary>
        /// 更新 - 按条件查询  
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Update(Sql sql)
        {
            return Update(sql.Text, sql.Parameters);
        }
        /// <summary>
        /// 更新 - 按条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        public int Update(string where, object paramaters)
        {
            return _dbContext.Update<T>(where, paramaters);
        }

        /// <summary>
        /// 删除 - 主键值 - 单个/多个
        /// <example>
        /// Delete(new { ID=1 }),Delete(List-ID),
        /// </example>
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int Delete(object uid)
        {
            return _dbContext.Delete<T>(uid);
        }
        /// <summary>
        /// 删除 - 所有：清空表内容
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public int Delete()
        {
            return _dbContext.Delete<T>();
        }
        /// <summary>
        /// 删除 - 按条件查询  
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Delete(Sql sql)
        {
            return Delete(sql.Text, sql.Parameters);
        }
        /// <summary>
        /// 删除 - 按条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="paramaters"></param>
        /// <returns></returns>
        public int Delete(string where, object paramaters)
        {
            return _dbContext.Delete<T>(where, paramaters);
        }

        #endregion

        #region 查询

        /// <summary>
        /// 主键查询 -Entity
        /// </summary>   
        /// <example>
        /// GetSingleEntity(new { ID=1 })
        /// </example>
        /// <param name="uid"></param>
        /// <returns></returns>
        public T GetSingleEntity(object uid)
        {
            return _dbContext.GetSingleEntity<T>(uid);
        }
        /// <summary>
        /// 按条件查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T GetSingleEntity(Sql sql)
        {
            return _dbContext.GetSingleEntity<T>(sql.Text, sql.Parameters);
        }
        /// <summary>
        /// 按条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T GetSingleEntity(string where, object parameters)
        {
            return _dbContext.GetSingleEntity<T>(where, parameters);
        }
        /// <summary>
        /// 所有数据
        /// </summary>
        /// <returns></returns>
        public List<T> GetEntityList()
        {
            return _dbContext.GetEntityList<T>("", null).ToList();
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<T> GetEntityList(Sql sql)
        {
            return GetEntityList(sql.Text, sql.Parameters);
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> GetEntityList(string where, object parameters)
        {
            return _dbContext.GetEntityList<T>(where, parameters).ToList();
        }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<T> GetEntityList(Sql sql, int pageIndex, int pageSize, out int recordCount)
        {
            return GetEntityList(sql.Text, sql.Parameters, pageIndex, pageSize, out recordCount).ToList();
        }

        /// <summary>
        ///  分页查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<T> GetEntityList(string where, object parameters, int pageIndex, int pageSize, out int recordCount)
        {
            return _dbContext.GetEntityList<T>(where, parameters, pageIndex, pageSize, out recordCount).ToList();
        }
        #endregion

        #region 其他写法
        /// <summary>
        /// where
        /// </summary>
        private static Sql sql;
        private static int _skip;
        private static int _take;

        /// <summary>
        /// Where 条件
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public DataAccess<T> Where(Expression<Func<T, bool>> exp)
        {
            sql = new SqlExpression().ToSql(exp.Body);
            sql.Text = " WHERE " + sql.Text;
            return this;
        }
        public DataAccess<T> Skip(int skip)
        {
            _skip = skip;
            return this;
        }
        public DataAccess<T> Take(int take)
        {
            _take = take;
            return this;
        }
        /// <summary>
        /// List
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            if (_take > 0)
            {
                return _dbContext.GetEntityList<T>(sql.Text, sql.Parameters, _skip, _take).ToList();
            }
            else
            {
                return GetEntityList(sql.Text, sql.Parameters);
            }
        }
        /// <summary>
        /// Fist
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            return GetSingleEntity(sql.Text, sql.Parameters);
        }
        /// <summary>
        /// FistDefault
        /// </summary>
        /// <returns></returns>
        public T FirstDefault()
        {
            T t = First();
            if (t == null)
            {
                t = Activator.CreateInstance<T>();
            }
            return t;
        }
        #endregion
    }


    /// <summary>
    /// 数据库访问封装 - Sql
    /// </summary>
    public class DataAccess
    {
        #region 初始化
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private static DbContext _dbContext;
        //默认
        public DataAccess()
        {
            //默认配置数据库连接名
            _dbContext = new DbContext("DataServer");
        }

        public DataAccess(string dbCofig)
        {
            _dbContext = new DbContext(dbCofig);
        }
        #endregion

        #region 查询
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, string values)
        {
            return _dbContext.ExecuteNonQuery(sql, values);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, string values)
        {
            return _dbContext.ExecuteReader(sql, values);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql)
        {
            return ExecuteScalar<T>(sql, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, string values)
        {
            return (T)_dbContext.ExecuteScalar(sql, values);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql)
        {
            return Query<T>(sql, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="dynamic"></typeparam>
        /// <param name="sql"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object values)
        {
            return _dbContext.Query<T>(sql, values);
        }
        #endregion
    }
}
