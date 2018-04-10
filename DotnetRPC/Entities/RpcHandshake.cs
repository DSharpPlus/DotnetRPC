using Newtonsoft.Json;

namespace DotnetRPC.Entities
{
    internal class RpcHandshake
    {
		[JsonProperty("v")]
		public int Version = 1;

		[JsonProperty("client_id")]
		public string ClientId;
	}
}
