using System;

namespace DependencyInjection
{
    public class Engine
    {
        private ILog log;
        private int id;
        public Engine(ILog log)
        {
            this.log = log;
            id = new Random().Next();
        }

        public Engine(ILog log, int id)
        {
            this.log = log;
            this.id = id;
        }

        public void Ahead(int power)
        {
            log.Write($"Engine [{id}] ahead {power}");
        }


    }
}
