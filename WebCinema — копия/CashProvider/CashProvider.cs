using Microsoft.Extensions.Caching.Memory;

namespace WebCinema.CashProvider
{
    public class CashProvider
    {
        private IMemoryCache _cache;
        private const int _seconds = 300;

        public CashProvider(IMemoryCache cache)
        {
            this._cache = cache;
        }

        public object GetItem(string key)
        {
            _cache.TryGetValue(key, out object item);
            return item;
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
