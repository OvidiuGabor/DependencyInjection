using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{



    partial class Program
    {

        public class Parent
        {
            public override string ToString()
            {
                return "I am your father!";
            }
        }

        public class Child
        {
            public string name { get; set; }
            public Parent parent { get; set; }



            public void SetParent (Parent parent)
            {
                this.parent = parent;
            }
        }



        public class ParentChildModule : Autofac.Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<Parent>();
                builder.Register(x => new Child() { parent = x.Resolve<Parent>() });
            }
        }

        static void Main(string[] args)
        {

            var builder = new ContainerBuilder();

            //Parameters passed at Register Time


            //passing named parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter("phoneNumber", "+0728453839");


            //passing typed parameter
            //builder.RegisterType<SMSLog>()
            //    .As<ILog>()
            //    .WithParameter(new TypedParameter(typeof(string), "+0728453839"));


            //Resolved parameter
            //builder.RegisterType<SMSLog>()
            //    .As<ILog>()
            //    .WithParameter(
            //        new ResolvedParameter(
            //            //predicate => the paramter we are searching for
            //            (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
            //            //Value accessor => seting up the value to the parameter
            //            (pi, ctx) => "+07284539"
            //            )
            //    );


            //parameter passed at resolution time
            Random random = new Random();
            builder.Register((c, p) => new SMSLog(p.Named<string>("phoneNumber")))
                .As<ILog>();


            Console.WriteLine("About to build the container");

            var container = builder.Build();

            var sms = container.Resolve<ILog>(new NamedParameter("phoneNumber", random.Next().ToString()));
            sms.Write("Something");




            //Delegate Factories

            var cb = new ContainerBuilder();
            cb.RegisterType<Service>();
            cb.RegisterType<DomainService>();

            var container1 = cb.Build();

            //this option work only if the paramter position are not changed in the Constroctor, and also if there are no multiple paramters.
            var dom = container1.Resolve<DomainService>(new PositionalParameter(1, 432));

            Console.WriteLine(dom.ToString());

            //Using delegates to resolve a dependency when you have paramters in the constroctor that are not resolve by the IOC, and the number of parameter and position can change.
            var factory = container1.Resolve<DomainService.Factory>();
            DomainService dobj2 = factory(543);
            Console.WriteLine(dobj2);


            //Objects on Demand
            var cb2 = new ContainerBuilder();
            cb2.RegisterType<Entity>().InstancePerDependency();
            cb2.RegisterType<Provider>();

            var container2 = cb2.Build();
            var vm = container2.Resolve<Provider>();
            vm.Method();
            vm.Method();



            //property and method injection, when no constructor is specified!
            var cb3 = new ContainerBuilder();
            cb3.RegisterType<Parent>();
            //cb3.RegisterType<Child>().PropertiesAutowired(); //the container will automatically search for any parameters that are associated with the ones registerd
            //cb3.RegisterType<Child>().WithProperty("parent", new Parent());
            //cb3.Register(
            //    c =>
            //    {
            //        var child = new Child();
            //        child.SetParent(c.Resolve<Parent>());
            //        return child;
            //    }
            //    );

            cb3.RegisterType<Child>()
                .OnActivated(e =>
                {
                    Parent p = e.Context.Resolve<Parent>();
                    e.Instance.SetParent(p);
                });
          


            var container3 = cb3.Build();
            var parent = container3.Resolve<Child>().parent;
            Console.WriteLine(parent);




            //Scanning for types
            //where there are multiple dependency we can scan for the assembly and aregister them all at once, or we can filtger them.

            var assembly = Assembly.GetExecutingAssembly();

            var cb4 = new ContainerBuilder();
            cb4.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Log"))
                .Except<SMSLog>()
                .Except<ConsoleLog>(c => c.As<ILog>().SingleInstance())
                .AsSelf();

            cb4.RegisterAssemblyTypes(assembly)
                .Except<SMSLog>()
                .Except<ConsoleLog>()
                .Where(t => t.Name.EndsWith("Log"))
                .As(t => t.GetInterfaces()[0]);



            //scanning for module
            var cb5 = new ContainerBuilder();
            cb5.RegisterAssemblyModules(typeof(Program).Assembly);
            cb5.RegisterAssemblyModules<ParentChildModule>(typeof(Program).Assembly);

            var container5 = cb5.Build();

            var parent5 = container5.Resolve<Child>().parent;
            Console.WriteLine($"parent 5 {parent5} ");



            Console.Read();

        }


        public class Lecture1_Registration_Concepts
        {
            ////Standar Version, the hard way!
            //#region Standard way
            ////var log = new ConsoleLog();
            ////var engine = new Engine(log);
            ////var car = new Car(engine, log);
            ////car.Go();
            //#endregion

            //var builder = new ContainerBuilder();
            ////builder.RegisterType<EmailLog>().As<ILog>().AsSelf(); //in order to get the actual ConsoleLog class and be able to use as it is, need to add AsSelf at the end, so that it will be registered as the interface but also as the class itself.
            ////builder.registertype<emaillog>()
            ////    .as< ilog > ()
            ////    .as< iconsole > ();
            //builder.RegisterType<ConsoleLog>().As<ILog>();


            ////If you need the container to use a specific instance of a component;
            ////You can create it and then supply it tot he builder
            ////And that componenet will be use in all other component that are depedent of it
            ////you also need to spcify of which type it is, and make sure it is the type that is required by other componenets.
            ////var log = new ConsoleLog();
            ////builder.RegisterInstance(log).As<ILog>();

            ////if you have multiple componenets that are using the same depedency/interface
            ////the the last one you register will be the one used accros all container
            ////but you can specify that you want to use the previous one, by using PreserveExistingDefault();
            ////builder.RegisterType<ConsoleLog>().As<ILog>().PreserveExistingDefaults();
            ////builder.RegisterType<Engine>();


            ////in case you need to createa new component with a specific data
            ////you can use lanbda expresion in order to do it.
            ////also if you have multiple dependency, you can still use the container to get the ones that are already registered
            ////and you just specify the ones you need manually;
            //builder.Register(c => new Engine(c.Resolve<ILog>(), 123));


            ////If there are multiple constructor for the same class, you can specify which one to use
            ////By providing the type of dependency that they need in the respective constructor
            ////builder.RegisterType<Car>().UsingConstructor(typeof(Engine));
            //builder.RegisterType<Car>();

            //IContainer container = builder.Build();

            //var car1 = container.Resolve<Car>();
            //car1.Go();



            ////IList<t> --> List<T>
            ////IList<int> --> List<int>
            ////You can also register generics type to the container and you will get back the actual type 
            //var genericBuilder = new ContainerBuilder();
            //genericBuilder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>));
            //IContainer genericContainer = genericBuilder.Build();

            //var myList = genericContainer.Resolve<IList<int>>();

            //Console.WriteLine(myList.GetType().Name);


            //Console.Read();
        }
    }
}
