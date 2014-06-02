using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace WebRequestReflector.Models
{
	[Serializable]
	public class Headers : List<Header>
	{
		public Headers()
		{

		}

		public Headers(HttpHeaders headers)
		{
			foreach(var kvp in headers)
			{
				foreach(var v in kvp.Value)
				{
					Add(new Header(kvp.Key, v));
				}
			}
		}

		public override string ToString()
		{
			return string.Join("\n", this.Select(x => x.Value.ToString()));
		}
	}
}