#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
-
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Infrastructure/ApplicationDatabase/ApplicationDatabase.csproj", "Infrastructure/ApplicationDatabase/"]
RUN dotnet restore "Infrastructure/ApplicationDatabase/ApplicationDatabase.csproj"
COPY . .
WORKDIR "/src/Infrastructure/ApplicationDatabase"
RUN dotnet build "LogDatabase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApplicationDatabase.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApplicationDatabase.dll"]