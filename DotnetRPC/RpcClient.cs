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
			add { this._connectionclosed.Register(value); }
			remove { this._connectionclosed.Unregister(value); }
		}
		private AsyncEvent<AsyncEventArgs> _connectionclosed;

		#endregion
		internal PipeClient _pipe;
		internal string _clientid;
		internal Logger _logger;

		public RpcClient(string AppId, bool RegisterApp, string ExePath)
		{
			this._clientid = AppId;
			this._logger = new Logger();
			if (RegisterApp)
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					RpcHelpers.RegisterAppWin(AppId, ExePath, _logger); // Register app protocol for Windows
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					throw new PlatformNotSupportedException("App protocols on Linux environments are not (yet) supported!"); // Register app protocol for Linux
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					throw new PlatformNotSupportedException("App protocols on OSX environments are not (yet) supported!"); // Register app protocol for OSX
			}

			this._connectionclosed = new AsyncEvent<AsyncEventArgs>(EventError, "CONNECTION_CLOSE");
		}

		internal void EventError(string evname, Exception ex)
		{
			this._logger.Print(LogLevel.Error, $"", DateTimeOffset.Now);
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

			await _pipe.WriteAsync(shake);
			_logger.Print(LogLevel.Info, "Sent handshake", DateTimeOffset.Now);

			await Task.Factory.StartNew(async () =>
			{
				while (true)
				{
					var frame = RpcFrame.FromBytes(await _pipe.ReadNext());
					var content = JsonConvert.DeserializeObject<RpcCommand>(frame.GetStringContent());
					_logger.Print(LogLevel.Debug, $"Received frame with OpCode {frame.OpCode}\nwith Data:\n{JsonConvert.SerializeObject(content)}", DateTimeOffset.Now);

					// Handle frame here

					switch (frame.OpCode)
					{
						case OpCode.Close:
							_pipe._pipe.Close();
							_logger.Print(LogLevel.Warning, $"Received Opcode Close. Closing RPC connection.", DateTimeOffset.Now);
							await _connectionclosed.InvokeAsync(null);
							break;
					}

					await Task.Delay(50);
				}
			});
		}

		public async Task UpdateActivityAsync(RpcPresence presence)
		{
			var frame = new RpcFrame();

			var presenceupdate = new RpcPresenceUpdate();
			presenceupdate.ProcessId = Process.GetCurrentProcess().Id;
			presenceupdate.Presence = presence;

			var cmd = new RpcCommand();
			cmd.Arguments = JObject.FromObject(presenceupdate);
			cmd.Command = "SET_ACTIVITY";
			cmd.Nonce = new Random().Next(0, int.MaxValue).ToString();

			frame.OpCode = OpCode.Frame;
			frame.SetContent(JsonConvert.SerializeObject(cmd));

			await _pipe.WriteAsync(frame);
		}

		public void RegisterAppProtocol(string exepath)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				RpcHelpers.RegisterAppWin(this._clientid, exepath, _logger); // Register app protocol for Windows
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				throw new PlatformNotSupportedException("App protocols on Linux environments are not (yet) supported!"); // Register app protocol for Linux
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				throw new PlatformNotSupportedException("App protocols on OSX environments are not (yet) supported!"); // Register app protocol for OSX
		}
	}
}
