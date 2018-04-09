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
		internal NamedPipeClientStream _pipe;
		internal string _pipename;
		internal Logger _logger;

		public PipeClient(string pipename, Logger logger)
		{
			this._pipename = pipename;
			_pipe = new NamedPipeClientStream(".", pipename, PipeDirection.InOut);
			_logger = logger;
		}

		public async Task ConnectAsync()
		{
			await _pipe.ConnectAsync();
			await Task.Delay(1000);
			if (!_pipe.IsConnected)
				throw new Exception("Connection failed. Attempting next ipc pipe.");


			_logger.Print(LogLevel.Info, $"Connected to {_pipename}.", DateTimeOffset.Now);
		}

		public async Task<byte[]> ReadNext()
		{
			if (!_pipe.IsConnected)
				throw new Exception("Pipe is not connected!");

			var buffer = new byte[_pipe.InBufferSize];
			await _pipe.ReadAsync(buffer, 0, (int)_pipe.InBufferSize);

			return buffer;
		}

		public async Task WriteAsync(RpcFrame frame)
		{
			if (!_pipe.IsConnected)
				throw new Exception("Pipe is not connected!");

			var bf = frame.GetByteData();
			await _pipe.WriteAsync(bf, 0, bf.Length);
			_logger.Print(LogLevel.Debug, $"Wrote frame with OpCode {frame.OpCode}\nwith Data:\n{JsonConvert.SerializeObject(frame.GetStringContent())}", DateTimeOffset.Now);
		}
	}
}
