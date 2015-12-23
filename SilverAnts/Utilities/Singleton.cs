using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverAnts.Utilities
{
    /// <summary>
    /// 单例模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Singleton<T> where T : new()
    {
        private static readonly T instance = new T();

        public static T GetInstance()
        {
            return instance;
        }
    }
}
