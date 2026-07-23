FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all .csproj files to restore dependencies first (for faster build caching)
COPY ["src/Webapi/Webapi.csproj", "src/Webapi/"]
COPY ["src/SecondChance.Domain/SecondChance.Domain.csproj", "src/SecondChance.Domain/"]
COPY ["src/SecondChance.Application/SecondChance.Application.csproj", "src/SecondChance.Application/"]
COPY ["src/SecondChance.Infrastructure/SecondChance.Infrastructure.csproj", "src/SecondChance.Infrastructure/"]

RUN dotnet restore "src/Webapi/Webapi.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Webapi"
RUN dotnet build "Webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Webapi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Webapi.dll"]