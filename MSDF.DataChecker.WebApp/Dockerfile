#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

WORKDIR /src
COPY ["MSDF.DataChecker.WebApp/MSDF.DataChecker.WebApp.csproj", "MSDF.DataChecker.WebApp/"]
COPY ["MSDF.DataChecker.Services/MSDF.DataChecker.Services.csproj", "MSDF.DataChecker.Services/"]
COPY ["MSDF.DataChecker.Persistence/MSDF.DataChecker.Persistence.csproj", "MSDF.DataChecker.Persistence/"]
RUN dotnet restore "MSDF.DataChecker.WebApp/MSDF.DataChecker.WebApp.csproj"
COPY . .
WORKDIR "/src/MSDF.DataChecker.WebApp"
RUN dotnet build "MSDF.DataChecker.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MSDF.DataChecker.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSDF.DataChecker.WebApp.dll"]
