using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Web;

namespace WebRequestReflector.Models
{
    public class BucketManager : IDisposable
    {
        private RandomNumberGenerator _rng = new RNGCryptoServiceProvider();
        private ObjectCache _cache = new MemoryCache("bucketCache");

        public string GenerateBucketId()
        {
            byte[] randomNumbers = new byte[16];
            _rng.GetBytes(randomNumbers);
            return new String(randomNumbers.Select(x => (char)((x % 2 == 0 ? (short)'A' : (short)'a') + ((x / 2) % 26))).ToArray());
        }

        public Bucket Create()
        {
            string key =GenerateBucketId();
            Bucket newBucket = new Bucket(key);

            CacheItem cacheItem = new CacheItem(key, newBucket);
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = newBucket.Expires
            };

            _cache.Set(cacheItem, policy);

            return newBucket;
        }

        public Bucket Get(string id)
        {
            CacheItem item = _cache.GetCacheItem(id);
            if (item == null)
            {
                return null;
            }
            else
            {
                Bucket bucket = item.Value as Bucket;
                if (bucket == null || bucket.Expires <= DateTimeOffset.Now)
                {
                    _cache.Remove(id);
                    return null;
                }
                else
                {
                    return bucket;
                }
            }
        }

        public void Update(Bucket bucket)
        {
            Bucket b = Get(bucket.Id);
            if (b == null) throw new InvalidOperationException("The bucket does not exist or has expired.");
            else if(object.ReferenceEquals(b, bucket))
            {
                return;
            }
            else
            {
                CacheItem cacheItem = new CacheItem(b.Id, bucket);
                CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = b.Expires };
                _cache.Set(cacheItem, policy);
            }
        }

        public void Delete(string id)
        {
            _cache.Remove(id);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (_rng != null)
                {
                    _rng.Dispose();
                    _rng = null;
                }

                if (_cache != null)
                {
                    foreach (var item in _cache)
                    {
                        _cache.Remove(item.Key);
                    }

                    _cache = null;
                }
            }
        }

        ~BucketManager()
        {
            Dispose(false);
        }
    }
}