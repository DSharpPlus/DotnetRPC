using System;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using DotnetRPC.Entities;

namespace DotnetRPC.Sample
{
	class Program
	{
		private static void Main(string[] args)
		{
			var admin = false;
			using (var identity = WindowsIdentity.GetCurrent())
			{
				var principal = new WindowsPrincipal(identity);
				admin = principal.IsInRole(WindowsBuiltInRole.Administrator);
			}

			StartAsync(admin).GetAwaiter().GetResult();
			Console.ReadKey();
		}

		public async static Task StartAsync(bool admin)
		{
			var client = new RpcClient("176019685471551488", admin, Assembly.GetExecutingAssembly().Location);
			await client.StartAsync();

			var presence = new RpcPresence
			{
				Details = "DotnetRPC.Sample",
				State = "Part of DSharpPlus",
				Timestamps = new RpcTimestamps
				{
					Start = DateTimeOffset.Now,
					End = DateTimeOffset.Now.AddDays(365)
				},
				Assets = new RpcAssets
				{
					LargeText = "hello",
					SmallText = "test",
					LargeImage = "saiko",
					SmallImage = "saikosml"
				}
			};

			await client.SetActivityAsync(presence);
		}
	}
}
