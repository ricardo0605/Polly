using RequestService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder
    .Services
    .AddHttpClient("ResponseClient")
    //.AddPolicyHandler(request => request.Method == HttpMethod.Get ? new ClientPolicy().ImmediateHttpRetry : new ClientPolicy().ImmediateHttpRetry);
    .AddPolicyHandler(request => new ClientPolicy().LinearHttpRetry);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/request", async (IHttpClientFactory httpClientFactory) =>
{
    using (var client = httpClientFactory.CreateClient("ResponseClient"))
    {
        var response = await client.GetAsync("https://localhost:7242/response/50");

        #region NotUsingNamedHttpClientCodeReference

        //var response = await new ClientPolicy().ImmediateHttpRetry.ExecuteAsync(() =>
        //    client.GetAsync("https://localhost:7242/response/50"));

        //var response = await new ClientPolicy().LinearHttpRetry.ExecuteAsync(() =>
        //    client.GetAsync("https://localhost:7242/response/50"));

        //var response = await new ClientPolicy().ExponentialHttpRetry.ExecuteAsync(() =>
        //    client.GetAsync("https://localhost:7242/response/50"));

        #endregion

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("---> ResponseService returned success");
            return Results.StatusCode(StatusCodes.Status200OK);
        }

        Console.WriteLine("---> ResponseService returned failure");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})
.WithName("MakeRequest");

app.Run();