using System.Security;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using IoCLab.Attributes;
using IoCLab.Providers;

namespace IoCLab.Aspects
{
    public interface IAuthorizationInterceptor : IInterceptor
    {
    }

    public class AuthorizationInterceptor : AbstractInterceptor, IAuthorizationInterceptor
    {
        private readonly IPrincipalProvider _principalProvider;

        protected override string InterceptorName
        {
            get { return "AuthorizationInterceptor"; }
        }

        public AuthorizationInterceptor(ILogger logger, IPrincipalProvider principalProvider)
            : base(logger)
        {
            _principalProvider = principalProvider;
        }

        protected override void MakeInterception(IInvocation invocation)
        {
            var authorizationAttribute = GetAttributeOnInvocationTargetMethod<RequireAuthorizationAttribute>(invocation);
            if (authorizationAttribute != null)
            {
                var methodName = GetInvocationTargetMethodName(invocation);
                if (_principalProvider.IsInRole(authorizationAttribute.Role))
                {
                    Logger.ErrorFormat("{0} - Method '{1}' require authorization, user is not in role '{2}'.",
                        InterceptorName, methodName, authorizationAttribute.Role);

                    throw new SecurityException("User is not authorized.");
                }

                Logger.DebugFormat("{0} - Method '{1}' require authorization, user is in role '{2}'.",
                    InterceptorName, methodName, authorizationAttribute.Role);
            }

            invocation.Proceed();
        }
    }
}
