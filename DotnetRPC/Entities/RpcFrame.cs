using System;
using System.Text;

namespace DotnetRPC.Entities
{
	public enum OpCode
	{
		Handshake = 0,
		Frame = 1,
		Close = 2,
		Ping = 3,
		Pong = 4
	}

	internal class RpcFrame
	{
		public OpCode OpCode { get; internal set; }

		public byte[] Data { get; internal set; }

		public int Length => Data.Length;

		public void SetContent(string input)
		{
			Data = Encoding.UTF8.GetBytes(input);
		}

		public string GetStringContent()
		{
			return Encoding.UTF8.GetString(Data);
		}

		public byte[] GetByteData()
		{
			var opcode = BitConverter.GetBytes((int)OpCode);
			var length = BitConverter.GetBytes(Length);
			const int opcodeSize = sizeof(int);
			const int lengthSize = sizeof(int);

			var buffer = new byte[opcodeSize + lengthSize + Data.Length];
			opcode.CopyTo(buffer, 0);
			length.CopyTo(buffer, opcodeSize);
			Data.CopyTo(buffer, opcodeSize + lengthSize);

			return buffer;
		}

		public static RpcFrame FromBytes(byte[] input)
		{
			var frame = new RpcFrame();

			var opcode = new byte[4];
			var length = new byte[4];
			var data = new byte[input.Length - 8];

			Array.Copy(input, 0, opcode, 0, 4);
			Array.Copy(input, 4, length, 0, 4);
			Array.Copy(input, 8, data, 0, data.Length);

			frame.OpCode = (OpCode)BitConverter.ToInt32(opcode, 0);
			frame.Data = data;

			return frame;
		}
	}
}
