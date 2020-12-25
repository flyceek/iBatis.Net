using IBatisNet.Common.Utilities.Objects;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Statements
{
	[XmlRoot("selectKey", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class SelectKey : Statement
	{
		[System.NonSerialized]
		private SelectKeyType _selectKeyType = SelectKeyType.post;

		[System.NonSerialized]
		private string _property = string.Empty;

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

		[XmlAttribute("property")]
		public string PropertyName
		{
			get
			{
				return this._property;
			}
			set
			{
				this._property = value;
			}
		}

		[XmlAttribute("type")]
		public SelectKeyType SelectKeyType
		{
			get
			{
				return this._selectKeyType;
			}
			set
			{
				this._selectKeyType = value;
			}
		}

		[XmlIgnore]
		public bool isAfter
		{
			get
			{
				return this._selectKeyType == SelectKeyType.post;
			}
		}

		internal override void Initialize(ConfigurationScope configurationScope)
		{
			if (this.PropertyName.Length > 0)
			{
				IMappedStatement mappedStatement = configurationScope.SqlMapper.GetMappedStatement(base.Id);
				System.Type parameterClass = mappedStatement.Statement.ParameterClass;
				if (parameterClass != null && !ObjectProbe.IsSimpleType(parameterClass))
				{
					configurationScope.ErrorContext.MoreInfo = string.Format("Looking for settable property named '{0}' on type '{1}' for selectKey node of statement id '{2}'.", this.PropertyName, mappedStatement.Statement.ParameterClass.Name, base.Id);
					ReflectionInfo.GetInstance(mappedStatement.Statement.ParameterClass).GetSetter(this.PropertyName);
				}
			}
			base.Initialize(configurationScope);
		}
	}
}
