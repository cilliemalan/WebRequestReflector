using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace WebRequestReflector.Models
{
    public class BucketEntrySummary
    {
        public int Index { get; set; }
        public string Method { get; set; }
        public long Length { get; set; }
		public DateTimeOffset DateAdded { get; set; }

        public BucketEntrySummary(int index, BucketEntry x)
        {
            Index = index;
            Method = x.Method;
            Length = x.Contents.Length;
            DateAdded = x.DateAdded;
        }
    }
}
