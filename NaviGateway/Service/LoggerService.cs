using System;
using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using NaviGateway.Model;
using Newtonsoft.Json;
using LogMessage = NaviGateway.Model.LogMessage;

namespace NaviGateway.Service
{
    public class LoggerService
    {
        private readonly ProducerConfig _producerConfig;
        private readonly string _logTopic;
        private readonly string _logService;
        
        public LoggerService(IConfiguration configuration)
        {
            _producerConfig = new ProducerConfig
            {
                BootstrapServers = GetKafkaStrings(configuration, "KafkaServerAddr"),
                ClientId = Dns.GetHostName(),
                Acks = Acks.None
            };

            _logTopic = GetKafkaStrings(configuration, "KafkaTopic");
            _logService = GetKafkaStrings(configuration, "KafkaServiceName");
        }

        public async Task LogErrorMessageAsync(string message)
        {
            var messageObject = new LogMessage
            {
                Message = message,
                LogService = _logService,
                LogCreatedAt = DateTime.Now,
                LogType = LogMessageType.Error
            };
            
            await SendMessageToKafka(messageObject);
        }

        public async Task LogInfoMessageAsync(string message)
        {
            var messageObject = new LogMessage
            {
                Message = message,
                LogService = _logService,
                LogCreatedAt = DateTime.Now,
                LogType = LogMessageType.Info
            };
            await SendMessageToKafka(messageObject);
        }

        private async Task SendMessageToKafka(LogMessage message)
        {
            var serialized = JsonConvert.SerializeObject(message);
            using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
            await producer.ProduceAsync(_logTopic, new Message<Null, string> {Value = serialized});
        }
        
        private static string GetKafkaStrings (IConfiguration configuration, string name)
        {
            return configuration?.GetSection("KafkaStrings")?[name];
        }
    }
}