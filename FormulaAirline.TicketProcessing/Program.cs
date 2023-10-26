// See https://aka.ms/new-console-template for more information
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Welcome to the ticketing service");

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/"
};
var conn = factory.CreateConnection();
using var channel = conn.CreateModel();
channel.QueueDeclare("bookings", durable: true, exclusive: true, autoDelete: true);
/*
bu mesajlar kuyruktan sırayla gönderilir ancak olaylar, CLR iş 
parçacığı havuzunda boş olan iş parçacıkları tarafından işlenir . Bu, birden
fazla mesajın aynı anda farklı iş parçacıkları tarafından ele alınacağı anlamına gelir.
*/
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, EventArgs) =>
{
    var body = EventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"New ticket processing is initiated for - {message}");
};
channel.BasicConsume("bookings", true, consumer);
Console.ReadKey();

