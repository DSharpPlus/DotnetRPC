namespace DotnetRPC.Net
{
    internal static class Commands
    {
		// not all of these can be implemented over piping
        internal const string SetActivity = "SET_ACTIVITY";
		internal const string Dispatch = "DISPATCH";
		internal const string Authorize = "AUTHORIZE";
		internal const string Authenticate = "AUTHENTICATE";
		internal const string GetGuild = "GET_GUILD";
		internal const string GetGuilds = "GET_GUILDS";
		internal const string GetChannel = "GET_CHANNEL";
		internal const string GetChannels = "GET_CHANNELS";
		internal const string Subscribe = "SUBSCRIBE";
		internal const string Unsubscribe = "UNSUBSCRIBE";
		internal const string SetUserVoiceSettings = "SET_USER_VOICE_SETTINGS";
		internal const string SelectVoiceChannel = "SELECT_VOICE_CHANNEL";
		internal const string GetSelectedVoiceChannel = "GET_SELECTED_VOICE_CHANNEL";
		internal const string SelectTextChannel = "SELECT_TEXT_CHANNEL";
		internal const string GetVoiceSettings = "GET_VOICE_SETTINGS";
		internal const string SetVoiceSettings = "SET_VOICE_SETTINGS";
		internal const string CaptureShortcut = "CAPTURE_SHORTCUT";
		internal const string SetCertifiedDevices = "SET_CERTIFIED_DEVICES";
		internal const string SendActivityJoinInvite = "SEND_ACTIVITY_JOIN_INVITE";
		internal const string CloseActivityRequest = "CLOSE_ACTIVITY_REQUEST";
    }
}