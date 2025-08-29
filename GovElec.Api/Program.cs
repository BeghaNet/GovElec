

using GovElec.Api.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureApi(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureApplication();




app.Run();


