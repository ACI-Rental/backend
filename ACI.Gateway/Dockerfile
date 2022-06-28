FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ACI.Gateway/ACI.Gateway.csproj", "ACI.Gateway/"]
RUN dotnet restore "ACI.Gateway/ACI.Gateway.csproj"
COPY . .
WORKDIR "/src/ACI.Gateway"
RUN dotnet build "ACI.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACI.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACI.Gateway.dll"]
