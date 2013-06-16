using IoCLab.Controllers;

namespace MoreControllers
{
    public class AnotherController : IController
    {
        public string Execute(string value1, string value2, string value3, string value4)
        {
            return "Yet another controller...";
        }
    }
}
