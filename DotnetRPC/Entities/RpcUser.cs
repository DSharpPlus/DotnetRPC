using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

// Some parts taken from DSharpPlus but oh well, I own that repo anyway
namespace DotnetRPC.Entities
{
    public class RpcUser
    {
		[JsonProperty("id")]
		public ulong Id { get; internal set; }

		[JsonProperty("username")]
		public string Username { get; internal set; }

		[JsonProperty("discriminator")]
		public string Discriminator { get; internal set; }

		internal int DiscriminatorInt
			=> int.Parse(this.Discriminator, NumberStyles.Integer, CultureInfo.InvariantCulture);

		[JsonProperty("avatar")]
		public string AvatarHash { get; internal set; }

		[JsonIgnore]
		public string AvatarUrl
			=> !string.IsNullOrWhiteSpace(this.AvatarHash) ? (AvatarHash.StartsWith("a_") ? $"https://cdn.discordapp.com/avatars/{this.Id.ToString(CultureInfo.InvariantCulture)}/{AvatarHash}.gif?size=1024" : $"https://cdn.discordapp.com/avatars/{Id}/{AvatarHash}.png?size=1024") : this.DefaultAvatarUrl;

		[JsonIgnore]
		public string DefaultAvatarUrl
			=> $"https://cdn.discordapp.com/embed/avatars/{(this.DiscriminatorInt % 5).ToString(CultureInfo.InvariantCulture)}.png?size=1024";

		[JsonProperty("bot")]
		public bool IsBot { get; internal set; } = false; // Very likely not kek

		// RPC doesn't return much more than this.. anything more actually
	}
}
