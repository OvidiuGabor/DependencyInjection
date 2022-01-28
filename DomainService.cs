namespace DependencyInjection
{
    partial class Program
    {
        public class DomainService
        {
            private Service service;
            private int value;

            public delegate DomainService Factory(int value);

            public DomainService(Service service, int value)
            {
                this.service = service;
                this.value = value;
            }

            public override string ToString()
            {
                return service.DoSomething(value);
            }
        }
    }
}
