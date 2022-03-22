FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ACI.ImageService/ACI.ImageService.csproj", "ACI.ImageService/"]
RUN dotnet restore "ACI.ImageService/ACI.ImageService.csproj"
COPY . .
WORKDIR "/src/ACI.ImageService"
RUN dotnet build "ACI.ImageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACI.ImageService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACI.ImageService.dll"]
