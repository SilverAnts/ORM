using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverAnts.Utilities;
using System.Linq.Expressions;
using SilverAnts.Element;

namespace SilverAnts.Utilities
{
    /// <summary>
    /// 解析Expression
    /// </summary>
    public class ClauseExpression
    {
        /// <summary>
        /// 将表达式转义为Where条件
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        internal static Clause ToClause(Expression body)
        {
            //一元运算符有1个操作数。例如，递增(减)运算符"++、--",-取反 ！^ 非
            if (body is UnaryExpression)
            {
                return UnaryClauses((UnaryExpression)body);
            }
            else if (body is BinaryExpression)
            {
                //二元运算符有2个操作数。在SQL中体现为：比较运算符
                return BinaryClauses((BinaryExpression)body);
            }
            else if (body is MethodCallExpression)
            {
                //方法调用 即 is not null ,is null,Contains
                return MethodCallClauses((MethodCallExpression)body);
            }
            else
            {
                //其他
                return null;
            }
        }
        /// <summary>
        /// 一元
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private static Clause UnaryClauses(UnaryExpression exp)
        {
            //实在想不出一元怎么对应SQL
            switch (exp.NodeType)
            {
                case ExpressionType.Not:
                    break;

            }
            return null;
        }
        /// <summary>
        /// 二元
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private static Clause BinaryClauses(BinaryExpression exp)
        {
            switch (exp.NodeType)
            {
                //二元操作符
                case ExpressionType.Equal:
                    return Clause(exp, "=");
                case ExpressionType.GreaterThan:
                    return Clause(exp, ">");
                case ExpressionType.GreaterThanOrEqual:
                    return Clause(exp, ">=");
                case ExpressionType.LessThan:
                    return Clause(exp, "<");
                case ExpressionType.LessThanOrEqual:
                    return Clause(exp, "<=");
                case ExpressionType.NotEqual:
                    return Clause(exp, "<>");
                default:
                    throw new Exception("未实现的Where<T>(Lambda:" + exp.NodeType + ")表达式！请使用Where条件来操作！");
            }
        }

        private static Clause MethodCallClauses(MethodCallExpression ex, bool tag = true)
        {
            var member = ex.Object as MemberExpression;
            string name = member.Member.Name;
            object value
                = ex.Arguments[0].NodeType == ExpressionType.Constant
                     ? ((ConstantExpression)ex.Arguments[0]).Value
                     : System.Linq.Expressions.Expression.Lambda(ex.Arguments[0]).Compile().DynamicInvoke();
            switch (ex.Method.Name)
            {
                case "StartsWith":
                    return new Clause(name, "LIKE", "%" + value);
                case "EndsWith":
                    return new Clause(name, "LIKE", value + "%");
                case "Contains":
                    return new Clause(name, "LIKE", "%" + value + "%");
            }
            return null;
        }

        private static Clause Clause(BinaryExpression exp, string op)
        {
            var expLeft = exp.Left;
            var expRight = exp.Right;
            //成员
            string name = (expLeft as MemberExpression).Member.Name;
            object value = null;
            if (expRight.NodeType == ExpressionType.Constant)
            {
                value = (expRight as ConstantExpression).Value;
            }
            //Convert->比如：o=>o.DateTime>Convert.ToDateTime("2015-09-09")
            if (expRight.NodeType == ExpressionType.Call)
            {
                value = Expression.Lambda(expRight).Compile().DynamicInvoke();
            }
            //Convert->比如：o=>o.DateTime==DateTime.Now
            if (expRight.NodeType == ExpressionType.MemberAccess)
            {
                value = Expression.Lambda(expRight).Compile().DynamicInvoke();
            }
            return new Clause(name, op, value);
        }
    }

    /// <summary>
    /// 解析Expression
    /// </summary>
    public class SqlExpression
    {
        private string Text;
        private int Index = 0;
        public Dictionary<string, object> Argument;

        /// <summary>
        /// 成员
        /// </summary>
        /// <param name="expression"></param>
        public Sql ToSql(Expression expression)
        {
            Argument = new Dictionary<string, object>();
            Text = ResolveText(expression);
            return new Sql()
            {
                Text = Text,
                Parameters = Argument.Select(x => new KeyValuePair<string, object>(x.Key, x.Value)).ToArray()
            };
        }

        /// <summary>
        /// 转义Expression-Text
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        internal string ResolveText(Expression exp)
        {
            if (exp == null) return string.Empty;

            if (exp is UnaryExpression)
            {
                //一元运算符有1个操作数。在SQL中体现为：->只留一个反(非) ！
                return UnaryClauses(exp as UnaryExpression);
            }
            else if (exp is BinaryExpression)
            {
                //二元运算符有2个操作数。在SQL中体现为：比较运算符= <> > >= <= < ，以及从属逻辑运算符
                return BinaryClauses(exp as BinaryExpression);
            }
            else if (exp is MethodCallExpression)
            {
                //方法调用 即 is not null ,is null,Contains
                return MethodCallClauses(exp as MethodCallExpression);
            }
            throw new Exception("未实现的Where<T>(Lambda:" + exp.NodeType + ")表达式！请使用Where条件来操作！");
        }
        /// <summary>
        /// 一元
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string UnaryClauses(UnaryExpression exp)
        {
            //实在想不出一元怎么对应SQL
            switch (exp.NodeType)
            {
                case ExpressionType.Not:
                    return MethodCallClauses(exp.Operand as MethodCallExpression, false);
                default:
                    throw new Exception("未实现的Where<T>(Lambda:" + exp.NodeType + ")表达式！请使用Where条件来操作！");
            }

        }
        /// <summary>
        /// 二元
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string BinaryClauses(BinaryExpression exp)
        {
            switch (exp.NodeType)
            {
                //逻辑运算符
                //注意：此处不分别短路 && || 统一按& | 算
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return string.Format("{0} {1} {2}", ResolveText(exp.Left), "AND", ResolveText(exp.Right));
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return string.Format("{0} {1} {2}", ResolveText(exp.Left), "OR", ResolveText(exp.Right));
                //二元操作符
                case ExpressionType.Equal:
                    return ClauseText(exp, "=");
                case ExpressionType.GreaterThan:
                    return ClauseText(exp, ">");
                case ExpressionType.GreaterThanOrEqual:
                    return ClauseText(exp, ">=");
                case ExpressionType.LessThan:
                    return ClauseText(exp, "<");
                case ExpressionType.LessThanOrEqual:
                    return ClauseText(exp, "<=");
                case ExpressionType.NotEqual:
                    return ClauseText(exp, "<>");
                default:
                    throw new Exception("未实现的Where<T>(Lambda:" + exp.NodeType + ")表达式！请使用Where条件来操作！");
            }
        }
        /// <summary>
        /// 方法
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private string MethodCallClauses(MethodCallExpression ex, bool tag = true)
        {
            var member = ex.Object as MemberExpression;
            var name = member.Member.Name;
            var value
                = ex.Arguments[0].NodeType == ExpressionType.Constant
                     ? ((ConstantExpression)ex.Arguments[0]).Value
                     : System.Linq.Expressions.Expression.Lambda(ex.Arguments[0]).Compile().DynamicInvoke();
            
            string format = tag ? "{0} LIKE {1}" : "{0} NOT LIKE {1}";
            string param = "";
            switch (ex.Method.Name)
            {
                case "StartsWith":
                    param = SetArgument(name, "%" + value);
                    break;
                case "EndsWith":
                    param = SetArgument(name, value + "%");
                    break;
                case "Contains":
                    param = SetArgument(name, "%" + value + "%");
                    break;
                default:
                    throw new Exception("未实现的(Lambda:" + ex.Method.Name + ")表达式！请使用Where条件来操作！");
            }
            return string.Format(format, name, param);
        }

        private string SetArgument(string name, object value)
        {
            var param = "@" + name + Index;
            Argument[param] = value;
            Index++;
            return param;
        }

        private string ClauseText(BinaryExpression exp, string op)
        {
            var expLeft = exp.Left;
            var expRight = exp.Right;
            //成员
            string name = (expLeft as MemberExpression).Member.Name;
            object value = null;
            if (expRight.NodeType == ExpressionType.Constant)
            {
                value=(expRight as ConstantExpression).Value;
            }
            //Convert->比如：o=>o.DateTime>Convert.ToDateTime("2015-09-09")
            if (expRight.NodeType == ExpressionType.Call)
            {
                value = Expression.Lambda(expRight).Compile().DynamicInvoke();
            }
            //Convert->比如：o=>o.DateTime==DateTime.Now
            if (expRight.NodeType == ExpressionType.MemberAccess)
            {
                value = Expression.Lambda(expRight).Compile().DynamicInvoke();
            }
            string Result = "";
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                if (op == "=")
                {
                    Result = name + " IS NULL";
                }
                if (op == "<>")
                {
                    Result = name + " IS NOT NULL";
                }
            }
            else
            {
                //sql
                Result = string.Format("{0} {1} {2}", name, op, SetArgument(name, value));
            }
            return Result;
        }
    }
}
