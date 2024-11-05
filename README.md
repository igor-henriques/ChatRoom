# ***Real-Time Chat Application with Stock Command Validation***

This project is a real-time chat application built on the .NET Aspire framework. The architecture leverages PostgreSQL for data persistence, RabbitMQ for message queuing, and WebSocket for real-time message updates. This setup is designed to support a robust chat system with stock command validation in a scalable, distributed environment.

## **Overview**

The chat application allows users to communicate in real-time. Users can send messages, and the system validates commands related to stock information. The message flow is designed to ensure that commands are processed efficiently and that all active WebSocket clients receive updates immediately after message persistence.

## **Technologies Used**

- **.NET Aspire**: Core framework for building, deploying, and orchestrating services.
- **PostgreSQL**: Primary database for storing chat messages and user data.
- **RabbitMQ**: Manages message queues and topics to coordinate between services.
- **WebSocket**: Delivers real-time updates to connected users.

## **Architecture and Message Flow**

The architecture relies on a layered message flow across multiple services, ensuring messages are handled securely, validated, and delivered in real-time. Here’s an overview of the message flow:

1. **Message Posting and Initial Exchange**
   - The Web project serves as the entry point for messages. When a user posts a message in the chat, it is immediately published to a RabbitMQ exchange.
   - The exchange is set up to trigger two specific consumers that handle different aspects of message processing.

2. **Consumers and Message Processing**
   - **Stock Command Validation Consumer**: This consumer listens for messages containing specific stock-related commands. It validates and processes these commands, ensuring that users receive accurate and up-to-date information.
   - **Message Persistence Consumer**: This consumer is responsible for persisting messages to the PostgreSQL database using the API layer. It interacts with the API to ensure that each message is stored reliably.

3. **Message Republish for Real-Time Updates**
   - After the API successfully confirms that a message has been persisted, it republishes the message, now including an updated ID, to a dedicated queue.
   - This queue activates a consumer designed specifically for real-time message broadcasting.

4. **WebSocket Fanout for Real-Time Broadcasting**
   - The broadcasting consumer uses the updated message data to perform a fanout operation, pushing the message to all connected WebSocket clients.
   - Each WebSocket client receives the message in real time, enabling an immediate, synchronized chat experience across users.

5. **Redis for caching**
   - Still not implemented.

## **Project Structure**

The solution is organized into several key projects:

- **Web Project**: Handles the user interface and front-end interactions. It publishes messages to the RabbitMQ exchange.
- **API Project**: Serves as the backend for interacting with the database and handling business logic for message persistence.
- **Consumers**: Background services that handle message validation, persistence, and broadcasting. The two primary consumers are:
  - Stock Command Validation Consumer
  - Message Persistence Consumer
  - WebSocket fanout Consumer
- **WebSocket Endpoint**: Manages real-time connections to deliver live updates directly to users.

## **Setup and Configuration**

To run this application locally, ensure you have the following services available:

- **.NET 8 SDK**: To actually run the project.
- **Docker**: In order to Aspire orchestrate all containers.

## **Usage**

### Running the Application:

1. Make sure to have Aspire installed by running the following command **before opening Visual Studio**: *dotnet workload update*. Note that if you run this command with Visual Studio opened, you'll need to restart it in order to build the project.
2. Make sure Docker is up and running
3. Start the project by running *ChatRoom.AppHost*
4. Once Aspire is running, a dashboard with all services will be shown. Select the last one, named *webfrontend*. A web application should open in your browser, create an account and initiate a chat session at */chats*.

### Sending Messages:

- Enter a message or a stock command in the chat window, like: */stock=aapl.us*
- The Web project will publish the message to the RabbitMQ exchange, and the consumers will begin processing.

### Receiving Real-Time Updates:

- Messages are broadcasted to all active WebSocket connections as soon as they are persisted in the database, providing a real-time chat experience.

## **Future Improvements**

Potential future enhancements include:

- Improved error handling and resilience across services.
- Enhanced stock command validation with additional data sources.
- Real time broadcasting regarding new chats being created. Now the page needs to be refreshed.
- It is a known bug that stock commands only will be echoed back from the bot if two or more users are in the chat.
