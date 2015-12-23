using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SilverAnts.Element
{
    /// <summary>
    /// 
    /// </summary>
    public class DymaicRow : IDictionary<string, object>
    {
        readonly DymaicFields Fields;
        object[] Values;

        public DymaicRow(DymaicFields fields, object[] values)
        {
            if (fields == null) throw new ArgumentNullException("fields");
            if (values == null) throw new ArgumentNullException("values");
            Fields = fields;
            Values = values;
        }

        public bool ContainsKey(string key)
        {
            int index = Fields[key];
            if (index < 0) return false;
            return true;
        }

        public ICollection<string> Keys
        {
            get { return this.Select(kv => kv.Key).ToArray(); }
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { return this.Select(kv => kv.Value).ToArray(); }
        }

        public void Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
            var index = this.Fields[key];
            if (index < 0)
            {
                //不存在
                value = null;
                return false;
            }
            else
            {
                //存在
                value = index < Values.Length ? Values[index] : null;
                return true;
            }
        }

        public object this[string key]
        {
            get { object val; TryGetValue(key, out val); return val; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            IDictionary<string, object> dic = this;
            dic.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Values = new object[Fields.Names.Length];
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            object value;
            return TryGetValue(item.Key, out value) && Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            foreach (var kv in this)
            {
                array[arrayIndex++] = kv; // if they didn't leave enough space; not our fault
            }
        }

        public int Count
        {
            get
            {
                return Values.Length;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            IDictionary<string, object> dic = this;
            return dic.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            var names = this.Fields.Names;
            for (var i = 0; i < names.Length; i++)
            {
                object value = i < Values.Length ? Values[i] : null;
                yield return new KeyValuePair<string, object>(names[i], value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder().Append("{DymaicRows");
            foreach (var kv in this)
            {
                var value = kv.Value;
                sb.Append(", ").Append(kv.Key);
                if (value != null)
                {
                    sb.Append(" = '").Append(kv.Value).Append('\'');
                }
                else
                {
                    sb.Append(" = NULL");
                }
            }

            return sb.Append('}').ToString();
        }
    }
}
