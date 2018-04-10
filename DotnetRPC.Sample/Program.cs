using System;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using DotnetRPC.Entities;

namespace DotnetRPC.Sample
{
	internal static class Program
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

		}

		public static async Task StartAsync(bool admin)
		{
			var client = new RpcClient("176019685471551488", admin, Assembly.GetExecutingAssembly().Location);
			client.ConnectionClosed += _ =>
			{
				Console.WriteLine("Disconnected!");
				return Task.CompletedTask;
			};
			client.ClientErrored += args =>
			{
				Console.WriteLine($"Client error: {args.Exception}");
				return Task.CompletedTask;
			};
			await client.ConnectAsync();

			var presence = new RpcActivity
			{
				Details = "DotnetRPC.Sample",
				State = "Part of DSharpPlus",
				Timestamps =
				{
					Start = DateTimeOffset.Now,
					End = DateTimeOffset.Now.AddDays(365)
				},
				Assets =
				{
					LargeText = "hello",
					SmallText = "test",
					LargeImage = "saiko",
					SmallImage = "saikosml"
				}
			};
			await client.SetActivityAsync(presence);

			await Task.Delay(10000);
			client.Dispose();
			Console.WriteLine("It's gone!");
		}
	}
}
