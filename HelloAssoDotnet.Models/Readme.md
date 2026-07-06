# HelloAssoDotnet.Models package.
This package provides the **public** models the HelloAssoDotnet NuGet package uses.
It ships the following namespaces: 
* Configuration : used to configure the library. Usually used through `appsettings.json` files
* HelloAssoApi : models pulled from HelloAsso REST API. Those models percolate through this library, in order to limit models rewriting and code duplication.
* PublicApi : data models used by HelloAssoDotnet library to provide it's results
* Secrets : file formats that this library uses in order to properly communicate with HelloAsso Rest API services.
* Utils : some utilities and tools mainly used to properly read `Configuration` objects

# Purpose and goal of the NuGet package
The main purpose and goal of this package is to provide only the data models, so that other client tools can leverage it.
It allows proper decoupling between services and models.

For instance, a mobile app developed in dotnet MAUI (and then using the same data models) doesn't always need the HelloAssoDotnet services and clients.
Only the models can be used, when this mobile app talks to a server which has full control over the HelloAsso world.
