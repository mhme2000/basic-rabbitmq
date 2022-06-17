using System.Text;
using RabbitMQ.Client;
using System.Text.Json;

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

// criando e serializando mensagem em formato json
var messageJson = new
{
    Type = "Sucesso",
    Message = "Deu certo"
};
var messageJsonSerialize = JsonSerializer.Serialize(messageJson);
var body = Encoding.UTF8.GetBytes(messageJsonSerialize);

// enviando mensagem através do canal criado anteriormente
while (true)
{
    channel.BasicPublish(exchange: "exchange_csharp", routingKey: "key_csharp", mandatory: false, null, body);
    Console.WriteLine(" Enviando a seguinte mensagem: {0}", messageJsonSerialize);
    // aguardando 2 segundos para enviar a próxima mensagem para a fila
    Thread.Sleep(2000);
}
