FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY src/ConexaoSolidaria.Api/*.csproj ./src/ConexaoSolidaria.Api/
COPY src/ConexaoSolidaria.Application/*.csproj ./src/ConexaoSolidaria.Application/
COPY src/ConexaoSolidaria.Domain/*.csproj ./src/ConexaoSolidaria.Domain/
COPY src/ConexaoSolidaria.Infrastructure/*.csproj ./src/ConexaoSolidaria.Infrastructure/
RUN dotnet restore src/ConexaoSolidaria.Api/ConexaoSolidaria.Api.csproj
COPY src/ ./src/
RUN dotnet publish src/ConexaoSolidaria.Api/ConexaoSolidaria.Api.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ConexaoSolidaria.Api.dll"]
