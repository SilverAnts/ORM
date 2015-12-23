using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SilverAnts.Element
{
    internal class Config : ConfigurationSection
    {

        /// <summary>
        /// 默认值
        /// </summary>
        private int commandTimeout = 0;
        /// <summary>
        /// 查询超时时间
        /// </summary>
        [ConfigurationProperty("CommandTimeout")]
        public int CommandTimeout
        {
            get
            {
                if (this["CommandTimeout"] != null)
                {
                    return (int)this["CommandTimeout"];
                }
                return commandTimeout;
            }
            set { commandTimeout = value; }
        }


    }
}
