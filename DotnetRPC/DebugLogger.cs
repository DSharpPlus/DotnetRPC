using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetRPC
{
    internal class Logger
    {
		public Logger()
		{

		}

		public void Print(LogLevel level, string content, DateTimeOffset time)
		{
			switch (level)
			{
				default:
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case LogLevel.Debug:
					Console.ForegroundColor = ConsoleColor.Blue;
					break;
				case LogLevel.Info:
					Console.ForegroundColor = ConsoleColor.Magenta;
					break;
				case LogLevel.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case LogLevel.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case LogLevel.Critical:
					Console.ForegroundColor = ConsoleColor.Black;
					Console.BackgroundColor = ConsoleColor.Red;
					break;
			}
			Console.Write($"[{level.ToString().ToUpper()}]");
			Console.ResetColor();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write($"[{time.ToString()}] ");
			Console.ResetColor();
			Console.WriteLine(content);
		}
    }

	internal enum LogLevel
	{
		Debug,
		Info,
		Warning,
		Error,
		Critical
	}
}
