FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["TestApplication/TestApplication.csproj", "TestApplication/"]
RUN dotnet restore "TestApplication/TestApplication.csproj"
COPY . .
WORKDIR "/src/TestApplication"
RUN dotnet build "TestApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestApplication.dll"]