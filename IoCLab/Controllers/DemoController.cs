using System;
using System.Threading;
using IoCLab.Attributes;

namespace IoCLab.Controllers
{
    /// <summary>
    /// "Perfection is achieved, not when there is nothing more to add, but when there is nothing left to take away."
    /// -- Antoine de Saint Exupéry
    /// </summary>
    public class DemoController : IController
    {
        [SlaTimer(100)]
        [RequireAuthentication()]
        public string Execute([NotNull]string test1, string test2, string test3, string test4)
        {
            if (test1 == "985")
                throw new Exception("Oh my!");

            if (test1.StartsWith("timeout="))
            {
                int timeout;
                if (int.TryParse(test1.Substring("timeout=".Length), out timeout))
                    Thread.Sleep(timeout);
            }

            return string.Format("DemoController - Execute, value: {0}", test1);
        }
    }
}