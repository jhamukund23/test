using Dss.Application.Constants;
using Dss.Domain.UserRegistration;
using Kafka.Interfaces;

namespace Dss.API.kafkaEvents.UserRegistration.Handlers
{
    public class RegisterUserHandler : IKafkaHandler<string, RegisterUser>
    {
        private readonly IKafkaProducer<string, UserRegistered> _producer;

        public RegisterUserHandler(IKafkaProducer<string, UserRegistered> producer)
        {
            _producer = producer;
        }
        public Task HandleAsync(string key, RegisterUser value)
        {
            // Here we can actually write the code to register a User
            Console.WriteLine($"Consuming UserRegistered topic message with the below data\n FirstName: {value.FirstName}\n LastName: {value.LastName}\n UserName: {value.UserName}\n EmailId: {value.EmailId}");

            //After successful operation, suppose if the registered user has User Id as 1 the we can produce message for other service's consumption
            _producer.ProduceAsync(KafkaTopics.UserRegistered, "", new UserRegistered { UserId = 1 });

            return Task.CompletedTask;
        }
    }
}

