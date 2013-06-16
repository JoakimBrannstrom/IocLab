using System;
using System.Diagnostics;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using IoCLab.Attributes;

namespace IoCLab.Aspects
{
    public interface ISlaTimerInterceptor : IInterceptor
    {
    }

    public class SlaTimerTimerInterceptor : AbstractInterceptor, ISlaTimerInterceptor
    {
        protected override string InterceptorName
        {
            get { return "SlaTimerTimerInterceptor"; }
        }

        public SlaTimerTimerInterceptor(ILogger logger)
            : base(logger)
        {
        }

        protected override void MakeInterception(IInvocation invocation)
        {
            var stopWatch = new Stopwatch();
            var maxTime = TimeSpan.Zero;

            try
            {
                stopWatch.Start();
                if (maxTime > TimeSpan.Zero)
                    Logger.InfoFormat("{0} - Timer started..", InterceptorName);

                var timerAttribute = GetAttributeOnInvocationTargetMethod<SlaTimerAttribute>(invocation);
                if (timerAttribute != null)
                    maxTime = timerAttribute.MaxTime;

                invocation.Proceed();
            }
            finally
            {
                stopWatch.Stop();

                LogExecutionTime(stopWatch.Elapsed, maxTime);
            }
        }

        private void LogExecutionTime(TimeSpan elapsedTime, TimeSpan maxTime)
        {
            var timeMessage = string.Format("Execution time was: {0}", elapsedTime);
            if (maxTime > TimeSpan.Zero)
            {
                var slaMessage = string.Format("SLA agreement MaxTime: {0}", maxTime);
                var message = string.Format("{0}\t{1}{0}\t{2}", Environment.NewLine, slaMessage, timeMessage);
                if (elapsedTime > maxTime)
                    Logger.WarnFormat("{0} - SLA agreement failed!{1}", InterceptorName, message);
                else
                    Logger.InfoFormat("{0} - SLA agreement passed.{1}", InterceptorName, message);
            }
            else
                Logger.DebugFormat("{0} - {1}", InterceptorName, timeMessage);
        }
    }
}