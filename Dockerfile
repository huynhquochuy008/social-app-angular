#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DatingApp/DatingApp.csproj", "DatingApp/"]
COPY ["DatingApp.Application/DatingApp.Application.csproj", "DatingApp.Application/"]
COPY ["DatingApp.Core/DatingApp.Core.csproj", "DatingApp.Core/"]
COPY ["DatingApp.Infrastructure/DatingApp.Infrastructure.csproj", "DatingApp.Infrastructure/"]
RUN dotnet restore "DatingApp/DatingApp.csproj"
# RUN dotnet tool install --global dotnet-ef --version 5.0
COPY . .
WORKDIR "/src/DatingApp"
RUN dotnet build "DatingApp.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "DatingApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/Share/Images
ENTRYPOINT ["dotnet", "DatingApp.dll"]
