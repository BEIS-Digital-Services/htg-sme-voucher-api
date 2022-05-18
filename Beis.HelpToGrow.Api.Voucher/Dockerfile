#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Beis.HelpToGrow.Api.Voucher/Beis.HelpToGrow.Api.Voucher.csproj", "Beis.HelpToGrow.Api.Voucher/"]
RUN dotnet restore "Beis.HelpToGrow.Api.Voucher/Beis.HelpToGrow.Api.Voucher.csproj"
COPY . .
WORKDIR "/src/Beis.HelpToGrow.Api.Voucher"
RUN dotnet build "Beis.HelpToGrow.Api.Voucher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Beis.HelpToGrow.Api.Voucher.csproj" -c Release -o /app/publish



FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Beis.HelpToGrow.Api.Voucher.dll"]


HEALTHCHECK CMD curl --fail http://localhost:5000/healthz || exit

