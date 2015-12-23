using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverAnts.Element
{
    /// <summary>
    /// SQL - 分句
    /// </summary>
    public class Clause
    {

        public Clause()
        {

        }
        public Clause(string name, string op, object value)
        {
            this.Name = name;
            //sql
            string format = "{0} {1} @{2} ";
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {

                if (op == "=")
                {
                    this.Text = name + " IS NULL";
                }
                if (op == "<>")
                {
                    this.Text = name + " IS NOT NULL";
                }
            }
            else
            {
                this.Text = string.Format(format, name, op, name);
                //parameter
                this.Parameter = new KeyValuePair<string, object>(name, value);
            }
        }

        /// <summary>
        /// 条件列名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// sql 
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public KeyValuePair<string, object> Parameter { get; set; }

    }
}
