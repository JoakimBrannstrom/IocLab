using System;
using System.Linq;
using Castle.Core.Logging;
using Castle.DynamicProxy;

namespace IoCLab.Aspects
{
    public abstract class AbstractInterceptor
    {
        protected readonly ILogger Logger;

        protected abstract string InterceptorName { get; }

        protected virtual string InterceptionStartMessage
        {
            get { return string.Format("{0} - Invocation intercepted.", InterceptorName); }
        }

        protected virtual string InterceptionFinishedMessage
        {
            get { return string.Format("{0} - Interception finished.", InterceptorName); }
        }

        protected AbstractInterceptor(ILogger logger)
        {
            Logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            Logger.Debug(InterceptionStartMessage);

            MakeInterception(invocation);

            Logger.Debug(InterceptionFinishedMessage);
        }

        protected abstract void MakeInterception(IInvocation invocation);

        protected static T GetAttributeOnInvocationTargetMethod<T>(IInvocation invocation) where T : Attribute
        {
            return invocation
                       .GetConcreteMethodInvocationTarget()
                       .GetCustomAttributes(typeof(T), true)
                       .FirstOrDefault() as T;
        }

        protected static string GetInvocationTargetMethodName(IInvocation invocation)
        {
            return invocation.GetConcreteMethodInvocationTarget().Name;
        }
    }
}