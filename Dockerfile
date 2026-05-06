FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Talma.AiServices.Api/Talma.AiServices.Api.csproj", "src/Talma.AiServices.Api/"]
COPY ["src/Talma.AiServices.Core/Talma.AiServices.Core.csproj", "src/Talma.AiServices.Core/"]
COPY ["src/Talma.AiServices.Data/Talma.AiServices.Data.csproj", "src/Talma.AiServices.Data/"]
COPY ["src/Talma.AiServices.Infrastructure/Talma.AiServices.Infrastructure.csproj", "src/Talma.AiServices.Infrastructure/"]

RUN dotnet restore "src/Talma.AiServices.Api/Talma.AiServices.Api.csproj"

COPY . .

WORKDIR "/src/src/Talma.AiServices.Api"
RUN dotnet publish "Talma.AiServices.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl=8.5.0-2ubuntu10.9 && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .

RUN useradd -r -s /bin/false appuser
USER appuser

EXPOSE 5000

ENTRYPOINT ["dotnet", "Talma.AiServices.Api.dll"]