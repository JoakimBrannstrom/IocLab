using System.Text;
using IoCLab.Controllers;

namespace Controllers
{
    public class ExtensionController : IController
    {
        public string Execute(string value1, string value2, string value3, string value4)
        {
            var sb = new StringBuilder();
            sb.AppendLine("************************");
            sb.AppendLine("***** Sweet stuff! *****");
            sb.AppendLine("************************");
            return sb.ToString();
        }
    }
}
