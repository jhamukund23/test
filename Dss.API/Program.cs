using Confluent.Kafka;
using MediatR;
using Dss.API.Handlers;
using Dss.application.Interfaces;
using Dss.Application.Interfaces;
using Dss.Application.Services;
using Dss.Infrastructure.Persistence;
using Kafka.Consumer;
using Kafka.Interfaces;
using Kafka.Producer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Dss.Application.Extensions;
using Dss.Domain.Models.MRM;
using Dss.API.kafkaEvents.UserRegistration.Handlers;
using Dss.API.kafkaEvents.UserRegistration.Consumers;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// Connect to PostgreSQL Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(connectionString));

// Register Azure Blob Container Client
builder.Services.AddSingleton(x => new BlobContainerClient(builder.Configuration["BlobConnectionString"], builder.Configuration["BlobContainerName"]));
builder.Services.AddTransient(x => new BlobServiceClient(builder.Configuration["BlobConnectionString"]));


// Register MediatR services
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddTransient<IMediator, Mediator>();
builder.Services.AddMediatorHandlers(typeof(Program).GetTypeInfo().Assembly);
builder.Services.AddMediatorHandlers();


// Add services to the container.
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IAzureStorage, AzureStorage>();

// configure the consumer
builder.Services.Configure<ConsumerConfig>(options =>
{
    options.BootstrapServers = builder.Configuration.GetSection("KafkaConsumerConfig:bootstrapservers").Value;
    options.GroupId = builder.Configuration.GetSection("KafkaConsumerConfig:groupId").Value;
    options.AutoOffsetReset = AutoOffsetReset.Earliest;
});
// Inject the request handler
builder.Services.AddSingleton<RequestQueryHandler>();
// Inject the request command handler
builder.Services.AddSingleton<RequestCommandHandler>();


// Configure the producer
builder.Services.Configure<ProducerConfig>(options =>
{
    options.BootstrapServers = builder.Configuration.GetSection("KafkaProducerConfig:bootstrapservers").Value;
    //options.SaslUsername = builder.Configuration.GetSection("KafkaProducerConfig:SaslUsername").Value;
    //options.SaslPassword = builder.Configuration.GetSection("KafkaProducerConfig:SaslPassword").Value;
    //options.SecurityProtocol = builder.Configuration.GetSection("KafkaProducerConfig:SecurityProtocol").Value;  
});

var clientConfig = new ClientConfig()
{
    BootstrapServers = builder.Configuration["KafkaProducerConfig:bootstrapservers"],
    SaslUsername = builder.Configuration["KafkaProducerConfig:SaslUsername"],
    SaslPassword = builder.Configuration["KafkaProducerConfig:SaslPassword"],
    SecurityProtocol = SecurityProtocol.SaslSsl,
    SaslMechanism = SaslMechanism.Plain
};

var producerConfig = new ProducerConfig(clientConfig);
var consumerConfig = new ConsumerConfig(clientConfig)
{
    GroupId = Guid.NewGuid().ToString(),
    EnableAutoCommit = true,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    StatisticsIntervalMs = 5000,
    SessionTimeoutMs = 6000
};

builder.Services.AddSingleton(producerConfig);
builder.Services.AddSingleton(consumerConfig);
builder.Services.AddSingleton(typeof(IKafkaProducer<,>), typeof(KafkaProducer<,>));

//builder.Services.AddScoped<IKafkaHandler<string, RegisterUser>, RegisterUserHandler>();
builder.Services.AddScoped<IKafkaHandler<string, AddDocument>, AddDocumentHandler>();
builder.Services.AddSingleton(typeof(IKafkaConsumer<,>), typeof(KafkaConsumer<,>));
builder.Services.AddHostedService<AddDocumentConsumer>();
//builder.Services.AddHostedService<RegisterUserConsumer>();


// Add Serilog
builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Console()
    .WriteTo.File("logs/dsslogs.txt", rollingInterval: RollingInterval.Day)
    .ReadFrom.Configuration(builder.Configuration);
});

//set uploadsize large files
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

//set uploadsize large files
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(120);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(120);
    //options.Limits.MaxRequestBodySize = 500_000_000;
    //options.Limits.MaxRequestBufferSize = null;
    //options.Limits.MaxResponseBufferSize = null;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Shows UseCors with CorsPolicyBuilder.
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseAuthorization();
app.MapControllers();

app.Run();

