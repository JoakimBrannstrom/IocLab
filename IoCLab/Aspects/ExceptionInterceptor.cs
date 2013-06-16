using System;
using Castle.Core.Logging;
using Castle.DynamicProxy;

namespace IoCLab.Aspects
{
    public interface IExceptionInterceptor : IInterceptor
    {
    }

    public class ExceptionInterceptor : AbstractInterceptor, IExceptionInterceptor
    {
        protected override string InterceptorName
        {
            get { return "ExceptionInterceptor"; }
        }

        public ExceptionInterceptor(ILogger logger)
            : base(logger)
        {
        }

        protected override void MakeInterception(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (SystemException exception)
            {
                Logger.FatalFormat(exception, "{0} - An exception occurred in {1} on {2}.", InterceptorName, invocation.Method.Name, invocation.TargetType);
                throw;
            }
            catch (ApplicationException exception)
            {
                Logger.WarnFormat(exception, "{0} - An exception occurred in {1} on {2}.", InterceptorName, invocation.Method.Name, invocation.TargetType);
                throw;
            }
            catch (Exception exception)
            {
                Logger.ErrorFormat(exception, "{0} - An exception occurred in {1} on {2}.", InterceptorName, invocation.Method.Name, invocation.TargetType);
                throw;
            }
        }
    }
}
