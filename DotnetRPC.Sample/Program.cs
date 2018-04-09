using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotnetRPC;

namespace DotnetRPC.Sample
{
	class Program
	{
		private static void Main(string[] args)
		{
			 StartAsync().GetAwaiter().GetResult();
			Console.ReadKey();
		}

		public async static Task StartAsync()
		{
			RpcClient _client = new RpcClient("176019685471551488");
			await _client.StartAsync();
		}
	}
}
