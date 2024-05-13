FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY src/HiLoGame/HiLoGame.csproj HiLoGame/
COPY src/HiLoGame.Client/HiLoGame.Client.csproj HiLoGame.Client/
COPY src/HiLoGame.Domain/HiLoGame.Domain.csproj HiLoGame.Domain/
RUN dotnet restore HiLoGame/HiLoGame.csproj
COPY . .
WORKDIR "/src/src/HiLoGame"
RUN dotnet build "./HiLoGame.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HiLoGame.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5263
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HiLoGame.dll"]