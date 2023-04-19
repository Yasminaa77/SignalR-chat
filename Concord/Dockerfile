FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5037

ENV ASPNETCORE_URLS=http://+:5037

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Concord.csproj", "./"]
RUN dotnet restore "Concord.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Concord.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Concord.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Concord.dll"]
