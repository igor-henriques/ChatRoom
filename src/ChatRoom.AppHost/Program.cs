using ChatRoom.AppHost.Readiness;

var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("chatroom-redis")
    .WithDataVolume(isReadOnly: false);

var dbPassword = builder.AddParameter("DatabasePassword", true);
var postgres = builder
    .AddPostgres(name: "postgres", password: dbPassword, port: 6432)
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("chatroomdb");

var messageBrokerPassword = builder.AddParameter("RabbitMQPassword", true);
var messageBroker = builder.AddRabbitMQ("chatbroker", password: messageBrokerPassword)
    .WithManagementPlugin()
    .WithDataVolume(isReadOnly: false);

var migration = builder.AddProject<Projects.ChatRoom_MigrationService>("migrations")
    .WithReference(postgres)
    .WaitFor(postgres);

var apiService = builder.AddProject<Projects.ChatRoom_ApiService>("apiservice")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(messageBroker)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WaitFor(messageBroker)
    .WaitForCompletion(migration);

builder.AddProject<Projects.ChatRoom_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(postgres)
    .WithReference(messageBroker)
    .WithReference(redis)
    .WaitFor(apiService)
    .WaitFor(postgres)
    .WaitFor(messageBroker)
    .WaitFor(redis)
    .WaitForCompletion(migration);

builder.AddProject<Projects.ChatRoom_Messages_PersistenceConsumer>("chatroom-messages-persistenceconsumer")
    .WithReference(messageBroker)
    .WithReference(apiService)
    .WaitFor(messageBroker)
    .WaitFor(apiService)
    .WaitForCompletion(migration); 

builder.AddProject<Projects.ChatRoom_Messages_BotConsumer>("chatroom-messages-botconsumer")
    .WithReference(messageBroker)
    .WithReference(apiService)
    .WaitFor(messageBroker)
    .WaitFor(apiService)
    .WaitForCompletion(migration);

builder.AddProject<Projects.ChatRoom_Messages_Fanout>("chatroom-messages-realtimefanout")
    .WithReference(messageBroker)
    .WithReference(apiService)
    .WaitFor(messageBroker)
    .WaitFor(apiService)
    .WaitForCompletion(migration);

await builder.Build().RunAsync();