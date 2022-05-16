FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ACI.Shared/ACI.Shared.csproj", "ACI.Shared/ACI.Shared.csproj"]
COPY ["ACI.Products/ACI.Products.csproj", "ACI.Products/ACI.Products.csproj"]

RUN dotnet restore "ACI.Products/ACI.Products.csproj"
COPY . .
WORKDIR "/src/ACI.Products"
RUN dotnet build "ACI.Products.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACI.Products.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACI.Products.dll"]