using System.Text;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
using Serilog;
using simpleApi.Connection;
using simpleApi.Interface;
using simpleApi.Mapping;
using simpleApi.Models;
using simpleApi.Repository;
using simpleApi.Service;


var builder = WebApplication.CreateBuilder(args);

// start setup .env
DotNetEnv.Env.Load();
// end setup .env

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleApi Dong", Version = "v1" });
});

// start add costom logging    
builder.Services.AddHttpLogging(logging =>
{
    // Customize HTTP logging here.
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("my-response-header");
    logging.MediaTypeOptions.AddText("application/json");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/miniApiLog.log", rollingInterval: RollingInterval.Day, encoding: Encoding.UTF8)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// end add costom logging    

// start add Kafka configuration
builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddSingleton<KafkaConsumerService>();
builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection("KafkaConfig"));
// end add Kafka configuration

// start  add config db nya
builder
    .Services
    .AddDbContext<DatabaseUtama>(
        options =>
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionString"))
        // .LogTo(Console.WriteLine, LogLevel.Information)
        // untuk menampilkan log query ke db optional
    );
// end  add config db nya


// end add costom logging    
var apiKey = Environment.GetEnvironmentVariable("TES");
var dbLog = Convert.ToBoolean(Environment.GetEnvironmentVariable("DB_LOG"));
Console.WriteLine("PPPPPPP  " + apiKey);
Console.WriteLine(dbLog.GetType());
Console.WriteLine(dbLog);

builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IExternalDataService, ExternalDataService>();
builder.Services.AddScoped<IAlbumRepository, SqlAlbumRepoository>();

builder.Services.AddAutoMapper(typeof(CustomAutoMapper));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleApi lah v1"); });
}

app.UseHttpsRedirection();

app.UseAuthorization();
// start add config auto logger
app.UseMiddleware<LoggingMiddleware>();
// end add config auto logger
app.UseCors();
app.MapControllers();

// Consuming Kafka messages in a background thread

var isKafkaConsumerEnabled = builder.Configuration.GetValue<bool>("ENABLE_KAFKA_CONSUMER");
// var isKafkaConsumerEnabled = Convert.ToBoolean(Environment.GetEnvironmentVariable("ENABLE_KAFKA_CONSUMER"));
if (isKafkaConsumerEnabled)
{
    logger.Information(
        $"\nInformation -> Program.cs \n  Kafka Consumer is Enable based on the environment variable. : {isKafkaConsumerEnabled}\n");
    var serviceProvider = app.Services;
    var kafkaConsumerService = serviceProvider.GetRequiredService<KafkaConsumerService>();
    var cancellationTokenSource = new CancellationTokenSource();
    var cancellationToken = cancellationTokenSource.Token;

    Task.Run(() => kafkaConsumerService.ConsumeMessages(cancellationToken), cancellationToken);
}
else
{
    logger.Information(
        $"\nInformation -> Program.cs \n  Kafka Consumer is disabled based on the environment variable. : {isKafkaConsumerEnabled}\n");
}


app.Run();