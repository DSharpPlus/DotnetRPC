using System;
using Microsoft.Win32;

namespace DotnetRPC
{
	internal class RpcHelpers
	{
		/// <summary>
		/// Register application protocol as discord-[appid]:// (Windows only)
		/// </summary>
		/// <param name="appId">Your app's Client ID</param>
		/// <param name="exePath">Path to your app EXE</param>
		public static void RegisterAppWin(string appId, string exePath, Logger logger)
		{
			// Register application protocol as discord-[appid]://
			var key = Registry.ClassesRoot.OpenSubKey($"discord-{appId}");  // Open protocol key

			if (key != null)
				Registry.ClassesRoot.DeleteSubKeyTree($"discord-{appId}");

			key = Registry.ClassesRoot.CreateSubKey($"discord-{appId}"); // Create new key if not exists

			key.SetValue(string.Empty, $"URL: Run game {appId} Protocol");
			key.SetValue("URL Protocol", string.Empty);

			var command = key.CreateSubKey(@"shell\open\command");
			command.SetValue(string.Empty, exePath);

			var defaulticon = key.CreateSubKey(@"DefaultIcon");
			defaulticon.SetValue(string.Empty, exePath);

			// Close registry keys.
			command.Close();
			defaulticon.Close();
			key.Close();

			logger.Print(LogLevel.Info, "Registered registry key for this app.", DateTimeOffset.Now);
		}
	}
}
