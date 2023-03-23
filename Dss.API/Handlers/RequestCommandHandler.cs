using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Dss.API.Handlers
{
    public class RequestCommandHandler
    {
        private readonly IOptions<ProducerConfig> _configuration;

        public RequestCommandHandler(IOptions<ProducerConfig> configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Handle(RequestCommand request)
        {
            try
            {
                // Serialize the data
                var serializedData = JsonConvert.SerializeObject(request.Person);
                // create the producer instance
                var producer = new ProducerBuilder<Null, string>(_configuration.Value).Build();
                // publish the data on the Topic
                await producer.ProduceAsync(request.Topic, new Message<Null, string> { Value = serializedData });
                // destroy the producer after completing all requests
                producer.Flush(TimeSpan.FromSeconds(10));
                //return success
                return await Task.FromResult(true);
            }
            catch (KafkaException)
            {
                throw;
            }
        }
    }
}
