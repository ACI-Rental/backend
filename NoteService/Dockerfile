FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NoteService.csproj", "NoteService/"]
RUN dotnet restore "NoteService/NoteService.csproj"
WORKDIR "/src/NoteService"
COPY . .
RUN dotnet build "NoteService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NoteService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NoteService.dll"]