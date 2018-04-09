using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.IO;
using DotnetRPC.Entities;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotnetRPC
{
	public class RpcClient
	{
		internal PipeClient _pipe;
		internal string _clientid;
		internal Logger _logger;

		public RpcClient(string AppId)
		{
			string Path = Assembly.GetExecutingAssembly().Location;

			this._clientid = AppId;
			this._logger = new Logger();
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				RpcHelpers.RegisterAppWin(AppId, Path, _logger); // Register app protocol for Windows
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				throw new PlatformNotSupportedException("Linux environments are not (yet) supported!"); // Register app protocol for Linux
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				throw new PlatformNotSupportedException("OSX environments are not (yet) supported!"); // Register app protocol for OSX
		}

		public async Task StartAsync()
		{
			_logger.Print(LogLevel.Info, "Connecting to Discord RPC..", DateTimeOffset.Now);
			int rpc = 0;
			for (int i = 0; i < 10; i++)
			{
				_pipe = new PipeClient($"discord-ipc-{i}", this._logger);
				try
				{
					await _pipe.ConnectAsync();
					rpc = i;
					break;
				}
				catch (Exception)
				{
					continue;
				}
			}

			_logger.Print(LogLevel.Info, $"Connected to pipe discord-ipc-{rpc}", DateTimeOffset.Now);
			_logger.Print(LogLevel.Info, "Attempting handshake...", DateTimeOffset.Now);

			var shake = new RpcFrame();
			shake.OpCode = OpCode.Handshake;
			var hs = new RpcHandshake();
			shake.SetContent(JsonConvert.SerializeObject(hs));

			_pipe.QueueData(shake.GetByteData());
			_logger.Print(LogLevel.Info, "Queued handshake", DateTimeOffset.Now);

			await Task.Factory.StartNew(async () =>
			{
				while (true)
				{
					var frame = RpcFrame.FromBytes(await _pipe.ReadNext());
					var content = JsonConvert.DeserializeObject<RpcCommand>(frame.GetStringContent());
					_logger.Print(LogLevel.Debug, $"Received frame with OpCode {frame.OpCode}\nwith Data:\n{JsonConvert.SerializeObject(content)}\n", DateTimeOffset.Now);

					await Task.Delay(50);
				}
			});
		}
	}
}
