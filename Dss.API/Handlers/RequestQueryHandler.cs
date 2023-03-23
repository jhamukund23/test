using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Dss.API.Handlers
{
    public class RequestQueryHandler
    {
        private readonly IOptions<ConsumerConfig> _configuration;

        public RequestQueryHandler(IOptions<ConsumerConfig> configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Handle(RequestQuery request)
        {
            try
            {
                // create the consumer
                var consumer = new ConsumerBuilder<Null,string>(_configuration.Value).Build();
                // subscribe to the specified topic
                consumer.Subscribe(request.TopicName);
                // while no data available wait
                while (true)
                {
                    // consume the data
                    var cr = consumer.Consume();
                    // close & commit
                    consumer.Close();
                    // return result
                    return await Task.FromResult(cr.Message.Value);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
