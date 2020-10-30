# Samples.GameMatch
Sample game/competition opponent matching api

## [Build Test Run](#build-test-run)

* Download and install dotnet core 3.1 LTS
  * Mac: <https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-3.1.100-macos-x64-installer>

  * Linux instructions: <https://docs.microsoft.com/dotnet/core/install/linux-package-managers>

  * Windows: <https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-3.1.100-windows-x64-installer>

* Clone code and build:
```bash
git clone https://github.com/boydc7/Samples.GameMatch
cd Samples.GameMatch
dotnet publish -c Release -o publish Samples.GameMatch.Api/Samples.GameMatch.Api.csproj
```
* Run (from Samples.GameMatch folder from above)
```bash
cd publish
dotnet gmapi.dll
```

The api will start and by default listen for requests on localhost:8084.

Once started, you can view the Swagger for the API at <http://localhost:8084 />

## [Authentication](#authentication)

Uses a simple JWT based auth method.

Signup a new user first with a POST to /users.
Login with a POST to /users/login , which will return a JWT token in the response.

At startup, the API is seeded with 50 sample users, with emails and passwords in the following format (where X is an integer from 1 to 50)
* demouserX@gamematchdemo.io
* UserXSuperSecretPassword

These 50 users are also seeded with a single Chess game rating ranging from 0.1 up to 5.0.

## [Endpoints](#endpoints)

See swagger at <http://localhost:8084 /> after starting the API.
