FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/SecondChance.WebApi/SecondChance.WebApi.csproj", "src/SecondChance.WebApi/"]
COPY ["src/SecondChance.Domain/SecondChance.Domain.csproj", "src/SecondChance.Domain/"]
COPY ["src/SecondChance.Application/SecondChance.Application.csproj", "src/SecondChance.Application/"]
COPY ["src/SecondChance.Infrastructure/SecondChance.Infrastructure.csproj", "src/SecondChance.Infrastructure/"]

RUN dotnet restore "src/SecondChance.WebApi/SecondChance.WebApi.csproj"

COPY . .
WORKDIR "/src/src/SecondChance.WebApi"
RUN dotnet build "SecondChance.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SecondChance.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SecondChance.WebApi.dll"]