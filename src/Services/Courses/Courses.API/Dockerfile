FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "microservices.sln" "microservices.sln"

COPY "BuildingBlocks/EventBus/EventBus/EventBus.csproj" "BuildingBlocks/EventBus/EventBus/EventBus.csproj"
COPY "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj" "BuildingBlocks/EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj"
COPY "BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj" "BuildingBlocks/EventBus/EventBusServiceBus/EventBusServiceBus.csproj"
COPY "BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj" "BuildingBlocks/EventBus/IntegrationEventLogEF/IntegrationEventLogEF.csproj"
COPY "BuildingBlocks/WebHost.Customization/WebHost.Customization.csproj" "BuildingBlocks/WebHost.Customization/WebHost.Customization.csproj"
COPY "BuildingBlocks/Validation/Validation.csproj" "BuildingBlocks/Validation/Validation.csproj"
COPY "Services/Courses/Courses.API/Courses.API.csproj" "Services/Courses/Courses.API/Courses.API.csproj"
COPY "Services/Courses/Courses.Domain/Courses.Domain.csproj" "Services/Courses/Courses.Domain/Courses.Domain.csproj"
COPY "Services/Courses/Courses.FunctionalTests/Courses.FunctionalTests.csproj" "Services/Catalog/Catalog.FunctionalTests/Catalog.FunctionalTests.csproj"
COPY "Services/Courses/Courses.Infrastructure/Courses.Infrastructure.csproj" "Services/Courses/Courses.Infrastructure/Courses.Infrastructure.csproj"
COPY "Services/Courses/Courses.StoredProcedures/Courses.StoredProcedures.csproj" "Services/Courses/Courses.StoredProcedures/Courses.StoredProcedures.csproj"
COPY "Services/Courses/Courses.UnitTests/Courses.UnitTests.csproj" "Services/Courses/Courses.UnitTests/Courses.UnitTests.csproj"
COPY "Services/Identity/Identity.API/Identity.API.csproj" "Services/Identity/Identity.API/Identity.API.csproj"
COPY "Testing/BuildingBlocks/Core/TestingCore.csproj" "Testing/BuildingBlocks/Core/TestingCore.csproj"

COPY "docker-compose.dcproj" "docker-compose.dcproj"
 
# COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "microservices.sln"

COPY . .

WORKDIR /src/Services/Courses/Courses.API
RUN dotnet publish --no-restore -c Release -o /app

FROM build as unittest
WORKDIR /src/Services/Courses/Courses.UnitTests

FROM build as functionaltest
WORKDIR /src/Services/Courses/Courses.FunctionalTests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

# copy ca.crt
COPY ./Certificates/ca.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "Courses.API.dll"]