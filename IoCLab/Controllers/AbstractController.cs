namespace IoCLab.Controllers
{
    public abstract class AbstractController : IController
    {
        public abstract string Execute(string value1, string value2, string value3, string value4);
    }
}
