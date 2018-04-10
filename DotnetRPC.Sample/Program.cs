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

			var asx = new AsyncExecutor();
			asx.Execute(StartAsync(admin));
		}

		public static async Task StartAsync(bool admin)
		{
			var client = new RpcClient("345229890980937739", admin, Assembly.GetExecutingAssembly().Location);
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

			/*
			var presence = new RpcActivity
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
					LargeImage = "canary-large",
					SmallImage = "ptb-small"
				}
			};
			await client.SetActivityAsync(presence);
			*/

			await client.SetActivityAsync(x =>
			{
				x.Details = "DotnetRPC.Sample";
				x.State = "Part of DSharpPlus";
				x.StartUnix = DateTimeOffset.Now;
				x.EndUnix = DateTimeOffset.Now.AddHours(24);

				x.LargeImage = "canary-large";
				x.LargeImageText = "Testing testing testing";

				x.SmallImage = "ptb-small";
				x.SmallImageText = "ayy ayy";
			});

			await Task.Delay(10000);
			client.Dispose();
			Console.WriteLine("It's gone!");
		}
	}
}
