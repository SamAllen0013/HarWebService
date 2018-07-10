using System.Runtime.Caching;

namespace SamAllen_Rigor_Challenge.Helpers
{
    public class CacheHelper
    {
        public CacheHelper()
        {
            cachePlace = MemoryCache.Default;
            cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.Priority = CacheItemPriority.NotRemovable;
        }

        public void AddCache(object objectToCache, string cacheKeyName)
        {         
            CacheItem cacheItem = new CacheItem(cacheKeyName, objectToCache);
            cachePlace.Add(cacheItem, cacheItemPolicy);
        }

        public void UpdateCache(object objectToCache, string cacheKeyName)
        {
            CacheItem cacheItem = new CacheItem(cacheKeyName, objectToCache);
            cachePlace.Set(cacheItem, cacheItemPolicy);
        }

        public void RemoveObjectFromCache(string cacheKeyName)
        {
            cachePlace.Remove(cacheKeyName);
        }

        public object GetObjectFromCache(string cacheKeyName)
        {
            return cachePlace.Get(cacheKeyName);
        }

        public bool CheckIfItemExists(string cacheKeyName)
        {
            bool itemExists = false;
            if (cachePlace[cacheKeyName] != null)
            {
                itemExists = true;
            }
            return itemExists;
        }

        private CacheItemPolicy cacheItemPolicy { get; set; }
        private ObjectCache cachePlace { get; set; }
    }
}