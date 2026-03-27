FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY src/ProductsupConnector/ProductsupConnector.csproj src/ProductsupConnector/
RUN dotnet restore src/ProductsupConnector/ProductsupConnector.csproj

COPY src/ src/
RUN dotnet publish src/ProductsupConnector/ProductsupConnector.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "ProductsupConnector.dll"]
