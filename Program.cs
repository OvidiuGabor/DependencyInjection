using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public interface IConsole
    {

    }

    class Program
    {
        static void Main(string[] args)
        {

            //Standar Version, the hard way!
            #region Standard way
            //var log = new ConsoleLog();
            //var engine = new Engine(log);
            //var car = new Car(engine, log);
            //car.Go();
            #endregion

            var builder = new ContainerBuilder();
            //builder.RegisterType<EmailLog>().As<ILog>().AsSelf(); //in order to get the actual ConsoleLog class and be able to use as it is, need to add AsSelf at the end, so that it will be registered as the interface but also as the class itself.
            //builder.registertype<emaillog>()
            //    .as< ilog > ()
            //    .as< iconsole > ();
            builder.RegisterType<ConsoleLog>().As<ILog>();


            //If you need the container to use a specific instance of a component;
            //You can create it and then supply it tot he builder
            //And that componenet will be use in all other component that are depedent of it
            //you also need to spcify of which type it is, and make sure it is the type that is required by other componenets.
            //var log = new ConsoleLog();
            //builder.RegisterInstance(log).As<ILog>();

            //if you have multiple componenets that are using the same depedency/interface
            //the the last one you register will be the one used accros all container
            //but you can specify that you want to use the previous one, by using PreserveExistingDefault();
            //builder.RegisterType<ConsoleLog>().As<ILog>().PreserveExistingDefaults();
            //builder.RegisterType<Engine>();


            //in case you need to createa new component with a specific data
            //you can use lanbda expresion in order to do it.
            //also if you have multiple dependency, you can still use the container to get the ones that are already registered
            //and you just specify the ones you need manually;
            builder.Register(c => new Engine(c.Resolve<ILog>(), 123));


            //If there are multiple constructor for the same class, you can specify which one to use
            //By providing the type of dependency that they need in the respective constructor
            //builder.RegisterType<Car>().UsingConstructor(typeof(Engine));
            builder.RegisterType<Car>();

            IContainer container = builder.Build();

            var car1 = container.Resolve<Car>();
            car1.Go();



            //IList<t> --> List<T>
            //IList<int> --> List<int>
            //You can also register generics type to the container and you will get back the actual type 
            var genericBuilder = new ContainerBuilder();
            genericBuilder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>));
            IContainer genericContainer = genericBuilder.Build();

            var myList = genericContainer.Resolve<IList<int>>();

            Console.WriteLine(myList.GetType().Name);


            Console.Read();
        }
    }
}
