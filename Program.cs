using IPv64IPScanner;
using Microsoft.AspNetCore.Http.Json;
using Quartz;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<IStartupFilter, StartUpBuilder>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAuthentication();

builder.Services.AddCors();

builder.Services.AddControllers(opt => { opt.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true; })
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
        opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true; // Enable case-insensitivity
    options.SerializerOptions.PropertyNamingPolicy = null; // Preserve exact casing
});

builder.Services.AddSingleton(builder.Configuration);

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("JobValidateIp");
    q.AddJob<ExecuteValidateIp>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("JobValidateIp-trigger")
        .StartAt(DateTimeOffset.Now.AddMinutes(1))
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(Env.IP_TASK_INTERVAL).RepeatForever())
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("IPv64IPScanner API")
            .WithModels(false)
            .WithLayout(ScalarLayout.Classic)
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
    app.UseCors(x => x
        .WithOrigins("http://localhost:4200")
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
}
else
{
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();