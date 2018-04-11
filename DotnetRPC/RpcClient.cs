using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DotnetRPC.Entities;
using DotnetRPC.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotnetRPC
{
	public class RpcClient : IDisposable
	{
		#region Events

		public event AsyncEventHandler<AsyncEventArgs> ConnectionClosed
		{
			add => _connectionClosed.Register(value);
			remove => _connectionClosed.Unregister(value);
		}
		private readonly AsyncEvent<AsyncEventArgs> _connectionClosed;

		public event AsyncEventHandler<ClientErroredEventArgs> ClientErrored
		{
			add => _clientErrored.Register(value);
			remove => _clientErrored.Unregister(value);
		}
		private readonly AsyncEvent<ClientErroredEventArgs> _clientErrored;

		public event AsyncEventHandler<RpcReady> Ready
		{
			add => _ready.Register(value);
			remove => _ready.Unregister(value);
		}
		private readonly AsyncEvent<RpcReady> _ready;
		#endregion

		public RpcUser CurrentUser { get; internal set; } = null;
		public int Version { get; internal set; } = 1;

		private ApiClient ApiClient { get; set; }
		private PipeClient Pipe { get; set; }
		private string ClientId { get; }
		private Logger Logger { get; }

		public RpcClient(string appId, bool registerApp = false, string exePath = null)
		{
			ClientId = appId;
			Logger = new Logger();

			if (registerApp)
			{
				if (exePath == null)
					throw new ArgumentNullException(nameof(exePath), "Pitiful! If you set registerApp to true, you " +
																	 "must provide path to the executable to register.");

				RegisterAppProtocol(exePath);
			}

			_connectionClosed = new AsyncEvent<AsyncEventArgs>(EventError, "CONNECTION_CLOSE");
			_clientErrored = new AsyncEvent<ClientErroredEventArgs>(EventError, "CLIENT_ERROR");
			_ready = new AsyncEvent<RpcReady>(EventError, Events.Ready);
		}

		internal void EventError(string evname, Exception ex)
		{
			Logger.Print(LogLevel.Error, $"Whope! Error in event handler: {ex.StackTrace}", DateTimeOffset.Now);
		}

		public async Task ConnectAsync()
		{
			Logger.Print(LogLevel.Info, "Connecting to Discord RPC..", DateTimeOffset.Now);
			var rpc = 0;
			for (var i = 0; i < 10; i++)
			{
				Pipe = new PipeClient($"discord-ipc-{i}", Logger);
				try
				{
					await Pipe.ConnectAsync();
					rpc = i;
					break;
				}
				catch (Exception)
				{
				}
			}
			// TODO: handle the failure case

			ApiClient = new ApiClient(Pipe, Logger);

			Logger.Print(LogLevel.Info, $"Connected to pipe discord-ipc-{rpc}", DateTimeOffset.Now);
			Logger.Print(LogLevel.Info, "Attempting handshake...", DateTimeOffset.Now);

			var shake = new RpcFrame { OpCode = OpCode.Handshake };
			var hs = new RpcHandshake { ClientId = ClientId };
			shake.SetContent(JsonConvert.SerializeObject(hs));

			await Pipe.WriteAsync(shake);
			Logger.Print(LogLevel.Info, "Sent handshake", DateTimeOffset.Now);

			await Task.Factory.StartNew(async () =>
			{
				try
				{
					while (Pipe != null && Pipe.Stream.IsConnected)
					{
						var frame = RpcFrame.FromBytes(await Pipe.ReadNext());
						var content = JsonConvert.DeserializeObject<RpcCommand>(frame.GetStringContent());
						Logger.Print(LogLevel.Debug,
							$"Received frame with OpCode {frame.OpCode}\nwith Data:\n{JsonConvert.SerializeObject(content)}",
							DateTimeOffset.Now);

						// Handle frame here

						switch (frame.OpCode)
						{
							case OpCode.Close:
								Dispose();
								Logger.Print(LogLevel.Warning, "Received Opcode Close. Closing RPC connection.", DateTimeOffset.Now);
								await _connectionClosed.InvokeAsync(null);
								break;
							case OpCode.Frame:
								var cmd = JsonConvert.DeserializeObject<RpcCommand>(frame.GetStringContent());
								Logger.Print(LogLevel.Debug, $"Received Opcode Frame with command {cmd.Command}", DateTimeOffset.Now);
								await HandleFrameAsync(cmd);
								break;
						}

						await Task.Delay(50);
					}

					Logger.Print(LogLevel.Info, "Disconnected! Thread pool is no longer a slave.", DateTimeOffset.Now);
					await _connectionClosed.InvokeAsync(null);
				}
				catch (Exception e)
				{
					await _clientErrored.InvokeAsync(new ClientErroredEventArgs { Exception = e });
				}
			});
		}

		internal async Task HandleFrameAsync(RpcCommand frame)
		{
			switch (frame.Command)
			{
				case Commands.Dispatch:
					await HandleEventAsync(frame.Event, frame.Data);
					break;
			}
		}

		internal async Task HandleEventAsync(string evt, JObject data)
		{
			switch (evt)
			{
				case Events.Ready:
					var ready = data.ToObject<RpcReady>();

					this.Logger.Print(LogLevel.Info, $"Received READY with user {ready.User.Username}#{ready.User.Discriminator} with ID {ready.User.Id}", DateTimeOffset.Now);

					this.CurrentUser = ready.User;
					this.Version = ready.Version;

					await _ready.InvokeAsync(ready);
					break;
			}
		}

		/// <summary>
		/// Used to update a user's Rich Presence.
		/// </summary>
		/// <param name="activity">The activity/presence to set, or <c>null</c> to set a playing status without Rich
		/// Presence.</param>
		/// <param name="pid">The application's process ID, defaults to the current process' PID</param>
		/// <returns>Task resolving when the command is executed</returns>
		public async Task SetActivityAsync(RpcActivity activity, int pid = -1)
		{
			await ApiClient.SendCommandAsync(Commands.SetActivity, new RpcActivityUpdate
			{
				ProcessId = pid != -1 ? pid : Process.GetCurrentProcess().Id,
				Activity = activity
			});
		}

		public async Task SetActivityAsync(Action<ActivitySetModel> action, int pid = -1)
		{
			var mdl = new ActivitySetModel();
			action(mdl);

			var activity = new RpcActivity();

			#region oof
			if (!string.IsNullOrEmpty(mdl.LargeImage)
				|| !string.IsNullOrEmpty(mdl.LargeImageText)
				|| !string.IsNullOrEmpty(mdl.SmallImage)
				|| !string.IsNullOrEmpty(mdl.SmallImageText))
				activity.Assets = new RpcAssets();

			if (!string.IsNullOrEmpty(mdl.LargeImage))
				activity.Assets.LargeImage = mdl.LargeImage;
			if (!string.IsNullOrEmpty(mdl.SmallImage))
				activity.Assets.SmallImage = mdl.SmallImage;
			if (!string.IsNullOrEmpty(mdl.LargeImageText))
				activity.Assets.LargeText = mdl.LargeImageText;
			if (!string.IsNullOrEmpty(mdl.SmallImageText))
				activity.Assets.SmallText = mdl.SmallImageText;

			if (!string.IsNullOrEmpty(mdl.Details))
				activity.Details = mdl.Details;
			if (!string.IsNullOrEmpty(mdl.State))
				activity.State = mdl.State;

			if ((mdl.CurrentPartySize > 0 && mdl.MaxPartySize > 0) || !string.IsNullOrEmpty(mdl.PartyId))
				activity.Party = new RpcParty();

			if (!string.IsNullOrEmpty(mdl.PartyId))
				activity.Party.Id = mdl.PartyId;
			if (mdl.CurrentPartySize > 0 && mdl.MaxPartySize > 0)
				activity.Party.Size = new int[2] { mdl.CurrentPartySize, mdl.MaxPartySize };

			if (!string.IsNullOrEmpty(mdl.JoinSecret)
				|| !string.IsNullOrEmpty(mdl.MatchSecret)
				|| !string.IsNullOrEmpty(mdl.SpectateSecret))
				activity.Secrets = new RpcSecrets();

			if (!string.IsNullOrEmpty(mdl.JoinSecret))
				activity.Secrets.Join = mdl.JoinSecret;
			if (!string.IsNullOrEmpty(mdl.MatchSecret))
				activity.Secrets.Match = mdl.MatchSecret;
			if (!string.IsNullOrEmpty(mdl.SpectateSecret))
				activity.Secrets.Spectate = mdl.SpectateSecret;

			if (mdl.StartUnix._unixvalue > 0 && mdl.EndUnix._unixvalue > 0)
				activity.Timestamps = new RpcTimestamps();

			if (mdl.StartUnix._unixvalue > 0)
				activity.Timestamps.StartUnix = mdl.StartUnix;

			if (mdl.EndUnix._unixvalue > 0)
				activity.Timestamps.EndUnix = mdl.EndUnix;
			#endregion

			await this.SetActivityAsync(activity, pid);
		}

		/// <summary>
		/// Used to clear a user's Rich Presence and playing status.
		/// </summary>
		/// <param name="pid">The application's process ID, defaults to the current process' PID</param>
		/// <returns>Task resolving when the command is executed</returns>
		public async Task ClearActivityAsync(int pid = -1)
		{
			await ApiClient.SendCommandAsync(Commands.SetActivity, new RpcEmptyActivityUpdate
			{
				ProcessId = pid != -1 ? pid : Process.GetCurrentProcess().Id,
			});
		}

		public void RegisterAppProtocol(string exePath)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				RpcHelpers.RegisterAppWin(ClientId, exePath, Logger); // Register app protocol for Windows
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				throw new PlatformNotSupportedException("App protocols on Linux environments are not (yet) supported!"); // Register app protocol for Linux
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				throw new PlatformNotSupportedException("App protocols on OSX environments are not (yet) supported!"); // Register app protocol for OSX
		}

		/// <summary>
		/// Disconnects from RPC. This will free up an IPC slot, and will require calling <see cref="ConnectAsync"/> to
		/// use the client again.
		/// </summary>
		public void Disconnect() => Dispose();

		public void Dispose()
		{
			Pipe.Stream.Close();
			Pipe.Stream.Dispose();
			Pipe = null;
			ApiClient = null;
		}
	}
}