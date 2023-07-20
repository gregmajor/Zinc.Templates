# Data Migrations
This project creates an entrypoint DLL that runs database migrations for the solution.

## Adding migrations
Before adding a migration, a migration number needs to be reserved using Servo, the team chat bot, on one of the squad channels in Slack. 

```
servo assign migration
```

All migrations should be added to the `Migrations` folder in this project. Normally, the added migration runs in all environments. However, we can filter which migrations are executed depending on the environment and the entrypoint project.

### Filtering based on environment
The `Tags` attribute can be used to constrain the migration run only on the selected environments. These environments can be used:

* __Development__ is the environment when developer uses a local build/run sequence, like `dotnet run`.
* __docker-local__ is the environment when developer runs the migrations inside a docker container, locally.
* __docker-circleci__ is the environment when the migrations run in docker container in the build server.
* __preprod__ is the Kubernetes preproduction environment.
* __prod__ is the Kubernetes production environment.

For instance, the migration that has the following tags will run only on Development, docker-local, and docker-circleci environments. It will not run on preprod or prod.

```csharp
[Tags(TagBehavior.RequireAny, "Development", "docker-local", "docker-circleci")]
```

> __TIP:__ The environment name is captured from  `DOTNET_ENVIRONMENT` environment variable, or if it is not defined, `ASPNETCORE_ENVIRONMENT`. 

### Filtering Based on Entrypoint
Although migrations create its own entrypoint dll, it can also be run in-process. Integration and functional tests, for instance, run the migrations within their own process.

You can filter migrations based on the name of the entrypoint dotnet project that started the whole execution. This helps us control further which migrations run. For instance, the following migration will only run for functional tests and web integration tests, but will not run for `Zinc.Templates.Data.Migrations` or `Zinc.Templates.IntegrationTests.Messaging`.

```csharp
[Tags(TagBehavior.RequireAny, "Zinc.Templates.FunctionalTests", "Zinc.Templates.IntegrationTests.Web")]
```

> __TIP:__ The entrypoint name is captured from `APP_ENTRYPOINT` enironment variable.
