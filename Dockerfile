FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/ask:8.0 AS build
WORKDIR /src
COPY ["GirlsWantTheBestShop.csproj", "."]
RUN dotnet restore "./GirlsWantTheBestShop.csproj"
COPY . . 
WORKDIR "/src/."
RUN dotnet build "GirlsWantTheBestShop.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GirlsWantTheBest.csproj" -c -Release -o /app/publish

FROM base AS final 
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "GirlsWantTheBestShop.dll"]