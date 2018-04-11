using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetRPC.Net
{
    internal static class Events
    {
		internal const string Ready = "READY";
		internal const string Error = "ERROR";
		internal const string GuildStatus = "GUILD_STATUS";
		internal const string GuildCreate = "GUILD_CREATE";
		internal const string ChannelCreate = "CHANNEL_CREATE";
		internal const string VoiceChannelSelect = "VOICE_CHANNEL_SELECT";
		internal const string VoiceStateCreate = "VOICE_STATE_CREATE";
		internal const string VoiceStateUpdate = "VOICE_STATE_UPDATE";
		internal const string VoiceStateDelete = "VOICE_STATE_DELETE";
		internal const string VoiceSettingsUpdate = "VOICE_SETTINGS_UPDATE";
		internal const string VoiceConnectionStatus = "VOICE_CONNECTION_STATUS";
		internal const string SpeakingStart = "SPEAKING_START";
		internal const string SpeakingStop = "SPEAKING_STOP";
		internal const string MessageCreate = "MESSAGE_CREATE";
		internal const string MessageUpdate = "MESSAGE_UPDATE";
		internal const string MessageDelete = "MESSAGE_DELETE";
		internal const string NotificationCreate = "NOTIFICATION_CREATE";
		internal const string CaptureShortcutChange = "CAPTURE_SHORTCUT_CHANGE";
		internal const string ActivityJoin = "ACTIVITY_JOIN";
		internal const string ActivitySpectate = "ACTIVITY_SPECTATE";
		internal const string ActivityJoinRequest = "ACTIVITY_JOIN_REQUEST";
	}
}
