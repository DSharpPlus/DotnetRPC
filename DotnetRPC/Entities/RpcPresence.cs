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
		public RpcPresence Presence { get; internal set; }

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this);
		}
	}

	public class RpcPresence
	{
		[JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
		public string State { get; set; }

		[JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
		public string Details { get; set; }

		[JsonProperty("timestamps", NullValueHandling = NullValueHandling.Ignore)]
		public RpcTimestamps Timestamps { get; set; }

		[JsonProperty("assets", NullValueHandling = NullValueHandling.Ignore)]
		public RpcAssets Assets { get; set; }

		[JsonProperty("party", NullValueHandling = NullValueHandling.Ignore)]
		public RpcParty Party { get; set; }

		[JsonProperty("secrets", NullValueHandling = NullValueHandling.Ignore)]
		public RpcSecrets Secrets { get; set; }

		[JsonProperty("instance", NullValueHandling = NullValueHandling.Ignore)]
		public bool Instance { get; set; } = true;
	}

	public class RpcTimestamps
	{
		[JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
		public int StartUnix { get; set; }

		[JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
		public int EndUnix { get; set; }
	}

	public class RpcAssets
	{
		[JsonProperty("large_image", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImage { get; set; }

		[JsonProperty("large_text", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeText { get; set; }

		[JsonProperty("small_image", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImage { get; set; }

		[JsonProperty("small_text", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallText { get; set; }
	}

	public class RpcParty
	{
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string Id { get; set; }

		/// <summary>
		/// Two ints: current_size and max_size
		/// </summary>
		[JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
		public int[] Size { get; set; }
	}

	public class RpcSecrets
	{
		[JsonProperty("join", NullValueHandling = NullValueHandling.Ignore)]
		public string Join { get; set; }

		[JsonProperty("spectate", NullValueHandling = NullValueHandling.Ignore)]
		public string Spectate { get; set; }

		[JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
		public string Match { get; set; }
	}
}
