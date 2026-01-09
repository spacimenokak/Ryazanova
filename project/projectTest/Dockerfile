FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["projectTest.csproj", "./"]
RUN dotnet restore "projectTest.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "projectTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "projectTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "projectTest.dll"]
