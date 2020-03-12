FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /x10d

EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80/;https://+:443
ENV ASPNETCORE_HTTPS_PORT 443
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=certificate.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS app_build
WORKDIR /source
RUN dotnet dev-certs https -ep certificate.pfx -p password
COPY . .
RUN dotnet restore && dotnet build "X10D.sln" -c Release

FROM node AS web_build
WORKDIR /source
COPY . .
WORKDIR /source/web
RUN rm -rf node_modules && npm cache clean --force && npm install @angular/cli && npm install && npm run build-prod

FROM runtime AS image
WORKDIR /x10d
COPY --from=app_build /source/.builds .
COPY --from=web_build /source/.builds .
WORKDIR /x10d/app/netcoreapp3.1 
COPY --from=app_build /source/certificate.pfx .

ENTRYPOINT ["dotnet", "X10D.Core.dll"]