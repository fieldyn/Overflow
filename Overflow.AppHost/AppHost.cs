#pragma warning disable ASPIRECERTIFICATES001
var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloak", 6001)
    .WithoutHttpsCertificate()
    .WithDataVolume("keycloak-data2");

var postgres = builder.AddPostgres("postgres", port: 5432)
    .WithDataVolume("postgres-data2")
    .WithPgAdmin();

var questionDb = postgres.AddDatabase("questionDb");

var questionService = builder.AddProject<Projects.QuestionService>("question-src")
    .WithReference(keycloak)
    .WithReference(questionDb)
    .WaitFor(keycloak)
    .WaitFor(questionDb);

builder.Build().Run();