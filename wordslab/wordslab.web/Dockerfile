#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["wordslab.web.csproj", "wordslab.web/"]
RUN dotnet restore "wordslab.web/wordslab.web.csproj"
COPY [".", "wordslab.web/"]
WORKDIR "/src/wordslab.web"
RUN dotnet build "wordslab.web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "wordslab.web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "wordslab.web.dll"]