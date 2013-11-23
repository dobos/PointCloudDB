using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Engine.Schema
{
    public class SchemaObjectCollection<T> : IDictionary<string, T>, IList<T>
        where T : SchemaObject
    {
        #region Private member variables

        private List<T> list;
        private Dictionary<string, T> dict;

        #endregion
        #region Properties

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                dict.Remove(list[index].Name);
                list[index] = value;
                dict.Add(value.Name, value);
            }
        }

        public T this[string key]
        {
            get
            {
                return dict[key];
            }
            set
            {
                var index = list.IndexOf(dict[key]);
                dict[key] = value;
                list[index] = value;
            }
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ICollection<string> Keys
        {
            get { return dict.Keys; }
        }

        public ICollection<T> Values
        {
            get { return dict.Values; }
        }

        #endregion
        #region Constructors and initializers

        public SchemaObjectCollection()
        {
            list = new List<T>();
            dict = new Dictionary<string, T>(Constants.Comparer);
        }

        public SchemaObjectCollection(int capacity)
        {
            list = new List<T>(capacity);
            dict = new Dictionary<string, T>(capacity, Constants.Comparer);
        }

        #endregion

        public void Add(T item)
        {
            dict.Add(item.Name, item);
            list.Add(item);
        }

        public void Add(string key, T value)
        {
            dict.Add(key, value);
            list.Add(value);
        }

        public void Add(KeyValuePair<string, T> item)
        {
            dict.Add(item.Key, item.Value);
            list.Add(item.Value);
        }

        public void Insert(int index, T item)
        {
            dict.Add(item.Name, item);
            list.Insert(index, item);
        }

        public bool Remove(T item)
        {
            list.Remove(item);
            return dict.Remove(item.Name);
        }

        public bool Remove(string key)
        {
            list.Remove(dict[key]);
            return dict.Remove(key);
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            list.Remove(item.Value);
            return dict.Remove(item.Key);
        }

        public void RemoveAt(int index)
        {
            dict.Remove(list[index].Name);
            list.RemoveAt(index);
        }

        public void Clear()
        {
            dict.Clear();
            list.Clear();
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return dict.ContainsKey(item.Name) && dict[item.Name].Equals(item);
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return dict.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            return dict.TryGetValue(key, out value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            int i = 0;
            foreach (var v in dict)
            {
                array[i] = v;
                i++;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            return dict.GetEnumerator();
        }

    }
}
