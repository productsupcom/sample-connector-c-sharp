# Sample Connector (C#)

A sample Productsup connector written in C# targeting .NET 9.

The connector communicates with the platform through the Container API. See the [Container API documentation](https://help.productsup.com/docs/connector-development/container-api) for details.

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Docker (optional)

## Run locally

```bash
cd src/ProductsupConnector
dotnet run
```

## Commands

The connector type is selected with the first argument (defaults to `data-service`):

| Command        | Reads                                                  | Writes     |
|----------------|--------------------------------------------------------|------------|
| `datasource`   | — (generates `PRODUCTS` products, `BATCH` per request) | `output`   |
| `data-service` | `input`                                                | `output`   |
| `export`       | `input`                                                | `feedback` |
| `export-delta` | `new`, `modified`, `unchanged`, `deleted`              | `feedback` |

```bash
dotnet run -- export
dotnet ProductsupConnector.dll export-delta
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
