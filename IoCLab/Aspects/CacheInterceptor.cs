using System.Linq;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using IoCLab.Providers;

namespace IoCLab.Aspects
{
    public interface ICacheInterceptor : IInterceptor
    {
    }

    public class CacheInterceptor : AbstractInterceptor, ICacheInterceptor
    {
        private readonly ICacheProvider _cacheProvider;

        protected override string InterceptorName
        {
            get { return "CacheInterceptor"; }
        }

        public CacheInterceptor(ILogger logger, ICacheProvider cacheProvider) : base(logger)
        {
            _cacheProvider = cacheProvider;
        }

        protected override void MakeInterception(IInvocation invocation)
        {
            var cacheKey = GetCacheKey(invocation);
            if(_cacheProvider.HasKey(cacheKey))
            {
                Logger.Debug("CacheInterceptor - Key found in cache, returning cached value.");
                invocation.ReturnValue = _cacheProvider.Get(cacheKey);
            }
            else
            {
                Logger.Debug("CacheInterceptor - Key not found in cache, proceeding with invocation.");
                invocation.Proceed();
                _cacheProvider.Store(cacheKey, invocation.ReturnValue);
            }
        }

        private static string GetCacheKey(IInvocation invocation)
        {
            var fullName = invocation.TargetType.FullName;
            var method = invocation.Method.Name;
            var argumentNames = invocation.GetConcreteMethodInvocationTarget().GetParameters().Select(a => a.Name);
            var argumentValues = string.Join("_", invocation.Arguments);

            return string.Format(
                "{0}_{1}_{2}_{3}", 
                fullName, 
                method,
                string.Join("_", argumentNames), 
                argumentValues);
        }
    }
}
