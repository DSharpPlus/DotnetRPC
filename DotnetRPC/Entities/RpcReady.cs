using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetRPC.Entities
{
    public class RpcReady : AsyncEventArgs
    {
		[JsonProperty("v")]
		public int Version { get; internal set; }

		[JsonProperty("config")]
		public RpcConfig Config { get; internal set; }

		[JsonProperty("user")]
		public RpcUser User { get; internal set; }
    }

	public class RpcConfig
	{
		[JsonProperty("cdn_host")]
		public string CdnHost { get; internal set; }

		[JsonProperty("api_endpoint")]
		public string ApiEndpoint { get; internal set; }

		[JsonProperty("environment")]
		public string Environment { get; internal set; }
	}
}
