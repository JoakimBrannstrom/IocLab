using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Castle.Core;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using IoCLab.Aspects;
using IoCLab.Controllers;

namespace IoCLab
{
    #region TODO
    // * Add support for handling "DB-lost-scenarios", using IHandlerSelector. http://ayende.com/Blog/archive/2008/10/05/windsor-ihandlerselector.aspx
    #endregion

    internal class Program
    {
        #region Members
        private static readonly StringBuilder RegistredComponents = new StringBuilder();
        private static readonly IWindsorContainer Container = new WindsorContainer();
        #endregion

        private static void Main(string[] args)
        {
            RegisterComponents(LoggerLevel.Debug);

            RegisterPlugins();

            Console.Write(RegistredComponents.ToString());

            RunManual();

            // RunPerformanceTest();

            Console.ReadLine();
        }

        private static void RegisterPlugins()
        {
            var pluginsDir = Directory.GetCurrentDirectory() + "\\Extensions";
            if (!Directory.Exists(pluginsDir))
                Directory.CreateDirectory(pluginsDir);

            var watcher = new FileSystemWatcher(pluginsDir, "*.dll");
            watcher.Created += (sender, eventArgs) => RegisterExtensionAssembly(eventArgs.Name);
            watcher.Deleted += (sender, eventArgs) => UnregisterExtensionAssembly(eventArgs.Name);
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName;
        }

        private static void UnregisterExtensionAssembly(string filename)
        {
            Container.Kernel.RemoveComponent("ExtensionController");
        }

        private static void RegisterExtensionAssembly(string filename)
        {
            var registredControllers = new List<string>();
            Container.Register(

                AllTypes // Register IController:s
                    .FromAssemblyInDirectory(new AssemblyFilter("Extensions"))
                    .BasedOn<IController>()
                    .WithService.FirstInterface()
                    .Configure(configurer =>
                    {
                        if (!typeof(IController).IsAssignableFrom(configurer.Implementation))
                            return;

                        var name = configurer.Implementation.Name;

                        registredControllers.Add(name);

                        configurer
                            .Named(name)

                            .Interceptors( // The following interceptors should run first
                            new InterceptorReference(typeof(IAuthenticationInterceptor)),
                            new InterceptorReference(typeof(IAuthorizationInterceptor)),
                            new InterceptorReference(typeof(ISlaTimerInterceptor)),
                            new InterceptorReference(typeof(ILogInterceptor)),
                            new InterceptorReference(typeof(IExceptionInterceptor))
                            ).First
                            .Interceptors( // The following interceptors can run anywhere
                            new InterceptorReference(typeof(INotNullInterceptor))
                            ).Anywhere
                            .Interceptors( // The following interceptors should run last
                            new InterceptorReference(typeof(ICacheInterceptor))
                            ).Last

                            .LifeStyle
                            .Is(LifestyleType.Transient);
                    })
                );

            Console.Clear();
            Console.Write("Registred new controller(s): " + string.Join(", ", registredControllers));
        }

        private static void RunPerformanceTest()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //var controller = new DemoController();            // numberOfCalls = 1000000 => Elapsed time: 00:00:01.7913054
            //var controller = Container.Resolve<IController>();  // numberOfCalls = 10000 => Elapsed time: 00:00:01.9428946

            const int numberOfCalls = 10000;
            for (var i = 0; i < numberOfCalls; i++)
            {
                try
                {
                    var controller = Container.Resolve<IController>();  // numberOfCalls = 10000; => Elapsed time: 00:00:05.6845929
                    var result = controller.Execute(i.ToString(), "param2", "param3", "param4");
                    //Console.WriteLine(result);
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception hidden...");
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Elapsed time: {0}", stopWatch.Elapsed);
        }

        private static void RunManual()
        {
	        var controller = Container.Resolve<IController>();

            while (true)
            {
                Console.WriteLine("Press <enter> to continue.");
                Console.ReadLine();
                Console.Clear();

                Console.WriteLine("Type in your controller argument:");
                var controllerArg = Console.ReadLine();

                try
                {
                    #region Argument-handling
                    if (controllerArg == "exit")
                        break;
                    if (controllerArg == "null")
                        controllerArg = null;
                    else if (controllerArg.StartsWith("c="))
                    {
                        var key = controllerArg.Substring("c=".Length);
                        controller = Container.Resolve<IController>(key);
                        Console.WriteLine("Resolved new controller named: " + key);
                        continue;
                    }
                    #endregion

                    var result = controller.Execute(controllerArg, "param2", "param3", "param4");

                    Console.WriteLine(Environment.NewLine + result + Environment.NewLine);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }

            Console.WriteLine("Bye!");
        }

        private static void RegisterComponents(LoggerLevel loggerLevel)
        {
            SetupComponentRegistrationEvent();

            RegisterLoggers(loggerLevel);

            RegisterProviders();

            RegisterAspects();

            RegisterControllers();
        }

        private static void RegisterControllers()
        {
            Container.Register(
                AllTypes
                    .FromThisAssembly()
                    .BasedOn<IController>()
                    .WithService.FirstInterface()
                    .Configure(configurer =>
                                   {
                                       if (!typeof(IController).IsAssignableFrom(configurer.Implementation))
                                           return;

                                       var name = configurer.Implementation.Name;
                                       configurer
                                           .Named(name)

                                           .Interceptors( // The following interceptors should run first
                                            new InterceptorReference(typeof(IAuthenticationInterceptor)),
                                            new InterceptorReference(typeof(IAuthorizationInterceptor)),
                                            new InterceptorReference(typeof(ISlaTimerInterceptor)),
                                            new InterceptorReference(typeof(ILogInterceptor)),
                                            new InterceptorReference(typeof(IExceptionInterceptor))
                                           ).First
                                           .Interceptors( // The following interceptors can run anywhere
                                           new InterceptorReference(typeof(INotNullInterceptor))
                                           ).Anywhere
                                           .Interceptors( // The following interceptors should run last
                                           new InterceptorReference(typeof(ICacheInterceptor))
                                           ).Last

                                           .LifeStyle
                                           .Is(LifestyleType.Transient);
                                   })
                );
        }

        private static void RegisterAspects()
        {
            Container.Register(
                AllTypes // Register IInterceptor:s
                    .FromThisAssembly()
                    .BasedOn<IInterceptor>()
                    .WithService.FirstInterface()
                    .Configure(configurer => configurer.LifeStyle.Is(LifestyleType.Transient))
                );
        }

        private static void RegisterProviders()
        {
            Container.Register(
                AllTypes
                .FromThisAssembly()
                .Where(t => t.Name.EndsWith("Provider"))
                .WithService.FirstInterface()
                .Configure(c => c
                    .LifeStyle.Is(LifestyleType.Singleton))
                );
        }

        private static void RegisterLoggers(LoggerLevel loggerLevel)
        {
            Container.Register(
                Component.For(typeof(ILogger))
                    .ImplementedBy(typeof(ConsoleLogger))
                    .Parameters(Parameter.ForKey("loglevel").Eq(loggerLevel.ToString()))
                    .LifeStyle.Is(LifestyleType.Singleton));
        }

        private static void SetupComponentRegistrationEvent()
        {
            Container.Kernel.ComponentRegistered +=
                (key, handler) =>
                RegistredComponents
                    .AppendFormat("{0}Castle Registered component{0}Key:{1}{0}Service: {2}{0}Implementation: {3}{0}",
                                  Environment.NewLine, key, handler.Service, handler.ComponentModel.Implementation);
        }
    }
}
