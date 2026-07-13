# RutGeo
RutGeo is a mathematical analysis and graphing application developed in C# using Avalonia UI. It dynamically generates, classifies, and visualizes conic equations and piecewise limit functions based on numeric inputs, executing all operations from scratch without relying on the native System.Math library.

# Technical Showcase
- Custom Math Implementation: Rebuilds core mathematical functions (trigonometry, square roots, absolute values, and epsilon constants) via a proprietary RutGeoMath helper.

- Clean Architecture & MVVM: Enforces a strict separation of concerns between RutGeo.Core and RutGeo.UI, utilizing CommunityToolkit.Mvvm and Dependency Injection.

- Mathematical Orchestration: Automates complex algebraic pipelines to generate general equations, transform them to canonical forms, and identify functional discontinuities (Removable, Jump, Infinite).

- Step-by-Step Logging: Implements an incremental explanation logger to document algebraic transformation steps for educational defense and corroboration.

- Custom Plotting Services: Calculates parametric coordinates and limit evaluations independently before delegating the final graphical rendering to ScottPlot.

# Quick Start
## PreRequisites
.NET SDK installed on your machine.

## Run
```bash
dotnet build RutGeo.UI/RutGeo.UI.csproj
dotnet run --project RutGeo.UI/RutGeo.UI.csproj
- MVVM con CommunityToolkit.Mvvm
- Inyección de dependencias vía Microsoft.Extensions.DependencyInjection
```
