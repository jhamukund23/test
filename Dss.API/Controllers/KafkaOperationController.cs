using Dss.API.AzureBlobStorage.Controllers;
using Dss.application.Interfaces;
using Dss.Application.Constants;
using Dss.Domain.UserRegistration;
using Dss.Application.Queries;
using Dss.Domain.DTOs;
using Dss.Domain.MRM;
using Kafka.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Dss.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KafkaOperationController : ControllerBase
    {
        private readonly IKafkaProducer<string, RegisterUser> _kafkaProducer;
        private readonly IKafkaProducer<string, SASUrlRequest> _kafkaMRMProducer;
        private readonly IKafkaProducer<string, SASUrlResponse> _kafkaMRMResponseProducer;
        private readonly IAzureStorage _storage;
        private readonly ILogger<StorageController> _logger;
        private readonly IMediator _mediator;
        public KafkaOperationController(
            IKafkaProducer<string, RegisterUser> kafkaProducer,
            IKafkaProducer<string, SASUrlRequest> kafkaMRMProducer,
            IKafkaProducer<string, SASUrlResponse> kafkaMRMResponseProducer,
            IAzureStorage storage, ILogger<StorageController> logger, IMediator mediator
            )
        {
            _kafkaProducer = kafkaProducer;
            _kafkaMRMProducer = kafkaMRMProducer;
            _kafkaMRMResponseProducer = kafkaMRMResponseProducer;
            _storage = storage;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GenerateSASUrl")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateSASUrl(SASUrlRequest sasUrlRequest)
        {
            //await _kafkaMRMProducer.ProduceAsync(KafkaTopics.SendNewFileToIDM, null, sasUrlRequest);

            var res = await _storage.CreateContainerAsync("mr11");

            Uri sasUrl =  _storage.GetServiceSasUriForDirectory();
            //Uri sasUrl = _storage.GetServiceSasUriForBlob();
            //Uri sasUrl = await _storage.GetServiceSasUriForContainer();
            SASUrlResponse sASUrlResponse = new()
            {
                SasUrl = sasUrl,
                UserName= sasUrlRequest.UserName
            };
            //await _kafkaMRMResponseProducer.ProduceAsync(KafkaTopics.SASUrlResponse, null, sASUrlResponse);

           // var sasUrlDetails = await _mediator.Send(new GenerateSASUrlQuery());

            return Ok(sASUrlResponse);
            //return Ok(sasUrlDetails);
        }



        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ProduceMessage(RegisterUser request)
        {
            await _kafkaProducer.ProduceAsync(KafkaTopics.RegisterUser, null, request);

            return Ok("User Registration In Progress");
        }

    }
}
