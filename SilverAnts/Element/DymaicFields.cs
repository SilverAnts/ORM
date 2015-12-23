using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverAnts.Element
{
    /// <summary>
    /// 
    /// </summary>
    public class DymaicFields
    {
        public string[] Names { get; set; }
        readonly Dictionary<string, int> _CurrentIndex;
        public int this[string fieldName]
        {
            get
            {
                if (!_CurrentIndex.ContainsKey(fieldName))
                {
                    return -1;
                }
                return _CurrentIndex[fieldName];
            }
        }

        public DymaicFields(string[] fieldNames)
        {
            if (fieldNames == null)
            {
                throw new ArgumentNullException("fieldNames");
            }
            this.Names = fieldNames;

            _CurrentIndex = new Dictionary<string, int>(fieldNames.Length, StringComparer.Ordinal);
            for (int i = fieldNames.Length - 1; i >= 0; i--)
            {
                string key = fieldNames[i];
                if (key != null) _CurrentIndex[key] = i;
            }
        }
    }
}
