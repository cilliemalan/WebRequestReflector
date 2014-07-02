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
			Func<byte, char> MakeVowel = b =>
			{
				switch (b % 6)
				{
					case 0: return 'a';
					case 1: return 'e';
					case 2: return 'o';
					case 3: return 'u';
					case 4: return 'i';
					case 5: return 'y';
				};
				return 'A';
			};
			Func<byte, char> MakeConsonant = b =>
			{
				switch (b % 20)
				{
					case 0: return 'b';
					case 1: return 'c';
					case 2: return 'd';
					case 3: return 'f';
					case 4: return 'g';
					case 5: return 'h';
					case 6: return 'j';
					case 7: return 'k';
					case 8: return 'l';
					case 9: return 'm';
					case 10: return 'n';
					case 11: return 'p';
					case 12: return 'q';
					case 13: return 'r';
					case 14: return 's';
					case 15: return 't';
					case 16: return 'v';
					case 17: return 'w';
					case 18: return 'x';
					case 19: return 'z';
				};
				return 'A';
			};

            byte[] randomNumbers = new byte[16];
            _rng.GetBytes(randomNumbers);
			return new String(randomNumbers.Select((x,i) => i % 2 == 0 ? MakeConsonant(x) : MakeVowel(x)).ToArray());
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
                if (bucket == null || bucket.Expires <= DateTime.UtcNow)
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