using Distributed.MessagePipe.Implementation;
using Distributed.MessagePipe.Interface;
using Distributed.MessagePipe.Redis.SharedStore;
using M6T.Core.TupleModelBinder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sse.Distributed.WebApi;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

builder.Configuration.AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json");
builder.Configuration.AddEnvironmentVariables("SSE_");

builder.Services.Configure<RedisSharedStoreOptions>(
    opts => builder.Configuration.GetSection(nameof(RedisSharedStoreOptions)).Bind(opts));


// Add services to the container.
builder.Services.RegisterMessagePipe(p =>
{
    var opts = p.GetService<IOptions<RedisSharedStoreOptions>>();
    var redisSharedStore = new RedisSharedStateStore(opts);
    return redisSharedStore;
});




builder.Logging.ClearProviders().AddConsole();

builder.Services.AddMvc(options =>
{
    options.ModelBinderProviders.Insert(0, new TupleModelBinderProvider());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();