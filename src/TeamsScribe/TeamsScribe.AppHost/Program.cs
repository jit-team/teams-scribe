var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.TeamsScribe_ApiService>("apiservice");
builder.AddProject<Projects.TeamsScribe_Web>("webfrontend").WithReference(apiservice);
builder.Build().Run();
