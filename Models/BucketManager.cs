using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace WebRequestReflector.Models
{
    public class BucketManager : IDisposable
    {
        private RandomNumberGenerator _rng = new RNGCryptoServiceProvider();

        public string GenerateBucketId()
        {
            byte[] randomNumbers = new byte[16];
            _rng.GetBytes(randomNumbers);
            return new String(randomNumbers.Select(x => (char)((x % 2 == 0 ? (short)'A' : (short)'a') + ((x / 2) % 26))).ToArray());
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
            }
        }

        ~BucketManager()
        {
            Dispose(false);
        }
    }
}