FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY CustomerApi/CustomerApi.csproj CustomerApi/
RUN dotnet restore CustomerApi/CustomerApi.csproj
COPY . .
WORKDIR /src/CustomerApi
RUN dotnet build CustomerApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CustomerApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CustomerApi.dll"]
