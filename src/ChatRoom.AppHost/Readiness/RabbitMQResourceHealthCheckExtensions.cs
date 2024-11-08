﻿using HealthChecks.RabbitMQ;

namespace ChatRoom.AppHost.Readiness;

public static class RabbitMQResourceHealthCheckExtensions
{
    /// <summary>
    /// Adds a health check to the RabbitMQ server resource.
    /// </summary>
    public static IResourceBuilder<RabbitMQServerResource> WithHealthCheck(this IResourceBuilder<RabbitMQServerResource> builder)
    {
        return builder.WithAnnotation(HealthCheckAnnotation.Create(cs => new RabbitMQHealthCheck(new RabbitMQHealthCheckOptions { ConnectionUri = new(cs) })));
    }
}
