using Dss.Application.Constants;
using Dss.Domain.Models.MRM;
using Dss.Domain.UserRegistration;
using Kafka.Interfaces;
using System.Net;

namespace Dss.API.kafkaEvents.UserRegistration.Consumers
{
    public class AddDocumentConsumer : BackgroundService
    {
        private readonly IKafkaConsumer<string, AddDocument> _consumer;
        public AddDocumentConsumer(IKafkaConsumer<string, AddDocument> kafkaConsumer)
        {
            _consumer = kafkaConsumer;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _consumer.Consume(KafkaTopics.AddDocument, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{(int)HttpStatusCode.InternalServerError} ConsumeFailedOnTopic - {KafkaTopics.AddDocument}, {ex}");
            }
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();

            base.Dispose();
        }
    }
}
