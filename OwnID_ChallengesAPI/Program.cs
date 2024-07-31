using OwnID_ChallengesAPI.ChallengesRepository;
using OwnID_ChallengesAPI.ChallengesServices.Notifications;
using OwnID_ChallengesAPI.ChallengesServices;

var builder = WebApplication.CreateBuilder(args);

/*builder.Services.AddSingleton<Fido2>(sp =>
{
    var config = new Fido2Configuration
    {
        ServerDomain = "localhost",
        Origin = "http://localhost:5248"
    };

    return new Fido2(config);
});*/

builder.Services.AddControllers();
builder.Services.AddScoped<ChallengesService>();
builder.Services.AddHttpClient<NotificationsService>();
builder.Services.AddSingleton<ConfigurationRepository>();
builder.Services.AddSingleton<OtpHandler>();
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
