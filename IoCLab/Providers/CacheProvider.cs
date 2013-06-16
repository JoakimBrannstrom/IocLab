using System;
using System.Collections.Generic;

namespace IoCLab.Providers
{
    public interface ICacheProvider
    {
        void Delete(string key);
        object Get(string key);
        bool HasKey(string key);
        void Store(string key, object data);
    }

    public class CacheItem
    {
        public DateTime ValidUntil;
        public object Data;
    }

    public class InMemoryCacheProvider : ICacheProvider
    {
        private static IDictionary<string, CacheItem> _cachedData;

        private static readonly TimeSpan CacheTime = new TimeSpan(0, 0, 1, 0);

        public InMemoryCacheProvider()
            : this(new Dictionary<string, CacheItem>())
        {
        }

        public InMemoryCacheProvider(IDictionary<string, CacheItem> dictionary)
        {
            _cachedData = dictionary;
        }

        public void Delete(string key)
        {
            _cachedData.Remove(key);
        }

        public object Get(string key)
        {
            if (HasKey(key))
                return _cachedData[key].Data;

            return null;
        }

        public bool HasKey(string key)
        {
            if (!_cachedData.ContainsKey(key))
                return false;

            var item = _cachedData[key];
            if (item.ValidUntil > DateTime.UtcNow)
                return true;

            _cachedData.Remove(key);
            return false;
        }

        public void Store(string key, object data)
        {
            _cachedData[key] = new CacheItem() {Data = data, ValidUntil = DateTime.UtcNow.Add(CacheTime)};
        }
    }
}
