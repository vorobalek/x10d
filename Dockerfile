FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80/;https://+:443
ENV ASPNETCORE_HTTPS_PORT 443
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=certificate.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source
RUN dotnet dev-certs https -ep /app/certificate.pfx -p password
COPY . .

WORKDIR /source/core/X10D.Core
RUN dotnet restore
RUN dotnet publish "X10D.Core.csproj" -c Release -o /app

FROM runtime AS image
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "X10D.Core.dll"]