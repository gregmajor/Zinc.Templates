# Zinc.Templates
Congratulations! You have created a new micro-app using the `redline-app` template.

## <a id='solution-structure'></a>Solution Structure
When the solution is created, you will notice that the name of some projects in the solution are prefixed with `Zinc.Templates`. These projects contain the code that are specific to this microservice. Projects that start with the name `RedLine` are platform-specific and intended to be left alone. You can think of them as generated code that gets replaced. As we improve the functionality provided by the `RedLine` platform, we will replace the code in `RedLine` projects. So, if you add any changes to those projects, they will get overwritten.

>**IMPORTANT**: Always put your changes in the projects prefixed with  `Zinc.Templates`. Never edit `RedLine` projects. `RedLine` projects should be edited only in [](https://github.com/YourGitHubOrganization/Zinc.Templates) repository.

There are 3 host projects that are the main entry points:

* __Web__ serves the REST API and front-end component assets.
* __Messaging__ listens to RabbitMQ messages and handles events.
* __Jobs__ runs background jobs.

All code is built into the same container with each project having their own folder. The folder structure in the container looks like:

```
/app/
    Zinc.Templates.Host.Web/
        Zinc.Templates.Hosting.Web.dll
        ...
    Zinc.Templates.Host.Messaging/
        Zinc.Templates.Hosting.Messaging.dll
        ...
    Zinc.Templates.Host.Jobs/
        Zinc.Templates.Host.Jobs.dll
        ...
    Zinc.Templates.Data.Migrations/
        Zinc.Templates.Data.Migrations.dll
        ...
```
