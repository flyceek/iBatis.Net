using IBatisNet.Common.Exceptions;
using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper.Exceptions;
using IBatisNet.DataMapper.MappedStatements;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace IBatisNet.DataMapper.Configuration.Cache
{
	[XmlRoot("cacheModel", Namespace = "http://ibatis.apache.org/mapping")]
	[System.Serializable]
	public class CacheModel
	{
		[System.NonSerialized]
		public const long NO_FLUSH_INTERVAL = -99999L;

		private static System.Collections.IDictionary _lockMap = new HybridDictionary();

		[System.NonSerialized]
		private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		[System.NonSerialized]
		public static readonly object NULL_OBJECT = new object();

		[System.NonSerialized]
		private object _statLock = new object();

		[System.NonSerialized]
		private int _requests = 0;

		[System.NonSerialized]
		private int _hits = 0;

		[System.NonSerialized]
		private string _id = string.Empty;

		[System.NonSerialized]
		private ICacheController _controller = null;

		[System.NonSerialized]
		private FlushInterval _flushInterval = null;

		[System.NonSerialized]
		private long _lastFlush = 0L;

		[System.NonSerialized]
		private HybridDictionary _properties = new HybridDictionary();

		[System.NonSerialized]
		private string _implementation = string.Empty;

		[System.NonSerialized]
		private bool _isReadOnly = true;

		[System.NonSerialized]
		private bool _isSerializable = false;

		[System.NonSerialized]
		private string _thirdpatyType = string.Empty;

		[XmlAttribute("id")]
		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				if (value == null || value.Length < 1)
				{
					throw new System.ArgumentNullException("The id attribute is mandatory in a cacheModel tag.");
				}
				this._id = value;
			}
		}

		[XmlAttribute("implementation")]
		public string Implementation
		{
			get
			{
				return this._implementation;
			}
			set
			{
				this._implementation = value;
			}
		}

		[XmlAttribute("thirdpatytype")]
		public string ThirdPatyType
		{
			get
			{
				return this._thirdpatyType;
			}
			set
			{
				this._thirdpatyType = value;
			}
		}

		public ICacheController CacheController
		{
			set
			{
				this._controller = value;
			}
		}

		[XmlElement("flushInterval", typeof(FlushInterval))]
		public FlushInterval FlushInterval
		{
			get
			{
				return this._flushInterval;
			}
			set
			{
				this._flushInterval = value;
			}
		}

		public bool IsSerializable
		{
			get
			{
				return this._isSerializable;
			}
			set
			{
				this._isSerializable = value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this._isReadOnly;
			}
			set
			{
				this._isReadOnly = value;
			}
		}

		public object this[CacheKey key]
		{
			get
			{
				lock (this)
				{
					if (this._lastFlush != -99999L && System.DateTime.Now.Ticks - this._lastFlush > this._flushInterval.Interval)
					{
						this.Flush();
					}
				}
				object obj = null;
				lock (this.GetLock(key))
				{
					obj = this._controller[key];
				}
				if (this._isSerializable && !this._isReadOnly && obj != CacheModel.NULL_OBJECT && obj != null)
				{
					try
					{
						System.IO.MemoryStream memoryStream = new System.IO.MemoryStream((byte[])obj);
						memoryStream.Position = 0L;
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
						obj = binaryFormatter.Deserialize(memoryStream);
					}
					catch (System.Exception ex)
					{
						throw new IBatisNetException("Error caching serializable object.  Be sure you're not attempting to use a serialized cache for an object that may be taking advantage of lazy loading.  Cause: " + ex.Message, ex);
					}
				}
				lock (this._statLock)
				{
					this._requests++;
					if (obj != null)
					{
						this._hits++;
					}
				}
				if (CacheModel._logger.IsDebugEnabled)
				{
					if (obj != null)
					{
						CacheModel._logger.Debug(string.Format("Retrieved cached object '{0}' using key '{1}' ", obj, key));
					}
					else
					{
						CacheModel._logger.Debug(string.Format("Cache miss using key '{0}' ", key));
					}
				}
				return obj;
			}
			set
			{
				if (null == value)
				{
					value = CacheModel.NULL_OBJECT;
				}
				if (this._isSerializable && !this._isReadOnly && value != CacheModel.NULL_OBJECT)
				{
					try
					{
						System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
						System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
						binaryFormatter.Serialize(memoryStream, value);
						value = memoryStream.ToArray();
					}
					catch (System.Exception ex)
					{
						throw new IBatisNetException("Error caching serializable object. Cause: " + ex.Message, ex);
					}
				}
				this._controller[key] = value;
				if (CacheModel._logger.IsDebugEnabled)
				{
					CacheModel._logger.Debug(string.Format("Cache object '{0}' using key '{1}' ", value, key));
				}
			}
		}

		public double HitRatio
		{
			get
			{
				double result;
				if (this._requests != 0)
				{
					result = (double)this._hits / (double)this._requests;
				}
				else
				{
					result = 0.0;
				}
				return result;
			}
		}

		public CacheModel()
		{
			this._lastFlush = System.DateTime.Now.Ticks;
		}

		public void Initialize()
		{
			if (this._flushInterval == null)
			{
				this._flushInterval = new FlushInterval();
			}
			this._flushInterval.Initialize();
			try
			{
				if (this._thirdpatyType != null && this._thirdpatyType.Trim().Length > 0)
				{
					System.Type type = TypeUtils.GetTypeByString(this._thirdpatyType);
					object[] args = new object[0];
					this._controller = (ICacheController)System.Activator.CreateInstance(type, args);
				}
				else
				{
					if (this._implementation == null || this._implementation.Trim().Length <= 0)
					{
						throw new DataMapperException(string.Concat(new string[]
						{
							"Error instantiating cache controller for cache named '",
							this._id,
							"'. Cause: The class for name '",
							this._implementation,
							"' could not be found."
						}));
					}
					System.Type type = TypeUtils.ResolveType(this._implementation);
					object[] args = new object[0];
					this._controller = (ICacheController)System.Activator.CreateInstance(type, args);
				}
			}
			catch (System.Exception ex)
			{
				throw new ConfigurationException("Error instantiating cache controller for cache named '" + this._id + ". Cause: " + ex.Message, ex);
			}
			try
			{
				this._controller.Configure(this._properties);
			}
			catch (System.Exception ex)
			{
				throw new ConfigurationException("Error configuring controller named '" + this._id + "'. Cause: " + ex.Message, ex);
			}
		}

		public void RegisterTriggerStatement(IMappedStatement mappedStatement)
		{
			mappedStatement.Execute += new ExecuteEventHandler(this.FlushHandler);
		}

		private void FlushHandler(object sender, ExecuteEventArgs e)
		{
			if (CacheModel._logger.IsDebugEnabled)
			{
				CacheModel._logger.Debug(string.Concat(new string[]
				{
					"Flush cacheModel named ",
					this._id,
					" for statement '",
					e.StatementName,
					"'"
				}));
			}
			this.Flush();
		}

		public void Flush()
		{
			this._lastFlush = System.DateTime.Now.Ticks;
			this._controller.Flush();
		}

		public void AddProperty(string name, string value)
		{
			this._properties.Add(name, value);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		public object GetLock(CacheKey key)
		{
			int identityHashCode = HashCodeProvider.GetIdentityHashCode(this._controller);
			int hashCode = key.GetHashCode();
			int num = 29 * identityHashCode + hashCode;
			object obj = CacheModel._lockMap[num];
			if (obj == null)
			{
				obj = num;
				CacheModel._lockMap[num] = obj;
			}
			return obj;
		}
	}
}
