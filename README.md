# General overview
Experiment with docker compose, where 2 services (service-1 built with C# dotnet, and service-2  build with JavaScript node) retrieving some system information (Uptime, Available storage, IP address, running processes).

Both services are HTTP API, service-1 is exposed on port 8199 and the route "/"; It retrieves the information of its container and service-2.
