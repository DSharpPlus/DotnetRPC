using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetRPC.Entities
{
    internal class RpcCommand
    {
		/// <summary>
		/// payload command	
		/// </summary>
		[JsonProperty("cmd")]
		public string Command { get; internal set; }

		/// <summary>
		/// RPC Arguments
		/// </summary>
		[JsonProperty("args", NullValueHandling = NullValueHandling.Ignore)]
		public JObject Arguments { get; internal set; }

	    /// <summary>
	    /// unique string used once for replies from the server	
	    /// </summary>
	    [JsonProperty("nonce")]
	    public string Nonce { get; internal set; } = Guid.NewGuid().ToString();

	    /// <summary>
	    /// event data
	    /// </summary>
		[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
		public JObject Data { get; internal set; }

		/// <summary>
		/// Event name
		/// </summary>
		[JsonProperty("evt", NullValueHandling = NullValueHandling.Ignore)]
		public string Event { get; internal set; }
    }
}
