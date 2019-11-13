using System;
using System.Configuration;
using System.Text;
using System.Threading;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SendAndReceiveWithWorker
{
    class Program
    {
        
        //  private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly int NumberOfThreads = int.Parse(ConfigurationManager.AppSettings["numberOfThreads"]);

        static void Main()
        {

        Thread th1 = new Thread(Start);
         th1.Start();

            Thread[] threads = new Thread[NumberOfThreads];

            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(Worker);
                threads[i].Start();
            }
            

        }
        private static void Start()
        {

            
            
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
               
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueuePurge("task_queue");

                for (int i =0 ; i<=1000; i++)
                    {
                        var body = Encoding.UTF8.GetBytes($" message{i}");

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;


                    channel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: properties, body: body);
                        Console.WriteLine(" [x] Sent {0}", $" message{i}");


                        //logger.Info(message + "  " + 0 );
                    }
                }
            

        }

        private static void Worker()
        {
            
            Logging  logging = new Logging();
               var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    //  logger.Info(logging +"  "+threadNumber);
                    logging.WriteToLog(message+ Thread.CurrentThread.ManagedThreadId);
                   
                    Thread.Sleep(5000);

                    Console.WriteLine(" [x] Done");
                    using (RabbitMqModel reModel = new RabbitMqModel())
                    {
                        reModel.Logs.Add(new Log() {Message = message});
                        reModel.SaveChanges();
                    }

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "task_queue", autoAck: false, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
