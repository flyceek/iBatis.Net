using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Cache
{
	[XmlRoot("flushInterval")]
	[System.Serializable]
	public class FlushInterval
	{
		private int _hours = 0;

		private int _minutes = 0;

		private int _seconds = 0;

		private int _milliseconds = 0;

		private long _interval = -99999L;

		[XmlAttribute("hours")]
		public int Hours
		{
			get
			{
				return this._hours;
			}
			set
			{
				this._hours = value;
			}
		}

		[XmlAttribute("minutes")]
		public int Minutes
		{
			get
			{
				return this._minutes;
			}
			set
			{
				this._minutes = value;
			}
		}

		[XmlAttribute("seconds")]
		public int Seconds
		{
			get
			{
				return this._seconds;
			}
			set
			{
				this._seconds = value;
			}
		}

		[XmlAttribute("milliseconds")]
		public int Milliseconds
		{
			get
			{
				return this._milliseconds;
			}
			set
			{
				this._milliseconds = value;
			}
		}

		[XmlIgnore]
		public long Interval
		{
			get
			{
				return this._interval;
			}
		}

		public void Initialize()
		{
			long num = 0L;
			if (this._milliseconds != 0)
			{
				num += (long)this._milliseconds * 10000L;
			}
			if (this._seconds != 0)
			{
				num += (long)this._seconds * 10000000L;
			}
			if (this._minutes != 0)
			{
				num += (long)this._minutes * 600000000L;
			}
			if (this._hours != 0)
			{
				num += (long)this._hours * 36000000000L;
			}
			if (num == 0L)
			{
				num = -99999L;
			}
			this._interval = num;
		}
	}
}
