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
		/// RPC Command
		/// </summary>
		[JsonProperty("cmd")]
		public string Command { get; internal set; }

		/// <summary>
		/// RPC Arguments
		/// </summary>
		[JsonProperty("args", NullValueHandling = NullValueHandling.Ignore)]
		public JObject Arguments { get; internal set; }

		/// <summary>
		/// Nonce
		/// </summary>
		[JsonProperty("nonce")]
		public string Nonce { get; internal set; } = new Random().Next(0, 1000).ToString();

		[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
		public JObject Data { get; internal set; }

		/// <summary>
		/// Event name
		/// </summary>
		[JsonProperty("evt", NullValueHandling = NullValueHandling.Ignore)]
		public string Event { get; internal set; }
    }
}
