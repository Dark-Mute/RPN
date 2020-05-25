FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /src
COPY ["WEB API.csproj", "./"]
RUN dotnet restore "./WEB API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WEB API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WEB API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WEB API.dll"]
