#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ProductsAPIServices2/ProductsAPIServices2.csproj", "ProductsAPIServices2/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "ProductsAPIServices2/ProductsAPIServices2.csproj"
COPY . .
WORKDIR "/src/ProductsAPIServices2"
RUN dotnet build "ProductsAPIServices2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductsAPIServices2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductsAPIServices2.dll"]