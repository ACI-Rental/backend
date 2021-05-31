  
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ReservationService.csproj", "ReservationService/"]
RUN dotnet restore "ReservationService/ReservationService.csproj"
WORKDIR "/src/ReservationService"
COPY . .
RUN dotnet build "ReservationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReservationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReservationService.dll"]