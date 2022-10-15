using OuraRingDataIngest.BackgroundServices;
using OuraRingDataIngest.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOuraRingDataIngestServices();
builder.Services.AddHostedService<HeartRateIngestBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseServiceStack(new AppHost());
app.Run();
