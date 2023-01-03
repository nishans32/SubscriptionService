# User API Dev Guide

The api endpoints are hooked up to the comments in the controller using swashbuckle/swagger. Once the application is running, [applicationurl]/index.html will launch a Swagger ui which will allow consumers / contributers to understand the api methods/capabilities. 

Furthermore, the [postman request collection](./TestProject.WebAPI/Zip-Pay-api-postman-request-collection.json) can be used to understand the api spec

## Building
Prerequisites
1. .NET Core 3.0
1. Docker

```
git clone --recursive https://git-rba.hackerrank.com/git/cee43fb0-6c5d-4d49-995a-1daa7e51221b
cd cee43fb0-6c5d-4d49-995a-1daa7e51221b

dotnet restore
dotnet build 

```

Please note that the `dotnet [action]` commands are to be run from the root of the project(solution)

## Local 

### Setting up postgres DB for local development

#### In Docker

A secondary docker-compose file `docker-compose-local.yml` can be used to stand up the postgres database locally (in docker) which also create roles, users and tables

```
docker-compose -f docker-compose-local.yml up
```

#### Local Postgres server

Run the scripts [grant_permissions.sql](./Scripts/grant_permissions.sql) and [create_tables.sql](./Scripts/create_tables.sql) in the same order 

Change the solutions appsettings.development.json  to reflect the database connection settings as follows

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "DBConnectionStrings": {
    "UserDb": "Server=127.0.0.1; Database=zip_pay_user; Port=5432; User Id=zippay_app_user; Password=[PasswordInDockerComposeLocalFile]"
  }
}
```
### Running the app

Run the app through Visual Studio / Rider / VSCode, or with CLI using:

```
dotnet run --project TestProject.WebAPI
```
note: If you are using VS Code to run the app, please ensure that you have changed your `launch.json` to reflect the above changes

## Testing
From the root of the repo:
`dotnet test TestProject.Tests/TestProject.Tests.csproj`
Or from the `TestProject.Tests` folder:
`dotnet test`

To test the endpoints, a postman request collection : [Zip-Pay-api-postman-request-collection.json](./TestProject.WebAPI/Zip-Pay-api-postman-request-collection.json) can be used. Please ensure that you populate the required values

## Deploying

The docker compose file : `docker-compose.yml` is configured to stand up docker containers for the postgres database and the zip-pay-web-api and establishe a network connection between them. This file also runs the database scripts which creates all database : roles, users and tables.

```
 docker compose up
```

Monitoring : /health provides a basic health status of the app.

## Additional Information

Please note that the `docker-compose.yml` contains password and this is not ideal at all. These will need to be moved to a cloud : secure store (aws Secret store) and injected in the build pipeline as environment variables.

Integration tests are not included in this solution. This is not ideal as a production system should contain integration tests. There should be two sets of integration tests

1. DB integration tests.
   A minimal set of tests should be implemented to confirm the integration between the DB and th app. These can be implemented as a .Net test project which uses the existing components such as Repository classes to communicate to the DB. A connection string should be included for this project

2. Api tests

   a. There are two ways to implement this. We can run the [provided series of postman tests](./TestProject.WebAPI/Zip-Pay-api-postman-request-collection.json)  
      against a stage environment which are triggered from the build pipeline. 

   b. Use a request-interception framework such as  [justeat/httpclient-interception](https://github.com/justeat/httpclient-interception) to intercept the requests 
      and mock the repo to test the api responses. 

    

