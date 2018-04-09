using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using DotnetRPC;
using DotnetRPC.Entities;

namespace DotnetRPC.Sample
{
	class Program
	{
		private static void Main(string[] args)
		{
			bool admin = false;
			using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
			{
				WindowsPrincipal principal = new WindowsPrincipal(identity);
				admin = principal.IsInRole(WindowsBuiltInRole.Administrator);
			}

			StartAsync(admin).GetAwaiter().GetResult();
			Console.ReadKey();
		}

		public async static Task StartAsync(bool admin)
		{
			RpcClient _client = new RpcClient("176019685471551488", admin, Assembly.GetExecutingAssembly().Location);
			await _client.StartAsync();

			RpcPresence presence = new RpcPresence()
			{
				Details = "DotnetRPC.Sample",
				State = "Part of DSharpPlus",
				Timestamps = new RpcTimestamps()
				{
					StartUnix = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
					EndUnix = (int)DateTimeOffset.Now.AddDays(365).ToUnixTimeSeconds()
				},
				Assets = new RpcAssets()
				{
					LargeText = "hello",
					SmallText = "test",
					LargeImage = "saiko",
					SmallImage = "saikosml"
				}
			};

			await _client.UpdatePresenceAsync(presence);
		}
	}
}
