using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// criando instância das configurações da conexão
ConnectionFactory factory = new ConnectionFactory();
factory.UserName = "guest"; //usuário padrão
factory.Password = "guest"; //senha padrão
factory.VirtualHost = "/";
factory.HostName = "localhost";
factory.Port = 5672; // porta padrão
// criando a conexão de fato, com as configurações passadas acima
IConnection conn = factory.CreateConnection();

// criando o canal de comunicação dentro da conexão estabelecida
IModel channel = conn.CreateModel();
// declarando um gerenciador de filas (exchange)
channel.ExchangeDeclare(exchange: "exchange_csharp", type: ExchangeType.Direct, durable: false, autoDelete: false, null);
// declarando uma fila (queue)
channel.QueueDeclare(queue: "queue_csharp", durable: true, exclusive: false, autoDelete: false, null);
// declarando uma ligação entre o gerenciador (exchange) e a fila (queue)
channel.QueueBind(queue: "queue_csharp", exchange: "exchange_csharp", routingKey: "key_csharp", null);

// criando consumidor através do canal configurado anteriormente
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (ch, ea) =>
{
    var body = ea.Body.ToArray(); //pegando corpo da requisiçaõ recebida em bytes
    var messageSerialize = Encoding.UTF8.GetString(body); //extraindo mensagem do corpo da requisição
    var message = JsonSerializer.Deserialize<object>(messageSerialize); //desserializando mensagem em um objeto
    Console.WriteLine(" Recebendo a seguinte mensagem: {0}", message);
    channel.BasicAck(ea.DeliveryTag, false);
    // aguardando 4 segundos para pegar a próxima mensagem da fila
    Thread.Sleep(4000);
};

// consumindo mensagens da fila de acordo com as configurações passadas acima
string consumerTag = channel.BasicConsume(queue: "queue_csharp", autoAck: false, consumer: consumer);
Console.ReadLine();