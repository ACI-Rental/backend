FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ACI.Messaging/ACI.Messaging.csproj", "ACI.Messaging/"]
RUN dotnet restore "ACI.Messaging/ACI.Messaging.csproj"
COPY . .
WORKDIR "/src/ACI.Messaging"
RUN dotnet build "ACI.Messaging.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACI.Messaging.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACI.Messaging.dll"]
