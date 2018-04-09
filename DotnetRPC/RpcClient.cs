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
		#region Events

		public event AsyncEventHandler<AsyncEventArgs> ConnectionClosed
		{
			add => this._connectionclosed.Register(value);
			remove => this._connectionclosed.Unregister(value);
		}
		private readonly AsyncEvent<AsyncEventArgs> _connectionclosed;

		#endregion
		internal PipeClient Pipe;
		internal string ClientId;
		internal readonly Logger Logger;

		public RpcClient(string appId, bool registerApp, string exePath)
		{
			this.ClientId = appId;
			this.Logger = new Logger();
			if (registerApp)
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					RpcHelpers.RegisterAppWin(appId, exePath, Logger); // Register app protocol for Windows
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					throw new PlatformNotSupportedException("App protocols on Linux environments are not (yet) supported!"); // Register app protocol for Linux
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					throw new PlatformNotSupportedException("App protocols on OSX environments are not (yet) supported!"); // Register app protocol for OSX
			}

			this._connectionclosed = new AsyncEvent<AsyncEventArgs>(EventError, "CONNECTION_CLOSE");
		}

		internal void EventError(string evname, Exception ex)
		{
			this.Logger.Print(LogLevel.Error, $"", DateTimeOffset.Now);
		}

		public async Task StartAsync()
		{
			Logger.Print(LogLevel.Info, "Connecting to Discord RPC..", DateTimeOffset.Now);
			var rpc = 0;
			for (var i = 0; i < 10; i++)
			{
				Pipe = new PipeClient($"discord-ipc-{i}", this.Logger);
				try
				{
					await Pipe.ConnectAsync();
					rpc = i;
					break;
				}
				catch (Exception)
				{
					continue;
				}
			}

			Logger.Print(LogLevel.Info, $"Connected to pipe discord-ipc-{rpc}", DateTimeOffset.Now);
			Logger.Print(LogLevel.Info, "Attempting handshake...", DateTimeOffset.Now);

			var shake = new RpcFrame {OpCode = OpCode.Handshake};
			var hs = new RpcHandshake();
			shake.SetContent(JsonConvert.SerializeObject(hs));

			await Pipe.WriteAsync(shake);
			Logger.Print(LogLevel.Info, "Sent handshake", DateTimeOffset.Now);

			await Task.Factory.StartNew(async () =>
			{
				while (true)
				{
					var frame = RpcFrame.FromBytes(await Pipe.ReadNext());
					var content = JsonConvert.DeserializeObject<RpcCommand>(frame.GetStringContent());
					Logger.Print(LogLevel.Debug, $"Received frame with OpCode {frame.OpCode}\nwith Data:\n{JsonConvert.SerializeObject(content)}", DateTimeOffset.Now);

					// Handle frame here

					switch (frame.OpCode)
					{
						case OpCode.Close:
							Pipe.Pipe.Close();
							Logger.Print(LogLevel.Warning, $"Received Opcode Close. Closing RPC connection.", DateTimeOffset.Now);
							await _connectionclosed.InvokeAsync(null);
							break;
					}

					await Task.Delay(50);
				}
			});
		}

		public async Task UpdatePresenceAsync(RpcPresence presence)
		{
			var frame = new RpcFrame();

			var presenceupdate = new RpcPresenceUpdate
			{
				ProcessId = Process.GetCurrentProcess().Id,
				Presence = presence
			};

			var cmd = new RpcCommand
			{
				Arguments = JObject.FromObject(presenceupdate),
				Command = "SET_ACTIVITY",
				Nonce = new Random().Next(0, int.MaxValue).ToString()
			};

			frame.OpCode = OpCode.Frame;
			frame.SetContent(JsonConvert.SerializeObject(cmd));

			await Pipe.WriteAsync(frame);
		}
	}
}
