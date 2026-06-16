FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# copy csproj and restore
COPY ["Estufa.Api.csproj", "./"]
RUN dotnet restore "Estufa.Api.csproj"

# copy everything else and publish
COPY . .
RUN dotnet publish "Estufa.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Estufa.Api.dll"]
