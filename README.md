# Zinc.Templates
This repository contains the RedLine templates used in code generation for microservices. New microservies are generated using a template in this repository. The template creates the boilerplate code, which helps us bootstrap the new microservice faster and focus on the application logic.

## Installing the Template

### <a id='clone-repo'>Clone the Zinc.Templates Repository</a>
The source for the template is contained in the [Zinc.Templates](https://github.com/YourGitHubOrganization/Zinc.Templates) repository on GitHub. You'll need to use `git` to clone a copy to your computer.

```cmd
git clone git@github.com:YourGitHubOrganization/Zinc.Templates.git
```

### <a id='install-template'>Install the .NET Solution Template</a>
Once you have a copy of the template source code, you'll need to install the template itself.  There is 1 template directory in the repository:

* dotnet-7.0

Open a command line in the template directory of your choice and run the following command:

Windows:

```cmd
.template.config/install.bat
```

Mac/Linux:

```cmd
chmod +x .template.config/install.sh && .template.config/install.sh
```

## Using the Template
Read these template-specific help documents for using each template.

* [Read this for instructions on using the dotnet-7.0 template](./dotnet-7.0.md)
