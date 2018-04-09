using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetRPC.Entities
{
    internal class RpcHandshake
    {
		[JsonProperty("v")]
		public int Version = 1;

		[JsonProperty("client_id")]
		public string ClientId = "176019685471551488";
	}
}
