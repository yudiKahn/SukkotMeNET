# Repository Guidelines

## Project Structure & Module Organization

`IEsrog.sln` contains one .NET 9 Blazor Server project in `IEsrog/`. Application startup and dependency registration live in `Program.cs`. Razor pages are grouped under `Pages/` (`AdminPages/` and `UserPages/`), reusable UI belongs in `Components/` or `Shared/`, and static CSS/images live in `wwwroot/`. Domain models and MongoDB persistence are separated into `Models/` and `Data/`; application and email integrations live in `Services/`. Configuration types and JSON defaults are under `Configuration/`. There is currently no test project; add one beside the application, for example `IEsrog.Tests/`, and include it in the solution.

## Build, Test, and Development Commands

Run commands from the repository root:

- `dotnet restore IEsrog.sln` restores NuGet dependencies.
- `dotnet build IEsrog.sln` compiles the solution and reports analyzer/compiler errors.
- `dotnet run --project IEsrog/IEsrog.csproj` starts the local Blazor application using `Properties/launchSettings.json`.
- `dotnet test IEsrog.sln` runs all test projects once tests are added.
- `dotnet format IEsrog.sln --verify-no-changes` checks formatting before submitting changes.

## Coding Style & Naming Conventions

Use four-space indentation, nullable reference types, and implicit usings as configured in `IEsrog.csproj`. Follow standard C# conventions: PascalCase for public types, members, Razor components, and files; camelCase for locals and parameters; prefix interfaces with `I`. Keep component markup in `.razor` files and move substantial logic into matching `.razor.cs` partial classes. Register new services explicitly in `Program.cs`, choosing lifetimes deliberately.

## Testing Guidelines

Prefer xUnit for new tests and name files after the unit under test, such as `InvoiceServiceTests.cs`. Use `Method_Scenario_ExpectedResult` test names. Isolate MongoDB, email, filesystem, and clock dependencies so tests remain deterministic. Add regression tests for bug fixes and run `dotnet test IEsrog.sln` before opening a pull request.

## Commit & Pull Request Guidelines

Recent commits use short, imperative summaries (for example, `forgot password websiteurl`). Keep each commit focused and use a clearer equivalent such as `Fix forgot-password website URL`. Pull requests should explain behavior changes, configuration or migration needs, and verification performed; link relevant issues and include screenshots for visible Razor/CSS changes.

## Security & Configuration

Never commit credentials, connection strings, or email-provider keys. Keep safe defaults in `Configuration/appsettings.json` and store local secrets with `dotnet user-secrets --project IEsrog/IEsrog.csproj set "Key" "Value"`. Treat logs and invoice output as potentially sensitive customer data.
