# Using the dotnet-7.0 Template

## Reserve Ports
Each microservice is assigned a range of ports, which helps us keep track of them when running in developer machines and CICD. In order to help organize this, functionality from [Tom Servo chat bot](https://github.com/gregmajor/tom-servo) is used.

Use your team's slack channel to reserve ports using the _Tom Servo_ chat bot. The following syntax shows what to type (replace `<appName>` with the solution name):

```
servo reserve ports for <appName>
```

Ports are reserved in blocks of 100, like 6700-6799. In order to see the list of reserved ports, you can use the following command for `Tom Servo`:

```
servo list ports
```

## Create the Solution
When it comes time to create a new solution, follow these steps:

1. Create a folder for the solution. The template creates all of the content in that folder. The name of the folder becomes the solution name. It should follow the pattern `Element.AppName`. The `Element` part of the name comes from the periodic table, like `Hydrogen`, `Krypton`, etc. The second part of the name (`AppName`) is any meaningful name for the solution.
1. Open a terminal/console window in the newly created solution folder.
1. Run `dotnet new redline-app` to see the help text for required parameters. The following parameters are required:

    * `--dottedShort` is the short name, which is formed by abbreviating the element part of the name. We simply use its abbreviation from the periodic table, since it is chosen from there. For instance, `Hydrogen.Sample` is shortened to `H.Sample`, `Silver.Sample` is shortened to `Ag.Sample`, `Krypton.Sample` is shortened to `Kr.Sample`.
    * `--appPort` is the port number for the web host. Common convention is to use a port ending with `01` in the range of ports reserved. For instance, `5101` if the reserved port range is `5100 to 5199`.
    * `--jobsPort` is the port number for the jobs host. Common convention is to use a port ending with `11` in the range of ports reserved. For instance, `5111` if the reserved port range is `5100 to 5199`.
    * `--messagingPort` is the port number for the messaging host. Common convention is to use a port ending with `21` in the range of ports reserved. For instance, `5121` if the reserved port range is `5100 to 5199`.
    * `--dbSchema` is the Postgres schema name your app will use. This parameter will replace the "app" schema used by the sample code with your actual schema. It also replaces in the SchemasToInclude property of our test base classes. If not specificied, the value "app" schema will be preserved.

 ### Example
 Assuming that our solution name is `Carbon.Sample` and reserved ports are `5100 to 5199`, the following example shows the command with required parameters:

```
dotnet new redline-app --dottedShort C.Sample --appPort 5101 --jobsPort 5111 --messagingPort 5121 --dbSchema sample
```

### RedLine vs App
When the solution is created, you will notice that the name of some projects in the solution are prefixed with the solution name, like `Carbon.Sample.Data`. These projects contain the code that are specific to the solution.

Projects that start with the name `RedLine` are platform-specific and intended to be left alone. The idea is that, as we improve the functionality provided by the `RedLine` platform, we will replace the code in `RedLine` projects. So, if you add any changes to those projects, they will get overwritten.

> **IMPORTANT**: Always put your changes in the projects that share the name with the solution and never edit `RedLine` projects.

## Follow Further Instructions in README.md
At the root of the solution, a `README.md` file is created as part of the code generation. Read and follow instructions there to build and deploy the new solution.
