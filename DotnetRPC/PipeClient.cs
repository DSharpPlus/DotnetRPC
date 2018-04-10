using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using DotnetRPC.Entities;
using Newtonsoft.Json;

namespace DotnetRPC
{
	internal class PipeClient
	{
		internal readonly NamedPipeClientStream Stream;
		internal readonly string PipeName;
		internal readonly Logger Logger;

		public PipeClient(string pipeName, Logger logger)
		{
			this.PipeName = pipeName;
			Stream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
			Logger = logger;
		}

		public async Task ConnectAsync()
		{
			await Stream.ConnectAsync();
			await Task.Delay(1000);
			if (!Stream.IsConnected)
				throw new Exception("Connection failed. Attempting next ipc pipe.");


			Logger.Print(LogLevel.Info, $"Connected to {PipeName}.", DateTimeOffset.Now);
		}

		public async Task<byte[]> ReadNext()
		{
			if (!Stream.IsConnected)
				throw new Exception("Pipe is not connected!");

			var buffer = new byte[Stream.InBufferSize];
			await Stream.ReadAsync(buffer, 0, Stream.InBufferSize);

			return buffer;
		}

		public async Task WriteAsync(RpcFrame frame)
		{
			if (!Stream.IsConnected)
				throw new Exception("Pipe is not connected!");

			var bf = frame.GetByteData();
			await Stream.WriteAsync(bf, 0, bf.Length);
			Logger.Print(LogLevel.Debug, $"Wrote frame with OpCode {frame.OpCode}\nwith Data:\n{JsonConvert.SerializeObject(frame.GetStringContent())}", DateTimeOffset.Now);
		}
	}
}
