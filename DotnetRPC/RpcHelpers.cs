using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetRPC
{
	internal class RpcHelpers
	{
		/// <summary>
		/// Register application protocol as discord-[appid]:// (Windows only)
		/// </summary>
		/// <param name="AppId">Your app's Client ID</param>
		/// <param name="ExePath">Path to your app EXE</param>
		public static void RegisterAppWin(string AppId, string ExePath, Logger logger)
		{
			// Register application protocol as discord-[appid]://
			RegistryKey key = Registry.ClassesRoot.OpenSubKey($"discord-{AppId}");  // Open protocol key

			if (key != null)
				Registry.ClassesRoot.DeleteSubKeyTree($"discord-{AppId}");

			key = Registry.ClassesRoot.CreateSubKey($"discord-{AppId}"); // Create new key if not exists

			key.SetValue(string.Empty, $"URL: Run game {AppId} Protocol");
			key.SetValue("URL Protocol", string.Empty);

			var command = key.CreateSubKey(@"shell\open\command");
			command.SetValue(string.Empty, ExePath);

			var defaulticon = key.CreateSubKey(@"DefaultIcon");
			defaulticon.SetValue(string.Empty, ExePath);

			// Close registry keys.
			command.Close();
			defaulticon.Close();
			key.Close();

			logger.Print(LogLevel.Info, "Registered registry key for this app.", DateTimeOffset.Now);
		}
	}
}
