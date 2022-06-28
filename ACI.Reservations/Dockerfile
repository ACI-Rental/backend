FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ACI.Reservations/ACI.Reservations.csproj", "ACI.Reservations/"]
RUN dotnet restore "ACI.Reservations/ACI.Reservations.csproj"
COPY . .
WORKDIR "/src/ACI.Reservations"
RUN dotnet build "ACI.Reservations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACI.Reservations.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACI.Reservations.dll"]