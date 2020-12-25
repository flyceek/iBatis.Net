using System;
using System.Text;

namespace IBatisNet.DataMapper.Scope
{
	public class ErrorContext
	{
		private string _resource = string.Empty;

		private string _activity = string.Empty;

		private string _objectId = string.Empty;

		private string _moreInfo = string.Empty;

		public string Resource
		{
			get
			{
				return this._resource;
			}
			set
			{
				this._resource = value;
			}
		}

		public string Activity
		{
			get
			{
				return this._activity;
			}
			set
			{
				this._activity = value;
			}
		}

		public string ObjectId
		{
			get
			{
				return this._objectId;
			}
			set
			{
				this._objectId = value;
			}
		}

		public string MoreInfo
		{
			get
			{
				return this._moreInfo;
			}
			set
			{
				this._moreInfo = value;
			}
		}

		public void Reset()
		{
			this._resource = string.Empty;
			this._activity = string.Empty;
			this._objectId = string.Empty;
			this._moreInfo = string.Empty;
		}

		public override string ToString()
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			if (this._activity != null && this._activity.Length > 0)
			{
				stringBuilder.Append(System.Environment.NewLine);
				stringBuilder.Append("- The error occurred while ");
				stringBuilder.Append(this._activity);
				stringBuilder.Append(".");
			}
			if (this._moreInfo != null && this._moreInfo.Length > 0)
			{
				stringBuilder.Append(System.Environment.NewLine);
				stringBuilder.Append("- ");
				stringBuilder.Append(this._moreInfo);
			}
			if (this._resource != null && this._resource.Length > 0)
			{
				stringBuilder.Append(System.Environment.NewLine);
				stringBuilder.Append("- The error occurred in ");
				stringBuilder.Append(this._resource);
				stringBuilder.Append(".");
			}
			if (this._objectId != null && this._objectId.Length > 0)
			{
				stringBuilder.Append("  ");
				stringBuilder.Append(System.Environment.NewLine);
				stringBuilder.Append("- Check the ");
				stringBuilder.Append(this._objectId);
				stringBuilder.Append(".");
			}
			return stringBuilder.ToString();
		}
	}
}
