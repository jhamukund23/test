using Dss.Application.Constants;
using Dss.Domain.UserRegistration;
using Kafka.Interfaces;
using System.Net;

namespace Dss.API.kafkaEvents.UserRegistration.Consumers
{
    public class RegisterUserConsumer : BackgroundService
    {
        private readonly IKafkaConsumer<string, RegisterUser> _consumer;
        public RegisterUserConsumer(IKafkaConsumer<string, RegisterUser> kafkaConsumer)
        {
            _consumer = kafkaConsumer;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _consumer.Consume(KafkaTopics.RegisterUser, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{(int)HttpStatusCode.InternalServerError} ConsumeFailedOnTopic - {KafkaTopics.RegisterUser}, {ex}");
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
