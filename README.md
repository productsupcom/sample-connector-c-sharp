# Sample Connector (C#)

A sample Productsup connector written in C# targeting .NET 9.

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Docker (optional)

## Run locally

```bash
cd src/ProductsupConnector
dotnet run
```

## Run with Docker

```bash
docker compose up
```

Hot reload is enabled via `dotnet watch run` in the Docker Compose setup.

## Build Docker image

```bash
docker build -t productsup-connector .
```
