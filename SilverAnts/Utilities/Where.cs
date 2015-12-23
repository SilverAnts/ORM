using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverAnts.Element;
using System.Linq.Expressions;

namespace SilverAnts.Utilities
{
    public class Where
    {

        private string _orderBy = "";
        private string _join = "";
        private string _view = "";
        /// <summary>
        /// 参数-索引
        /// </summary>
        private int _index = 0;
        /// <summary>
        /// 条件
        /// </summary>
        private List<Clause> _clauses = new List<Clause>();

        public Where Template(string view)
        {
            if (view != "")
            {
                _view = view;
            }
            return this;
        }

        public Where And(string name, string op, object value)
        {
            return And(new Clause(name, op, value));
        }

        public Where Or(string name, string op, object value)
        {
            return Or(new Clause(name, op, value));
        }
       /// <summary>
       ///  AND -Like
       /// </summary>
       /// <param name="columnName"></param>
       /// <param name="value"></param>
       /// <returns></returns>
        public Where Like(string columnName, object value)
        {
            if (value.ToString() == "") return this;
            return And(new Clause(columnName, "LIKE", "%" + value + "%"));
        }

        /// <summary>
        /// AND
        /// </summary>
        /// <param name="clause"></param>
        public Where And(Clause clause)
        {
            string link = _clauses.Count > 0 ? " AND ({0})" : "{0}";
            return Condition(clause.Name, clause.Text, clause.Parameter.Value, link);
        }
        /// <summary>
        /// OR
        /// </summary>
        /// <param name="clause"></param>
        public Where Or(Clause clause)
        {
            string link = _clauses.Count > 0?" OR ({0}) ":"{0}";
            return Condition(clause.Name, clause.Text, clause.Parameter.Value, link);
        }

        public Where Condition(string name, string text, object value, string link="")
        {
            //添加
            _clauses.Add(new Clause()
            {
                Text = string.Format(link, text.Replace("@" + name, "@" + name + "_" + _index)),
                //
                Parameter = new KeyValuePair<string, object>(name + "_" + _index, value)
            });
            _index++;
            return this;
        }

        public Where OrderBy(string orderby)
        {
            _orderBy = string.Format(" ORDER BY {0} ", orderby);
            return this;
        }

        /// <summary>
        /// ORDER BY
        /// </summary>
        /// <param name="clause"></param>
        public Where OrderByAsc(string name)
        {
            return OrderBy(name + " ASC");
        }
        /// <summary>
        /// ORDER BY
        /// </summary>
        /// <param name="clause"></param>
        public Where OrderByDesc(string name)
        {
            return OrderBy(name + " DESC");
        }
       
        public Where LeftJoin(string join)
        {
            if (join != "")
            {
                _join = " LEFT JOIN " + join;
            }
            return this;
        }

        public Where On(string on)
        {
            if (on != "")
            {
                _join = _join + " ON " + on;
            }
            return this;
        }
        public Where RightJoin(string join)
        {
            if (join != "")
            {
                _join = " RIGHT JOIN " + join;
            }
            return this;
        }
        public Sql ToSql()
        {
            var sql = new Sql() { Text = _view + _join };
            if (_clauses.Count > 0)
            {
                sql.Text += " WHERE " + string.Join("", _clauses.Select(o => o.Text).ToArray());
                sql.Parameters = _clauses.Select(o => o.Parameter).ToArray();
            }
            sql.Text += _orderBy;
            return sql;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Sql
    {
        public string Text
        {
            get;
            set;
        }
        public object Parameters
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Where<T> : Where where T : class
    {
        /// <summary>
        /// AND
        /// </summary>
        public Where<T> And(Expression<Func<T, bool>> exp)
        {
            And(ClauseExpression.ToClause(exp.Body));
            return this;
        }
        /// <summary>
        /// OR
        /// </summary>
        public Where<T> Or(Expression<Func<T, bool>> exp)
        {
            Or(ClauseExpression.ToClause(exp.Body));
            return this;
        }
    }
}
