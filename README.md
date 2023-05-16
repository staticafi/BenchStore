# BenchStore
## About
BenchStore is an ASP.NET Core MVC application used for storing results from [BenchExec](https://github.com/sosy-lab/benchexec). BenchStore uses PostgreSQL as its database.

## Getting Started
### Requirements
- [Python 3.7 or newer](https://www.python.org/downloads/)
- [BenchExec](https://github.com/sosy-lab/benchexec)
- [PostgreSQL](https://www.postgresql.org/download/)
- [.NET 7.0 SDK](https://dotnet.microsoft.com/en-us/download)

### Appsettings
After downloading the project files, edit the `appsettings.json` configuration file inside the `BenchStoreMVC` project.
```json
  "TableGenerator": {
    "PythonPath": null,
    "TableGeneratorPath": null
  },
  "Storage": {
    "ResultStoragePath": null
  }
```
Set `TableGenerator__PythonPath` to the path of the Python executable.

Set `TableGenerator__TableGeneratorPath` to the path of the `table-generator` executable.

Set `Storage__ResultStoragePath` to the path where you want the result files to get stored.

### Connection string
To set the connection string for the PostgreSQL database, you can edit the `appsettings.json`'s `ConnectionStrings__BenchStoreContext` inside the `BenchStoreMVC` project.
```json
  "ConnectionStrings": {
    "BenchStoreContext": "Host=; Database=; Username=; Password="
  }
```
The format of the connection string is: `"Host=<hostname>; Database=<database-name>; Username=<username>; Password=<password>"`

Alternatively, set the environment variable `CUSTOMCONNSTR_BenchStoreContext` to the PostgreSQL connection string.

Powershell:
```powershell
$env:CUSTOMCONNSTR_BenchStoreContext="Host=<hostname>; Database=<database-name>; Username=<username>; Password=<password>"
```

Bash
```bash
CUSTOMCONNSTR_BenchStoreContext="Host=<hostname>; Database=<database-name>; Username=<username>; Password=<password>"
```

### Quickstart
From the root level of the repository run:
```bash
dotnet run --project ./BenchStoreMVC/
```

Alternatively, in the `./BenchStoreMVC` directory run:
```bash
dotnet run
```

### Development
A development version of BenchStore can be started with [Visual Studio](https://visualstudio.microsoft.com/) on Windows.

The Visual Studio project is started with the `BenchStore.sln` file.

Set the Startup project to the `BenchStoreMVC` project and run the application.

## License
BenchStore is licensed under the Apache 2.0 License.