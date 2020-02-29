FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /x10d

EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80/;https://+:443
ENV ASPNETCORE_HTTPS_PORT 443
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=certificate.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source
RUN dotnet dev-certs https -ep certificate.pfx -p password
COPY . .
RUN dotnet restore
RUN dotnet build "X10D.sln" -c Release

FROM runtime AS image
WORKDIR /x10d
COPY --from=build /source/.builds .
WORKDIR /x10d/app/netcoreapp3.1 
COPY --from=build /source/certificate.pfx .
ENTRYPOINT ["dotnet", "X10D.Core.dll"]