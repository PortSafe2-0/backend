# Etapa 1: build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia arquivos de projeto e restaura dependências
COPY *.csproj ./
RUN dotnet restore ./portsafe_api.csproj

# Copia o restante do código
COPY . .
WORKDIR /app

# Build e publish
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "portsafe_api.dll"]
