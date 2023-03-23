using Dss.Application.Constants;
using Dss.Domain.Models.MRM;
using Kafka.Interfaces;

namespace Dss.API.kafkaEvents.UserRegistration.Handlers
{
    public class AddDocumentHandler : IKafkaHandler<string, AddDocument>
    {
        private readonly IKafkaProducer<string, AddDocumentResponse> _producer;

        public AddDocumentHandler(IKafkaProducer<string, AddDocumentResponse> producer)
        {
            _producer = producer;
        }
        public Task HandleAsync(string key, AddDocument value)
        {
            // Here we can actually write the code to register a User
            Console.WriteLine($"Consuming UserRegistered topic message with the below data\n StartsOn: {value.StartsOn}\n ExpiresOn: {value.ExpiresOn}");

            //After successful operation, suppose if the registered user has User Id as 1 the we can produce message for other service's consumption
            _producer.ProduceAsync(KafkaTopics.AddDocumentResponse, "", new AddDocumentResponse { SasUrl = new Uri("") });

            return Task.CompletedTask;
        }
    }
}

