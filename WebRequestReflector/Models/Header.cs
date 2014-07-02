using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRequestReflector.Models
{
	public class Header : IEquatable<Header>, IComparable<Header>
	{
		public string Key { get; set; }
		public string Value { get; set; }

		public Header()
		{

		}

		public Header(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public bool Equals(Header other)
		{
			return other.Key == Key && other.Value == Value;
		}

		public int CompareTo(Header other)
		{
			var i1 = Key.CompareTo(other.Key);
			return i1 != 0 ? i1 : Value.CompareTo(other.Value);
		}

		public override bool Equals(object obj)
		{
			return Equals((Header)obj);
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode() ^ Value.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", Key, Value);
		}
	}
}