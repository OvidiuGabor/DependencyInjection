namespace DependencyInjection
{
    public class Car
    {
        private Engine engine;
        private ILog log;

        public Car(Engine engine, ILog log)
        {
            this.log = log;
            this.engine = engine;
        }

        public Car(Engine engine)
        {
            this.engine = engine;
            log = new EmailLog();
        }
        public void Go()
        {
            engine.Ahead(100);
            log.Write("Car going forward.");
        }
    }
}
