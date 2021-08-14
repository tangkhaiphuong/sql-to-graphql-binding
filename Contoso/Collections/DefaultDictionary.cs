using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Contoso.Collections
{
    /// <inheritdoc />
    public class DefaultDictionary<T, TK> : IDictionary<T, TK> where T : class
    {
        private readonly IDictionary<T, TK> _dictionary = new Dictionary<T, TK>();
        private TK _default;

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<T, TK>> GetEnumerator()
        {
            if (_default != null)
                yield return new KeyValuePair<T, TK>(null, _default);

            foreach (var item in _dictionary)
                yield return item;
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionary).GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(KeyValuePair<T, TK> item)
        {
            if (item.Key == null)
                _default = item.Value;
            else
                _dictionary.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _dictionary.Clear();
            _default = default;
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<T, TK> item)
        {
            if (item.Key == null) return Equals(_default, item.Value);
            return _dictionary.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<T, TK>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            foreach (var item in _dictionary)
            {
                array[arrayIndex++] = item;
                if (arrayIndex >= array.Length) break;
            }
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<T, TK> item)
        {
            if (item.Key == null)
            {
                if (Equals(item.Value, _default))
                {
                    _default = default;
                    return true;
                }

                return false;
            }
            return _dictionary.Remove(item);
        }

        /// <inheritdoc />
        public int Count => _dictionary.Count + (_default == null ? 0 : 1);

        /// <inheritdoc />
        public bool IsReadOnly => _dictionary.IsReadOnly;

        /// <inheritdoc />
        public void Add(T key, TK value)
        {
            if (Equals(key, default))
                _default = value;
            else
                _dictionary.Add(key, value);
        }

        /// <inheritdoc />
        public bool ContainsKey(T key)
        {
            if (Equals(key, default))
                return _default != null;
            return _dictionary.ContainsKey(key);
        }

        /// <inheritdoc />
        public bool Remove(T key)
        {
            if (Equals(key, default))
            {
                _default = default;
                return true;
            }
            return _dictionary.Remove(key);
        }

        /// <inheritdoc />
        public bool TryGetValue(T key, out TK value)
        {
            if (Equals(key, default))
            {
                value = _default;
                return true;
            }
            return _dictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc />
        public TK this[T key]
        {
            get
            {
                if (Equals(key, default))
                {
                    return _default;
                }
                return _dictionary[key];
            }
            set
            {
                if (Equals(key, default))
                {
                    _default = value;
                }
                else
                    _dictionary[key] = value;
            }
        }

        /// <inheritdoc />
        public ICollection<T> Keys
        {
            get
            {
                if (!Equals(_default, default))
                {
                    var result = new List<T> { null };
                    result.AddRange(_dictionary.Keys);
                    return result;
                }
                return _dictionary.Keys;
            }
        }

        /// <inheritdoc />
        public ICollection<TK> Values
        {
            get
            {
                if (_default == null) return _dictionary.Values;

                return Enumerable.Repeat(_default, 1).Concat(_dictionary.Values).ToList();
            }
        }
    }
}