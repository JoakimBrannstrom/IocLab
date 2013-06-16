using System.Linq;
using Castle.Core.Logging;
using Castle.DynamicProxy;

namespace IoCLab.Aspects
{
    public interface ILogInterceptor : IInterceptor
    {
    }

    public class LogInterceptor : AbstractInterceptor, ILogInterceptor
    {
        protected override string InterceptorName
        {
            get { return "LogInterceptor"; }
        }

        public LogInterceptor(ILogger logger)
            : base(logger)
        {
        }

        protected override void MakeInterception(IInvocation invocation)
        {
            var parameters = invocation.GetConcreteMethodInvocationTarget().GetParameters().Select(a => a.Name);

            Logger.DebugFormat("{0} - TargetType: {1}", InterceptorName, invocation.TargetType);
            Logger.DebugFormat("{0} - Method.Name: {1}", InterceptorName, invocation.Method.Name);
            Logger.DebugFormat("{0} - Parameters: {1}", InterceptorName, string.Join(", ", parameters));

            invocation.Proceed();
        }
    }
}