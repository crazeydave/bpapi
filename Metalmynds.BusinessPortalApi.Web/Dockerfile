#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Metalmynds.BtBusinessPortal.Api/Metalmynds.BtBusinessPortal.Api.csproj", "Metalmynds.BtBusinessPortal.Api/"]
RUN dotnet restore "Metalmynds.BtBusinessPortal.Api/Metalmynds.BtBusinessPortal.Api.csproj"
COPY . .
WORKDIR "/src/Metalmynds.BtBusinessPortal.Api"
RUN dotnet build "Metalmynds.BtBusinessPortal.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Metalmynds.BtBusinessPortal.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Metalmynds.BtBusinessPortal.Api.dll"]