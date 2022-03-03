var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.MapGet("/response/{id:int}", (int id) =>
{
    if (new Random().Next(1, 100) >= id)
    {
        Console.WriteLine("---> Failure - Generate a HTTP 500");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }

    Console.WriteLine("---> Success - Generate a HTTP 200");
    return Results.StatusCode(StatusCodes.Status200OK);
})
.WithName("GetResponse");

app.Run();