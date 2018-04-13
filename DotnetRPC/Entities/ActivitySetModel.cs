using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetRPC.Entities
{
    public class ActivitySetModel
    {
		internal ActivitySetModel() { }

		public string State = "";

		public string Details = "";

		public Time StartUnix = 0;

		public Time EndUnix = 0;

		public string LargeImage = "";

		public string LargeImageText = "";

		public string SmallImage = "";

		public string SmallImageText = "";

		public string PartyId = "";

		public int MaxPartySize = 0;

		public int CurrentPartySize = 0;

		public string JoinSecret = "";

		public string MatchSecret = "";

		public string SpectateSecret = "";

		public RpcActivityType? Type { get; internal set; } = null;
    }

	public class Time
	{
		internal int _unixvalue = 0;

		internal Time(int unixvalue)
		{
			this._unixvalue = unixvalue;
		}

		public static implicit operator Time(int value)
		{
			return new Time(value);
		}

		public static implicit operator Time(DateTime value)
		{
			DateTimeOffset converted = new DateTimeOffset(value);
			return new Time((int)converted.ToUnixTimeSeconds());
		}

		public static implicit operator Time(DateTimeOffset value)
		{
			return new Time((int)value.ToUnixTimeSeconds());
		}

		public static implicit operator int(Time unix)
		{
			return unix._unixvalue;
		}
	}
}
