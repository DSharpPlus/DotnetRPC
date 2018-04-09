using DotnetRPC.Entities;
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
		internal List<byte[]> _frame_queue = new List<byte[]>();
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

		public void QueueData(byte[] data)
		{
			_frame_queue.Add(data);
		}

		public async Task<byte[]> ReadNext()
		{
			if (!_pipe.IsConnected)
				throw new Exception("Pipe is not connected! (READ)");

			if (_frame_queue.Count > 0)
			{
				var dat = _frame_queue.First();

				await _pipe.WriteAsync(dat, 0, dat.Length);
				RpcFrame frame = RpcFrame.FromBytes(dat);
				_logger.Print(LogLevel.Debug, $"Sent frame with OpCode {frame.OpCode}\nwith Data:\n{frame.GetStringContent()}", DateTimeOffset.Now);

				_frame_queue.RemoveAt(_frame_queue.Count - 1);
			}


			var buffer = new byte[_pipe.InBufferSize];
			await _pipe.ReadAsync(buffer, 0, (int)_pipe.InBufferSize);

			return buffer;
		}
	}
}
