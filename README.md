# Overview
This repository provides a sample Domain Driven Design (DDD) architecure for course management, hosted within docker-compose. It contains a REST and Grpc API, integrated with an Identity Server 4 instance. The architecture is based upon the Microsoft eshop containerised application, extended to provide HTTPS and compatability with the [Incrementally better Cookies](https://tools.ietf.org/html/draft-west-cookie-incrementalism-00/) standard.

Key technologies used include:
- [Autofac](https://autofac.org/)
- [C#](https://docs.microsoft.com/en-us/dotnet/core/) and [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/?view=aspnetcore-3.1#pivot=core)
- [Entity Framework Core](https://entityframeworkcore.com/)
- [Postgresql](https://www.postgresql.org/)
- [MediatR](https://github.com/jbogard/MediatR)
- [Microsoft SQL Server](https://docs.microsoft.com/en-us/sql/?view=sql-server-ver15)
- [Seq](https://datalust.co/seq)
- [Serilog](https://serilog.net/)
- [XUnit](https://xunit.net/)

# System Requirements
On MacOS environments ensure that docker is configured to run with at least 4GB of memory in addition to sharing */usr/local/share/dotnet*.

# Quick start
Start up the docker compose services by issuing the following command:
``` bash
docker-compose.yml -f docker-compose.yml -f docker-compose.override.yml up
```
Open a browser instance and visit the swagger interface at [https://localhost:8002](https://localhost:8002). Click the *authorize* button to sign in and acquire a token from the identity server instance. Try out the course API endpoints via the swagger interface.

# Docker Compose Architecture
The docker-compose architecture, illustrated below, provides a replicable development environment with the following services:

![architecture](https://gitlab.com/dcs3spp/myfiles/-/raw/master/Docker-Compose_Architecture.png)

- **courses-api**: An ASP.NET.Core API that provides http and Grpc endpoints for managing course resources. Swagger is accessible from https://localhost:8002. This provides a DDD Aggregate for managing courses with [EF Core](https://entityframeworkcore.com/) and uses [MediatR](https://github.com/jbogard/MediatR) for dispatch of commands and in-process domain events. [Autofac](https://autofac.org/) is used as an Inversion of Control (IOC) container to simplify the management of object dependencies.  
- **identity-api**: An ASP.NET MVC host serves as an identity provider or STS (Security Token Service). This is an Identity Server 4 implementation that encapsulates ASP.NET Identity, based on that provided by the Microsoft eshop containerised application. It has been extended with HTTPS, issuing cookies with a policy of *SameSite=None*. The login page is accessible from https://localhost:8001/account/login. Login, registration views etc. can be customised in the Views folder of the Identity/Identity.API project.
- **coursesdb**: A Postgresql database provides storage for course resources. The database is named *courses* and exposes port 5232 to enable a suitable Postgresql client to connect from the local development machine. The default database username is *app* with password *Pa55word*. Use a VSCode Postgresql extension to connect to host 127.0.0.1:5232 while docker-compose services are running. This will allow viewing of course management database tables.
- **identitydb**: A Postgresql database provides storage for Identity4 resources. The database is named *identity* with a username of *app* and password *Pass@word*. The data is stored in a volume named *courses-sqldata*. Use a VSCode Postgresql extension to connect to host 127.0.0.1:5231 while docker-compose services are running. This will allow viewing of identity database tables.
- **seq**: A seq instance accessible from *http://localhost:5340*, provides filtering of docker-compose logs and exceptions.
- **rabbitmq**: Message queue provider that delivers events dispatched between microservices to provide eventual consitency. 

There are three docker-compose files within the *src* folder of the project root:
- **docker-compose.yml**: Base service configuration.
- **docker-compose.override.yml**: Defines ports and environment variable configurations for use in development environment.
- **docker-compose.production.yml**: Defines ports and environment variable configuratios for use in production environment.

Most environment variables are configured directly within the docker compose files. A sample .env file has been provided within the *src* folder, *.env-sample*. This contains environment variables for configuration. Ensure that this is copied and renamed to *.env*. The .env file is not committed into source control.

The docker-compose architecture can be built from within the *src* folder as follows:

- **Development**: ```docker-compose -f docker-compose.yml -f docker-compose.override.yml```
- **Production**: ```docker-compose -f docker-compose.yml -f docker-compose.production.yml```

Once built the docker-compose architecture can be run by issuing the following command:

- **Development**: ```docker-compose -f docker-compose.yml -f docker-compose.override.yml up```
- **Production**: ```docker-compose -f docker-compose.yml -f docker-compose.override.yml up```

To stop the docker-compose architecture issue the following command:

- **Development**: ```docker-compose -f docker-compose.yml -f docker-compose.override.yml down```
- **Production**: ```docker-compose -f docker-compose.yml -f docker-compose.override.yml down```

For efficient storage it is occasionally beneficial to issue the following commands:
``` bash
# remove trailing containers
docker container prune -f

# remove redundant images
docker image prune -f

# remove redundant volumes
docker volume prune -f
```

# Unit Testing
Unit testing can be performed from within the *src/Services/Courses/Courses.UnitTests* folder, by issuing the following command:

``` bash
dotnet test
```

# End To End Testing
To perform end-to-end testing the docker-compose architecture needs to be started to provide the identity server,
databases and message queue services.

```docker-compose -f docker-compose.yml -f docker-compose.override.yml up```

A core library for functional testing has been provided at *src/Testing/BuildingBlocks/Core*. This offers a small class library for creating a [TestServer](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.testhost.testserver?view=aspnetcore-3.0) instance via a class derived from [WebApplicationFactory](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1?view=aspnetcore-3.0). This is in addition to hooks for setup and teardown of testing seed data, triggered during creation of the server. Example usage of the framework is available witin *src/Services/Courses/Courses.FunctionalTests*.

With the docker-compose architecture started, issue the following command in a separate terminal window, from within the *src/Services/Courses/Courses.FunctionalTests* folder. 

``` bash
dotnet test
```

# Certificates
The Certificates folder contains a server certificate (server.pfx) and a root certificate, suitable for use in development environments. The root certificate is added to the trusted certificate store of the course-api docker image to allow container-container HTTPS between the swagger web client and identity server. The server certificate is used in containers to provide HTTPS. In this demo the server certificate contains the SANs for the webserver and identity server, as opposed to creating a separate certificate per server. 

For development environments a self signed certificate is normally generated and saved to the trusted store with 
```dotnet dev-certs https --trusted```. In Linux environments developers usually extract and save the public certificate to the trusted store, e.g. by issuing ```sudo update-ca-certificates```.

At the time of development, Microsoft released dotnet 3.1.102, which introduced a bug with the dev-certs tool on macOS environments, as detailed [here](https://github.com/dotnet/aspnetcore/issues/19590). Subsequently, a workaround has been provided for macOS environments in the form of the ```generate.sh``` script within the Certificates folder.

If using Firefox then the certificate needs to be added into the Firefoxs certificate store.


# Incrementally Better Cookies
With the introduction of the [Incrementally better Cookies](https://tools.ietf.org/html/draft-west-cookie-incrementalism-00/) standard, cross site cookies must be configured as *SameSite=None; Secure*. This is being enforced in newer browsers, including *Chrome 80+*. However, some browsers, e.g. Safari < v13 do not recognise *Same-Site=None* and provide a default value of *Same-Site=Strict*. To provide compatability, the ASP.Net Core identity server has been modified with an extension method that inspects the requesting user agent and sets *Same-Site=None; Secure* manually, when issuing the cookie.
