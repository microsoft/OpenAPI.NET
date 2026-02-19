# Contributing to OpenAPI.net

OpenAPI.net is a mono-repo containing source code for the following packages:

## Libraries

| Library                                                              | NuGet Release                                                                                                                                                                              |
|----------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [Microsoft.OpenAPI](./src/Microsoft.OpenAPI/README.md)                         | [![NuGet Version](https://img.shields.io/nuget/vpre/Microsoft.OpenAPI?label=Latest&logo=nuget)](https://www.nuget.org/packages/Microsoft.OpenAPI/)                       |
| [Microsoft.OpenAPI.YamlReader](./src/Microsoft.OpenApi.YamlReader/README.md)                         | [![NuGet Version](https://img.shields.io/nuget/vpre/Microsoft.OpenAPI.YamlReader?label=Latest&logo=nuget)](https://www.nuget.org/packages/Microsoft.OpenAPI.YamlReader/)                       |
| [Microsoft.OpenAPI.Hidi](./src/Microsoft.OpenAPI.Hidi/README.md)                         | [![NuGet Version](https://img.shields.io/nuget/vpre/Microsoft.OpenAPI.Hidi?label=Latest&logo=nuget)](https://www.nuget.org/packages/Microsoft.OpenAPI.Hidi/)                       |

OpenAPI.net is open to contributions. There are a couple of different recommended paths to get contributions into the released version of this library.

__NOTE__ A signed a contribution license agreement is required for all contributions, and is checked automatically on new pull requests. Please read and sign [the agreement](https://cla.microsoft.com/) before starting any work for this repository.

## File issues

The best way to get started with a contribution is to start a dialog with the owners of this repository. Sometimes features will be under development or out of scope for this SDK and it's best to check before starting work on contribution. Discussions on bugs and potential fixes could point you to the write change to make.

## Submit pull requests for bug fixes and features

Feel free to submit a pull request with a linked issue.

### Branches and support policy

Because one major consumer of these libraries is ASP.net, the support policy of this repository is aligned with [dotnet support policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core#lifecycle).

The following table outlines the mapping between package major versions, dotnet versions, and which contributions are accepted. As a consumer, make sure the version of this library your application is using is aligned with the version of ASP.net described in the table below.

| Major version | Branch     | Supported [AspNetCore OpenAPI versions](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi)  | Supported [Swashbuckle.AspNetCore version](https://www.nuget.org/packages/Swashbuckle.AspNetCore/) | Supported OpenAPI versions | Changes provided by Microsoft               | Accepted contributions                      | End of support date |
| ------------- | ---------- | -------------------------- | ---------- | -------------------------- | ------------------------------------------- | ------------------------------------------- | --------------- |
| 1.X           | support/v1 | < 10                       | < 10          | 2.0, 3.0                   | security fixes                              | security and bugfixes                       | .NET 9 (Nov 2026)           |
| 2.X           | support/v2 | = 10 ¹                     | = 10 ³        | 2.0, 3.0, 3.1              | security and bugfixes                       | security and bugfixes                       | .NET 10 (Nov 2028) ¹ |
| 3.X  ²        | main       | not available              | not available | 2.0, 3.0, 3.1, 3.2         | security, bugfixes and feature improvements | security, bugfixes and feature improvements | TBD  |

> Notes:
>
> 1. This assumes that AspNetCore OpenAPI version 11 and above will adopt version 3 or above of this library, otherwise, it'd expand the support date for version 2 of this library.
> 2. This will be conditioned by new releases of OpenAPI, this library, ASP.NET and AspNetCore OpenAPI's adoption of new versions of this library.
> 3. This assumes that Swashbuckle.AspNetCore version 11 and above will adopt version 3 or above of this library.

### Multi-versions requirement for contributions

When contributing to the library, start by making a contribution to the main branch first, or the uppermost version it applies to. During the review process you'll be asked to demonstrate your contribution cannot apply to prior versions or to port your contribution to the branches for prior versions before the initial pull request can get merged.

This approach helps maintain a similar behavior across all versions under active support.

## Commit message format

To support our automated release process, pull requests are required to follow the [Conventional Commit](https://www.conventionalcommits.org/en/v1.0.0/)
format.
Each commit message consists of a __header__, an optional __body__ and an optional __footer__. The header is the first line of the commit and
MUST have a __type__ (see below for a list of types) and a __description__. An optional __scope__ can be added to the header to give extra context.

```
<type>[optional scope]: <short description>
<BLANK LINE>
<optional body>
<BLANK LINE>
<optional footer(s)>
```

The recommended commit types used are:

- __feat__ for feature updates (increments the _minor_ version)
- __fix__ for bug fixes (increments the _patch_ version)
- __perf__ for performance related changes e.g. optimizing an algorithm
- __refactor__ for code refactoring changes
- __test__ for test suite updates e.g. adding a test or fixing a test
- __style__ for changes that don't affect the meaning of code. e.g. formatting changes
- __docs__ for documentation updates e.g. ReadMe update or code documentation updates
- __build__ for build system changes (gradle updates, external dependency updates)
- __ci__ for CI configuration file changes e.g. updating a pipeline
- __chore__ for miscallaneous non-sdk changesin the repo e.g. removing an unused file

Adding an exclamation mark after the commit type (`feat!`) or footer with the prefix __BREAKING CHANGE:__ will cause an increment of the _major_ version.

## Updates to public API surface

Because we need to maintain a compatible public API surface within a major version, this project is using the public API analyzers to ensure no prior public API is changed/removed inadvertently.

This means that:

- All entries in an __Unshipped__ document need to be moved to the __Shipped__ document before any public release.
- All new APIs being added need to be __Unshipped__ document before the pull request can be merged, otherwise build will fail with a message like the example below.

```txt
Error: /home/runner/work/OpenAPI.NET/OpenAPI.NET/src/Microsoft.OpenApi/Models/OpenApiSecurityScheme.cs(39,46): error RS0016: Symbol 'OAuth2MetadataUrl.set' is not part of the declared public API (https://github.com/dotnet/roslyn-analyzers/blob/main/src/PublicApiAnalyzers/PublicApiAnalyzers.Help.md) [/home/runner/work/OpenAPI.NET/OpenAPI.NET/src/Microsoft.OpenApi/Microsoft.OpenApi.csproj::TargetFramework=net8.0]
```

### Update the unshipped document

To update the unshipped document, simply run the following commands

```shell
# add the missing public api entries
dotnet format --diagnostics RS0016
# discard changes to cs files to avoid creating conflicts
git checkout *.cs
```

### Move items from unshipped to unshipped document

```pwsh
. ./scripts/promoteUnshipped.ps1
```
