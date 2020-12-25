using Castle.DynamicProxy;
using IBatisNet.Common.Logging;
using IBatisNet.Common.Utilities.Objects.Members;
using IBatisNet.DataMapper.MappedStatements;
using System;
using System.Collections;
using System.Reflection;

namespace IBatisNet.DataMapper.Proxy
{
	[System.Serializable]
	internal class LazyLoadInterceptor : IInterceptor
	{
		private object _param = null;

		private object _target = null;

		private ISetAccessor _setAccessor = null;

		private ISqlMapper _sqlMap = null;

		private string _statementName = string.Empty;

		private bool _loaded = false;

		private object _lazyLoadedItem = null;

		private object _loadLock = new object();

		private static System.Collections.ArrayList _passthroughMethods;

		private static readonly ILog _logger;

		static LazyLoadInterceptor()
		{
			LazyLoadInterceptor._passthroughMethods = new System.Collections.ArrayList();
			LazyLoadInterceptor._logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			LazyLoadInterceptor._passthroughMethods.Add("GetType");
		}

		internal LazyLoadInterceptor(IMappedStatement mappedSatement, object param, object target, ISetAccessor setAccessor)
		{
			this._param = param;
			this._statementName = mappedSatement.Id;
			this._sqlMap = mappedSatement.SqlMap;
			this._target = target;
			this._setAccessor = setAccessor;
		}

		public object Intercept(IInvocation invocation, params object[] arguments)
		{
			if (LazyLoadInterceptor._logger.IsDebugEnabled)
			{
				LazyLoadInterceptor._logger.Debug("Proxyfying call to " + invocation.Method.Name);
			}
			lock (this._loadLock)
			{
				if (!this._loaded && !LazyLoadInterceptor._passthroughMethods.Contains(invocation.Method.Name))
				{
					if (LazyLoadInterceptor._logger.IsDebugEnabled)
					{
						LazyLoadInterceptor._logger.Debug("Proxyfying call, query statement " + this._statementName);
					}
					if (typeof(System.Collections.IList).IsAssignableFrom(this._setAccessor.MemberType))
					{
						this._lazyLoadedItem = this._sqlMap.QueryForList(this._statementName, this._param);
					}
					else
					{
						this._lazyLoadedItem = this._sqlMap.QueryForObject(this._statementName, this._param);
					}
					this._loaded = true;
					this._setAccessor.Set(this._target, this._lazyLoadedItem);
				}
			}
			object result = invocation.Method.Invoke(this._lazyLoadedItem, arguments);
			if (LazyLoadInterceptor._logger.IsDebugEnabled)
			{
				LazyLoadInterceptor._logger.Debug("End of proxyfied call to " + invocation.Method.Name);
			}
			return result;
		}
	}
}
