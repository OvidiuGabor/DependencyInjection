using System;

namespace DependencyInjection
{
    public class Entity
    {
        public delegate Entity Factory();

        private static Random random = new Random();
        private int number;

        public Entity()
        {
            number = random.Next();
        }

        public override string ToString()
        {
            return $" test {number}";
        }
    }
}
