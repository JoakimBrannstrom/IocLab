using System.Security;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using IoCLab.Attributes;
using IoCLab.Providers;

namespace IoCLab.Aspects
{
    public interface IAuthenticationInterceptor : IInterceptor
    {
    }

    public class AuthenticationInterceptor : AbstractInterceptor, IAuthenticationInterceptor
    {
        private readonly IIdentityProvider _identityProvider;

        protected override string InterceptorName
        {
            get { return "AuthenticationInterceptor"; }
        }

        public AuthenticationInterceptor(ILogger logger, IIdentityProvider identityProvider)
            : base(logger)
        {
            _identityProvider = identityProvider;
        }

        protected override void MakeInterception(IInvocation invocation)
        {
            var methodName = GetInvocationTargetMethodName(invocation);
            var authenticationAttribute = GetAttributeOnInvocationTargetMethod<RequireAuthenticationAttribute>(invocation);

            if (authenticationAttribute != null)
            {
                if (_identityProvider.Current == null || !_identityProvider.Current.IsAuthenticated)
                {
                    Logger.ErrorFormat("{0} - Method '{1}' require authentication, user is not authenticated.",
                        InterceptorName, methodName);

                    throw new SecurityException("User is not authenticated.");
                }

                Logger.DebugFormat("{0} - Method '{1}' require authentication, user is authenticated.",
                    InterceptorName, methodName);
            }

            invocation.Proceed();
        }
    }
}
