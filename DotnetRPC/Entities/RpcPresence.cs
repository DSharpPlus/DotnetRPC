using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DotnetRPC.Entities
{
    internal class RpcPresenceUpdate
    {
		[JsonProperty("pid")]
		public int ProcessId { get; internal set; }

		[JsonProperty("activity", NullValueHandling = NullValueHandling.Ignore)]
		public RpcActivity Activity { get; internal set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}

	internal class RpcActivity
	{
		[JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
		public string State { get; internal set; }

		[JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
		public string Details { get; internal set; }

		[JsonProperty("timestamps", NullValueHandling = NullValueHandling.Ignore)]
		public RpcTimestamps TimeStamps { get; internal set; }

		[JsonProperty("assets", NullValueHandling = NullValueHandling.Ignore)]
		public RpcAssets Assets { get; internal set; }

		[JsonProperty("party", NullValueHandling = NullValueHandling.Ignore)]
		public RpcParty Party { get; internal set; }

		[JsonProperty("secrets", NullValueHandling = NullValueHandling.Ignore)]
		public RpcSecrets Secrets { get; internal set; }

		[JsonProperty("instance", NullValueHandling = NullValueHandling.Ignore)]
		public bool Instance { get; internal set; } = true;
	}

	internal class RpcTimestamps
	{
		[JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
		public int StartUnix { get; internal set; }

		[JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
		public int EndUnix { get; internal set; }
	}

	internal class RpcAssets
	{
		[JsonProperty("large_image", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImage { get; internal set; }

		[JsonProperty("large_text", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeText { get; internal set; }

		[JsonProperty("small_image", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImage { get; internal set; }

		[JsonProperty("small_text", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallText { get; internal set; }
	}

	internal class RpcParty
	{
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; internal set; }

		/// <summary>
		/// Two ints: current_size and max_size
		/// </summary>
		[JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
		public int[] Size { get; internal set; }
	}

	internal class RpcSecrets
	{
		[JsonProperty("join", NullValueHandling = NullValueHandling.Ignore)]
		public string Join { get; internal set; }

		[JsonProperty("spectate", NullValueHandling = NullValueHandling.Ignore)]
		public string Spectate { get; internal set; }

		[JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
		public string Match { get; internal set; }
	}
}
