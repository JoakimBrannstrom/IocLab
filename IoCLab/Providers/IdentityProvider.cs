using System.Security.Principal;
using System.Threading;

namespace IoCLab.Providers
{
    public interface IIdentityProvider
    {
        IIdentity Current { get; }
    }

    public class IdentityProvider : IIdentityProvider
    {
        public IIdentity Current
        {
            get { return Thread.CurrentPrincipal.Identity; }
        }
    }
}
