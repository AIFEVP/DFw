var builder = DistributedApplication.CreateBuilder(args);
// Add the Docker Compose environment
builder.AddDockerComposeEnvironment("composedir");

var apiService = builder.AddProject<Projects.Aspire_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Aspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
