using Microsoft.Extensions.Caching.Memory;

namespace WebCinema.CacheService
{
    public class CacheProvider
    {
        private IMemoryCache _cache;
        private const int _seconds = 300;

        public CacheProvider(IMemoryCache cache)
        {
            this._cache = cache;
        }

        public object GetItem(string key)
        {
            _cache.TryGetValue(key, out object item);
            return item;
        }

        public T GetItem<T>(string key)
        {
            return (T)Convert.ChangeType(GetItem(key), typeof(T));
        }

        public void SetItem(object value, string key)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_seconds)
            });
        }
    }
}
