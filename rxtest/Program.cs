

using System.Reactive.Linq;

// a simple ticker using rx
IObservable<long> ticks = Observable.Timer(
    dueTime: TimeSpan.Zero,
    period: TimeSpan.FromSeconds(1));

// results will be shown only after we subscribe
ticks.Subscribe(
    ticks => Console.WriteLine($"ticks: {ticks}"));

Console.ReadLine();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
