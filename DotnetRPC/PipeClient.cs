using DotnetRPC.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetRPC
{
	internal class PipeClient
	{
		internal readonly NamedPipeClientStream Pipe;
		internal readonly string PipeName;
		internal readonly Logger Logger;

		public PipeClient(string pipeName, Logger logger)
		{
			this.PipeName = pipeName;
			Pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
			Logger = logger;
		}

		public async Task ConnectAsync()
		{
			await Pipe.ConnectAsync();
			await Task.Delay(1000);
			if (!Pipe.IsConnected)
				throw new Exception("Connection failed. Attempting next ipc pipe.");


			Logger.Print(LogLevel.Info, $"Connected to {PipeName}.", DateTimeOffset.Now);
		}

		public async Task<byte[]> ReadNext()
		{
			if (!Pipe.IsConnected)
				throw new Exception("Pipe is not connected!");

			var buffer = new byte[Pipe.InBufferSize];
			await Pipe.ReadAsync(buffer, 0, (int)Pipe.InBufferSize);

			return buffer;
		}

		public async Task WriteAsync(RpcFrame frame)
		{
			if (!Pipe.IsConnected)
				throw new Exception("Pipe is not connected!");

			var bf = frame.GetByteData();
			await Pipe.WriteAsync(bf, 0, bf.Length);
			Logger.Print(LogLevel.Debug, $"Wrote frame with OpCode {frame.OpCode}\nwith Data:\n{JsonConvert.SerializeObject(frame.GetStringContent())}", DateTimeOffset.Now);
		}
	}
}
