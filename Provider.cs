using System;

namespace DependencyInjection
{
    public class Provider
    {
        private readonly Entity.Factory entityFactory;
        public Provider(Entity.Factory entityFactory)
        {
            this.entityFactory = entityFactory;
        }



        public void Method()
        {
            var entity = entityFactory();
            Console.WriteLine(entity);

        }
    }
}
