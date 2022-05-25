FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ACI.Images/ACI.Images.csproj", "ACI.Images/"]
RUN dotnet restore "ACI.Images/ACI.Images.csproj"
COPY . .
WORKDIR "/src/ACI.Images"
RUN dotnet build "ACI.Images.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACI.Images.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACI.Images.dll"]
