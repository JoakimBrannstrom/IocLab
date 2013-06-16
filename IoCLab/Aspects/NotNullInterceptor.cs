using System;
using System.Linq;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using IoCLab.Attributes;

namespace IoCLab.Aspects
{
    public interface INotNullInterceptor : IInterceptor
    {
    }

    public class NotNullInterceptor : AbstractInterceptor, INotNullInterceptor
    {
        protected override string InterceptorName
        {
            get { return "NotNullInterceptor"; }
        }

        public NotNullInterceptor(ILogger logger)
            : base(logger)
        {
        }

        protected override void MakeInterception(IInvocation invocation)
        {
            var args = invocation.GetConcreteMethodInvocationTarget().GetParameters();

            var failedArgs = args
                                .Where(a =>
                                       a.IsDefined(typeof (NotNullAttribute), true) &&
                                       (invocation.Arguments[a.Position] == null));

            if (failedArgs.Any())
            {
                Logger.DebugFormat(
                    "{0} - Argument(s) that failed NotNull-inspection: {1}",
                    InterceptorName,
                    string.Join(", ", failedArgs.Select(a => a.Name)));

                throw new ArgumentNullException(failedArgs.First().Name);
            }

            Logger.DebugFormat("{0} - Arguments passed NotNull-inspection.", InterceptorName);

            invocation.Proceed();
        }
    }
}