using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DoubleKeyCollection
{
    public class DoubleKeyCollection<TId, TName, T> : IEnumerable<KeyValuePair<(TId, TName), T>>
    {
        private Dictionary<LocalTuple<TId, TName>, T> _dict = new Dictionary<LocalTuple<TId, TName>, T>();
        private Dictionary<TId, Dictionary<TName, T>> _dictById = new Dictionary<TId, Dictionary<TName, T>>();
        private Dictionary<TName, Dictionary<TId, T>> _dictByName = new Dictionary<TName, Dictionary<TId, T>>();

        public int Count => _dict.Count;

        public void Add(TId id, TName name, T value)
        {
            var key = new LocalTuple<TId, TName>(id, name);
            if (_dict.ContainsKey(key))
                throw new ArgumentException("Element with the same id-name pair already exists");

            _dict[key] = value;

            Dictionary<TName, T> dictName;

            if (!_dictById.TryGetValue(id, out dictName))
            {
                dictName = new Dictionary<TName, T>();
                _dictById[id] = dictName;
            }

            dictName[name] = value;

            Dictionary<TId, T> dictId;

            if (!_dictByName.TryGetValue(name, out dictId))
            {
                dictId = new Dictionary<TId, T>();
                _dictByName[name] = dictId;
            }

            dictId[id] = value;
        }

        public List<T> GetById(TId id)
        {
            Dictionary<TName, T> dict;
            if (_dictById.TryGetValue(id, out dict))
                return dict.Values.ToList();
            return new List<T>();
        }

        public List<T> GetByName(TName name)
        {
            Dictionary<TId, T> dict;
            if (_dictByName.TryGetValue(name, out dict))
                return dict.Values.ToList();
            return new List<T>();
        }

        public T Get(TId id, TName name)
        {
            return _dict[new LocalTuple<TId, TName>(id, name)];
        }

        public void Clear()
        {
            _dictById.Clear();
            _dictByName.Clear();
            _dict.Clear();
        }

        public bool Contains(T value)
        {
            return _dict.ContainsValue(value);
        }

        public bool ContainsKey(TId id, TName name)
        {
            return _dict.ContainsKey(new LocalTuple<TId, TName>(id, name));
        }

        public bool Remove(TId id, TName name)
        {

            var key = new LocalTuple<TId, TName>(id, name);
            if (!_dict.ContainsKey(key))
                return false;

            _dict.Remove(key);
            _dictById[id].Remove(name);
            _dictByName[name].Remove(id);
            return true;
        }

        public bool TryGetValue(TId id, TName name, out T value)
        {
            return _dict.TryGetValue(new LocalTuple<TId, TName>(id, name), out value);
        }

        public IEnumerator<KeyValuePair<(TId, TName), T>> GetEnumerator()
        {
            foreach (var dict in _dictById)
                foreach (var pair in dict.Value)
                    yield return new KeyValuePair<(TId, TName), T>((dict.Key, pair.Key), pair.Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal struct LocalTuple<T1, T2>
    {
        public T1 First;
        public T2 Second;

        public LocalTuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is LocalTuple<T1, T2>))
                return false;

            var t = (LocalTuple<T1, T2>)obj;
            return First.Equals(t.First) & Second.Equals(t.Second);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }
    }
}
