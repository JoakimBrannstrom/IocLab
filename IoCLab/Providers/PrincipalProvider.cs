using System.Threading;

namespace IoCLab.Providers
{
    public interface IPrincipalProvider
    {
        bool IsInRole(string role);
    }

    public class PrincipalProvider : IPrincipalProvider
    {
        public bool IsInRole(string role)
        {
            return Thread.CurrentPrincipal.IsInRole(role);
        }
    }
}
