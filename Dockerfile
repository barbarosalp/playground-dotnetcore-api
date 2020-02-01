FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Barb.Core.Api/*.csproj ./Barb.Core.Api/
RUN dotnet restore

# copy everything else and build app
COPY Barb.Core.Api/. ./Barb.Core.Api/
WORKDIR /app/Barb.Core.Api
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
EXPOSE 8080
WORKDIR /app
COPY --from=build /app/Barb.Core.Api/out ./
ENTRYPOINT ["dotnet", "Barb.Core.Api.dll"]
