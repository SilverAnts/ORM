using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SilverAnts.Core
{
    internal class Logger
    {
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object _lock = new object();//锁
        /// <summary>
        /// 日志路径
        /// </summary>
        private static readonly string _logPath = System.Environment.CurrentDirectory + "\\Logs\\";
      
        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLog(string message, Exception ex=null, string sql="", string values="")
        {
            //目录是否存在
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
            lock (_lock)
            {
                try
                {
                    using (FileStream stream = new FileStream(_logPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", FileMode.Append, FileAccess.Write))
                    {
                        StreamWriter sw = new StreamWriter(stream);
                        sw.WriteLine(string.Format("发生时间： {0}",  DateTime.Now.ToString("dd HH:mm:ss:fff")));
                        if (ex != null)
                        {
                            if (ex.InnerException != null) { ex = ex.InnerException; }
                            sw.WriteLine(string.Format("异常内容：{0}", message + "->" + ex.Message));
                            sw.WriteLine(string.Format("详细信息：{0}", ex.StackTrace));
                        }
                        else
                        {
                            sw.WriteLine(string.Format("消息内容： {0}", message));
                        }
                        sw.WriteLine(string.Format("原始SQL-Text：{0}", sql));
                        sw.WriteLine(string.Format("原始SQL-Parameters：{0}", values));
                        sw.WriteLine("===================================================");
                        sw.Flush();
                        sw.Close();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("--写日志错误" + e.Message);
                }
            }
        }
    }
}
