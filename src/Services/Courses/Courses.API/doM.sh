#! /usr/bin/env sh

dotnet ef migrations add Initial -c dcs3spp.courseManagementContainers.Services.Courses.Infrastructure.CourseContext --output-dir Infrastructure/Migrations
dotnet ef migrations add IntegrationEventInitial -s Courses.API.csproj -c dcs3spp.courseManagementContainers.BuildingBlocks.IntegrationEventLogEF.IntegrationEventLogContext -o Infrastructure/IntegrationEventMigrations
