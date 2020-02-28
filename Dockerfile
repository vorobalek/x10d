FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

EXPOSE 8080
EXPOSE 8443
ENV ASPNETCORE_URLS=http://+:8080/;https://+:8443
ENV ASPNETCORE_HTTPS_PORT 443
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=ssl.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password


ENV LANG=ru_RU.UTF-8  
ENV LANGUAGE=ru_RU:ru  
ENV LC_ALL=ru_RU.UTF-8

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source
RUN dotnet dev-certs https -ep /app/ssl.pfx -p password
COPY . .

WORKDIR /source/core/X10D.Core
RUN dotnet restore
RUN dotnet build "X10D.Core.csproj" -c Release -o /app

FROM runtime AS image
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "X10D.Core.dll"]