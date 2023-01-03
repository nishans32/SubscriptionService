FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY SubscriptionService.Web/*.csproj ./
RUN dotnet restore

COPY SubscriptionService.Web/ ./
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "SubscriptionService.Web.dll", "--environment=Development"]