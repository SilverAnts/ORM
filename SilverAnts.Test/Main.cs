using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using SilverAnts.Utilities;
using System.Threading;
using System.Diagnostics;

namespace SilverAnts.Test
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.combDataType.SelectedIndex = 0;
        }

        private void query_Click(object sender, EventArgs e)
        {
            this.gd_View.AutoGenerateColumns = false;
            this.gd_View.Rows.Clear();

            #region 测试明细
            var time = DateTime.Now;
            int maxQuery = Convert.ToInt32(this.txtLimit.Text.Trim());
            int maxInsert = Convert.ToInt32(this.txtLimitCount.Text.Trim());

            //================================================================
            int index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "删除";
            this.gd_View.Rows[index].Cells[1].Value = "删除表内容->Delete";
            Repository.Sql.ExecuteNonQuery("DELETE FROM S_Log");
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();


            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "插入-单个";
            this.gd_View.Rows[index].Cells[1].Value = "插入表内容->Insert-entity";
            Repository.SLog.Insert(new S_Log()
            {
                ActionName = "插入",
                ActionIP = "127.0.0.1",
                ActionUser = "默认用户",
                ActionTime = DateTime.Now,
                ActionDescription = "随机测试---->00000"
            });
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();
            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "清空";
            this.gd_View.Rows[index].Cells[1].Value = "清空表内容->Truancate";
            Repository.SLog.Delete();
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "插入-批量";
            this.gd_View.Rows[index].Cells[1].Value = "插入表内容->Insert-entitys";
            var logs = new List<S_Log>();
            for (int i = 0; i < maxInsert; i++)
            {
                /*
                 * 这个一条条插入 会慢非常多
                 * Repository.SLog.Insert(new S_Log()
                {
                    ActionName = "插入",
                    ActionIP = "127.0.0.1",
                    ActionUser = "默认用户",
                    ActionTime = DateTime.Now,
                    ActionDescription = "随机测试" + i
                });*/
                logs.Add(new S_Log()
                {
                    ActionName = "插入",
                    ActionIP = "127.0.0.1",
                    ActionUser = "默认用户",
                    ActionTime = DateTime.Now,
                    ActionDescription = "随机测试" + i
                });
            }
            Repository.SLog.Insert(logs);
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();


            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->查询全部";
            for (int i = 0; i <= maxQuery; i++)
            {
                var list = Repository.SLog.GetEntityList();
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();


            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "SQL模式->查询全部-S_log";
            for (int i = 0; i <= maxQuery; i++)
            {
                var list = Repository.Sql.Query<S_Log>("select * from s_log", null).ToList();
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();


            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "SQL模式->查询全部-dynamic";
            for (int i = 0; i <= maxQuery; i++)
            {
                var list = Repository.Sql.Query<dynamic>("select * from s_log", null).ToList();
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();


            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用匿名类做参数(1个)";
            for (int i = 0; i <= maxQuery; i++)
            {
                var list = Repository.SLog.GetEntityList(" WHERE ActionName like @ActionName", new { ActionName = "%插入%" });
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();


            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用匿名类做参数(2个)";
            for (int i = 0; i <= maxQuery; i++)
            {
                var list = Repository.SLog.GetEntityList(" WHERE ActionName like @ActionName AND ActionDescription like @ActionDescription", new { ActionName = "%插入%", ActionDescription = "%随机%" });
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "SQL模式->使用匿名类做参数(1个)";
            for (int i = 0; i <= maxQuery; i++)
            {
                var list = Repository.Sql.Query<S_Log>("select * from s_log WHERE ActionName LIKE @ActionName ", new { ActionName = "%插入%" }).ToList();
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "SQL模式->使用匿名类做参数(2个)";
            for (int i = 0; i <= maxQuery; i++)
            {
                var list = Repository.Sql.Query<S_Log>("select * from s_log WHERE ActionName LIKE @ActionName AND ActionDescription like @ActionDescription", new { ActionName = "%插入%", ActionDescription = "%随机%" }).ToList();
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用Where查询:KeyValue做参数(1个)";
            for (int i = 0; i <= maxQuery; i++)
            {
                var sql = new Where()
                   .Like("ActionName", "插入").ToSql();
                var list = Repository.SLog.GetEntityList(sql.Text, sql.Parameters);
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "SQL模式->使用Where查询:KeyValue做参数(1个)";
            for (int i = 0; i <= maxQuery; i++)
            {
                var sql = new Where()
                   .Template("select * from s_log")
                   .Like("ActionName", "插入").ToSql();
                var list = Repository.Sql.Query<S_Log>(sql.Text, sql.Parameters).ToList();
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();
            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用Where<T>查询:KeyValue做参数(1个)-标准写法";
            for (int i = 0; i <= maxQuery; i++)
            {
                var where = new Where<S_Log>()
                    .And(o => o.ActionName.Contains("插入")).ToSql();
                var list = Repository.SLog.GetEntityList(where.Text, where.Parameters);
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用Where<T>查询:KeyValue做参数(1个)-其他写法";
            for (int i = 0; i <= maxQuery; i++)
            {
                var list = Repository.SLog.Where(o => o.ActionName.Contains("插入")).ToList();
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用Where查询:KeyValue做参数(2个)";
            for (int i = 0; i <= maxQuery; i++)
            {
                var sql = new Where()
                 .Like("ActionName", "插入")
                 .Like("ActionDescription", "随机").ToSql();
                var list = Repository.SLog.GetEntityList(sql.Text, sql.Parameters);
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();
            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用Where<T>查询:KeyValue做参数(2个)";
            for (int i = 0; i <= maxQuery; i++)
            {
                var sql = new Where<S_Log>()
                .And(o => o.ActionName.Contains("插入"))
                .And(o => o.ActionDescription.Contains("随机"))
                .ToSql();
                var list = Repository.SLog.GetEntityList(sql.Text, sql.Parameters);
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "SQL模式->使用Where查询:KeyValue做参数(1个)-查询单列";
            for (int i = 0; i <= maxQuery; i++)
            {
                var sql = new Where()
                .Template("select ID from s_log")
                .Like("ActionName", "插入").ToSql();
                var list = Repository.Sql.Query<int>(sql.Text, sql.Parameters).ToList();
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询-分页";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用Where查询:KeyValue做参数(2个)";
            for (int i = 0; i <= maxQuery; i++)
            {
                var sql = new Where()
                 .Like("ActionName", "插入")
                 .Like("ActionDescription", "随机").ToSql();
                var count = 0;
                var list = Repository.SLog.GetEntityList(sql.Text, sql.Parameters, 1, 20, out count);
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用GetEntitylist FirstOrDefault 查询单个";
            for (int i = 0; i <= maxQuery; i++)
            {
                var ooEntity = Repository.SLog.GetEntityList(" WHERE ID = @ID", new { ID = 4 }).FirstOrDefault();
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            //================================================================
            time = DateTime.Now;
            index = this.gd_View.Rows.Add();
            this.gd_View.Rows[index].Cells[0].Value = "查询";
            this.gd_View.Rows[index].Cells[1].Value = "仓储模式->使用GetSingleEntity 查询单个 查询单个";
            for (int i = 0; i <= maxQuery; i++)
            {
                var ooEntity = Repository.SLog.GetSingleEntity(4);
            }
            this.gd_View.Rows[index].Cells[2].Value = (DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒";
            this.gd_View.Refresh();

            #endregion

        }

        private void combDataType_TextChanged(object sender, EventArgs e)
        {
            string privoder = "", connsrc = "";
            string tag = combDataType.SelectedItem.ToString().Trim();
            switch (tag)
            {
                case "MySql":
                    privoder = "MySql.Data.MySqlClient";
                    connsrc = @"Server=localhost;Charset=gb2312;Database=tile_oa_temp;User ID=root;Password=123456;pooling=true";
                    break;
                case "SQLServer":
                    privoder = "System.Data.SqlClient";
                    connsrc = @"Server=.\SQLEXPRESS;Database=tile_oa_temp;User ID=sa;Password=binghe100";
                    break;
                case "SQLite":
                    privoder = "System.Data.SQLite";
                    connsrc = @"Data Source=|Path|\data\tile_oa_temp.data;Pooling=true;FailIfMissing=false";
                    break;
            }
            this.lblPrividor.Text = privoder;
            var connection = new ConnectionStringSettings("DataServer", connsrc, privoder);
            var conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            conf.ConnectionStrings.ConnectionStrings.Remove("DataServer");
            conf.ConnectionStrings.ConnectionStrings.Add(connection);
            conf.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Repository.SLog.Delete(" 1=1 ", null);
            var time = DateTime.Now;
            var log = new S_Log()
            {
                ActionName = "插入",
                ActionIP = "127.0.0.1",
                ActionUser = "默认用户",
                ActionTime = DateTime.Now
            };
            for (int i = 0; i < 2000; i++)
            {
                log.ActionDescription = "随机测试" + i;
                Repository.SLog.Insert(log);
            }
            MessageBox.Show((DateTime.Now - time).TotalMilliseconds.ToString()+"毫秒");
        }
    }
}
