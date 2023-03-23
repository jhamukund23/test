using Confluent.Kafka;
using Dss.Domain.MRM;
using Dss.Application.Queries;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Dss.Application.Constants;

namespace Dss.Application.Handlers
{
    public class GenerateSASUrlHandler : IRequestHandler<GenerateSASUrlQuery, SASUrlResponse>
    {
        private readonly IOptions<ConsumerConfig> _configuration;

        public GenerateSASUrlHandler(IOptions<ConsumerConfig> configuration)
        {
            _configuration = configuration;
        }

        public async Task<SASUrlResponse> Handle(GenerateSASUrlQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // create the consumer
                var consumer = new ConsumerBuilder<Null, string>(_configuration.Value).Build();
                // subscribe to the specified topic
                //consumer.Subscribe(request.TopicName);
                consumer.Subscribe(KafkaTopics.SASUrlResponse);
                // while no data available wait
                while (true)
                {
                    // consume the data
                    var cr = consumer.Consume();
                    // close & commit
                    consumer.Close();
                    // return result
                    var result = JsonConvert.DeserializeObject<SASUrlResponse>(cr.Message.Value);
                    return await Task.FromResult(result);                    
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
