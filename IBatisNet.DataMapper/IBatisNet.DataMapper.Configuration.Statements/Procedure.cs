using IBatisNet.DataMapper.Scope;
using System;
using System.Data;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	[XmlRoot("procedure", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class Procedure : Statement
	{
		[XmlIgnore]
		public override CommandType CommandType
		{
			get
			{
				return CommandType.StoredProcedure;
			}
		}

		[XmlIgnore]
		public override string ExtendStatement
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		internal override void Initialize(ConfigurationScope configurationScope)
		{
			base.Initialize(configurationScope);
			if (base.ParameterMap == null)
			{
				base.ParameterMap = configurationScope.SqlMapper.GetParameterMap("iBATIS.Empty.ParameterMap");
			}
		}
	}
}
