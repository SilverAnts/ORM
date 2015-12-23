using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverAnts.Utilities;
using SilverAnts.Element;

namespace SilverAnts.Test
{
    /// <summary>
    /// 仓储
    /// </summary>
    public static class Repository
    {
        #region 表访问类
        public static DataAccess<S_Log> SLog { get { return Singleton<DataAccess<S_Log>>.GetInstance(); } }
        #endregion

        #region SQL
        public static DataAccess Sql { get { return Singleton<DataAccess>.GetInstance(); } }

        #endregion
    }

    [Serializable]
    [TableName("S_Log")]
    [PrimaryKey("ID", Generator.Native)]
    public class S_Log
    {
        #region S_Log 成员
        public int ID { get; set; }
        public string ActionName { get; set; }
        public string ActionDescription { get; set; }
        public string ActionUser { get; set; }
        public DateTime ActionTime { get; set; }
        public string ActionIP { get; set; }

        #endregion
    }

   
}
