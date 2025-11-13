var builder = DistributedApplication.CreateBuilder(args);
// Add the Docker Compose environment
builder.AddDockerComposeEnvironment("env");

// Add a Redis server to the application and set lifetime to persistent
var cache = builder.AddRedis("cache")
                   .WithLifetime(ContainerLifetime.Persistent);

var apiService = builder.AddProject<Projects.Aspire_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WaitFor(cache);

builder.AddProject<Projects.Aspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(cache)
    .WaitFor(cache);

builder.Build().Run();
