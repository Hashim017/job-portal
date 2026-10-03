FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY JobPortal.csproj ./
RUN dotnet restore JobPortal.csproj
COPY . .
RUN dotnet publish JobPortal.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000} dotnet JobPortal.dll"]