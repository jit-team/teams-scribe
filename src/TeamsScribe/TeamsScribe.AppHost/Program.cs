var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.TeamsScribe_ApiService>("apiservice");

builder.Build().Run();
