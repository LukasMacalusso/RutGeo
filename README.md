# EID Cálculo I — RutGeo

Aplicación Avalonia UI para el proyecto MAT1186. Valida RUTs chilenos, genera ecuaciones cónicas a partir de sus dígitos, las clasifica y transforma a forma canónica, las grafica, y analiza funciones por tramos con límites y continuidad.

## Arquitectura

```
RutGeo/
├── RutGeo.Core/          # Modelo, servicios, interfaces (sin dependencias UI)
└── RutGeo.UI/            # Vistas, ViewModels, helpers de renderizado
```

## Scripts

### RutGeo.Core (Modelo / Servicios)

| Script | Capa | Responsabilidad |
|---|---|---|
| `Helpers/RutGeoMath.cs` | Helper | Implementación manual de Abs, Sqrt, Sin, Cos, Tan, PI, IsInfinity, IsNaN, IsNearZero y constantes epsilon. **No usa System.Math** |
| `Helpers/RutDigitHelper.cs` | Helper | `GetPaddedDigits(body)` -> padding a 8 dígitos, `GetLimitCondition(d8)` -> `d8 % 3` |
| `Interfaces/Validation/IRutValidator.cs` | Interface | Contrato: `Validate(string)` -> `RutValidatorResult` |
| `Interfaces/Generators/IRutEquationGenerator.cs` | Interface | Contrato: `GenerateGeneralEquation(RutValidatorResult)` -> `GeneralEquation` |
| `Interfaces/Conics/IEquationTransformer.cs` | Interface | Contrato: `TransformToCanonical` + `TransformToGeneral` |
| `Interfaces/Conics/IConicElementsFactory.cs` | Interface | Contrato: fábrica de CircleElements, EllipseElements, HyperbolaElements, ParabolaElements |
| `Interfaces/Functions/IFunctionAnalyzer.cs` | Interface | Contrato: `AnalyzeFunctionFromRut(RutValidatorResult)` -> `LimitAnalysisResult` |
| `Interfaces/Common/IExplanationLogger.cs` | Interface | Contrato: log paso a paso (StartProcess, AppendStep, AppendEquation, GetFullLog, Clear) |
| `Interfaces/Conics/IConicOrchestrator.cs` | Interface | Contrato: `Execute(RutValidatorResult)` -> `ConicOrchestrationResult` |
| `Interfaces/Functions/ILimitOrchestrator.cs` | Interface | Contrato: `Execute(RutValidatorResult)` -> `LimitOrchestrationResult` |
| `Services/Validation/RutValidator.cs` | Servicio | Valida RUT chileno (módulo 11), normaliza entrada, extrae cuerpo y dígito verificador |
| `Services/Generators/RutEquationGenerator.cs` | Servicio | Genera ecuación general Ax²+By²+Cx+Dy+E=0 a partir de los dígitos del RUT |
| `Services/Conics/ToCanonicalTransformer.cs` | Servicio | Transforma ecuación general → canónica para cada tipo de cónica. Registra pasos en el logger |
| `Services/Conics/ToGeneralTransformer.cs` | Servicio | Transforma ecuación canónica → general (inversa) |
| `Services/Conics/ConicElementsFactory.cs` | Servicio | Crea objetos con propiedades geométricas de cada cónica (centro, vértices, focos, radios, excentricidad, etc.) |
| `Services/Functions/FunctionAnalyzer.cs` | Servicio | Analiza la función de límites según los dígitos del RUT: determina tipo de discontinuidad, límites laterales, expresión. Expone `EvaluateAt()` estático |
| `Services/Common/ExplanationLogger.cs` | Servicio | Implementación con StringBuilder del logger incremental de pasos |
| `Services/Conics/ConicOrchestrator.cs` | Servicio | Orquesta el pipeline cónico: genera ecuación -> clasifica -> transforma a canónica -> revierte a general -> extrae pasos del log |
| `Services/Functions/LimitOrchestrator.cs` | Servicio | Orquesta el pipeline de límites: analiza función -> genera descripción -> genera tabla de valores laterales |
| `Models/Conic.cs` | Modelo | Clasifica la cónica (Parábola, Hipérbola, Circunferencia, Elipse, Desconocida) según coeficientes |
| `Models/ConicElements.cs` | Modelo | Records: Point2D, Line2D. Classes: CircleElements, EllipseElements, HyperbolaElements, ParabolaElements |
| `Models/Equations/GeneralEquation.cs` | Modelo | Ax² + By² + Cx + Dy + E = 0 con EquationString formateado |
| `Models/Equations/CanonicalEquation.cs` | Modelo | ConicType + FormattedString + Elements |
| `Models/Results/RutValidatorResult.cs` | Modelo | IsValid, RutBody, Dv |
| `Models/Results/LimitAnalysisResult.cs` | Modelo | Resultado del análisis de límites: Digits, Condition, CriticalPoint, límites, justificación |
| `Models/Results/ConicOrchestrationResult.cs` | Modelo | Resultado del orquestador cónico: GeneralEquation, Conic, CanonicalEquation, TransformationSteps |
| `Models/Results/LimitOrchestrationResult.cs` | Modelo | Resultado del orquestador de límites: AnalysisResult, CaseDescription, tabla de valores |
| `Models/Types/ConicType.cs` | Modelo | Enum: Parabola, Hyperbola, Circunferencia, Elipse, Desconocida |
| `Models/Types/DiscontinuityType.cs` | Modelo | Enum: None, Removable, Jump, Infinite |

### RutGeo.UI (Vistas / ViewModels)

| Script | Capa | Responsabilidad |
|---|---|---|
| `Program.cs` | Entry | BuildAvaloniaApp().StartWithClassicDesktopLifetime |
| `App.axaml.cs` | DI | Configura contenedor de dependencias, registra servicios e interfaces |
| `ViewLocator.cs` | View | Resuelve View a partir de ViewModel por convención de nombres |
| `ViewModels/ViewModelBase.cs` | ViewModel | Clase base abstracta (ObservableObject) |
| `ViewModels/MainWindowViewModel.cs` | ViewModel | VM principal: valida RUT, orquesta los VMs hijos (ConicAnalysisVM, LimitAnalysisVM), comandos Analyze/ClearAll/CorroborateAll/ToggleSolutions |
| `ViewModels/ConicAnalysisViewModel.cs` | ViewModel | VM hijo de cónicas: bindea GeneralEquation, Conic, CanonicalEquation, pasos de transformación. Contiene ConicDefenseViewModel |
| `ViewModels/LimitAnalysisViewModel.cs` | ViewModel | VM hijo de límites: bindea resultado del análisis, descripción, expresión, tabla de valores laterales. Contiene LimitDefenseViewModel |
| `ViewModels/ConicDefenseViewModel.cs` | ViewModel | Panel de defensa de cónicas: campos para que el alumno ingrese centro, vértices, focos, ejes, directriz y corrobora |
| `ViewModels/LimitDefenseViewModel.cs` | ViewModel | Panel de defensa de límites: campos para límites laterales, existencia, f(a), continuidad, tipo y corrobora |
| `ViewModels/DefenseField.cs` | ViewModel | Clase reutilizable ObservableObject con UserValue/ExpectedValue/Status/IsVisible + método Corroborate() |
| `ViewModels/ValuePoint.cs` | ViewModel | DTO para tabla de valores: X (double), Fx (string formateado) |
| `Views/MainWindow.axaml.cs` | View | Window principal: maneja modo cónicas/límites, toggle paneles, actualiza gráfico al cambiar propiedades del VM |
| `Views/Graphic.axaml.cs` | View | View del gráfico ScottPlot. Delega renderizado a ConicPlotService y LimitPlotService |
| `Views/Analysis.axaml.cs` | View | Panel de análisis del caso (cónica o límite). Switchea visibilidad. Renombrado desde Description |
| `Views/Defense.axaml.cs` | View | Panel de defensa. Switchea entre conic-defense y limit-defense |
| `Views/Solutions.axaml.cs` | View | Panel de soluciones. Switchea visibilidad |
| `Views/Mode.axaml.cs` | View | Botones de selector: Cónicas / Límites |
| `Views/RutInputView.axaml.cs` | View | Input de RUT + botón toggle del log |
| `Views/RutLog.axaml.cs` | View | Panel del log de explicaciones |
| `Views/ValuesTable.axaml.cs` | View | Tabla de valores laterales con botón mostrar/ocultar |
| `Helpers/ConicPlotter.cs` | Helper | Genera puntos (x,y) paramétricos para graficar cada tipo de cónica (cálculo puro, sin ScottPlot) |
| `Helpers/LimitPlotter.cs` | Helper | Genera puntos (x,y) para graficar la función de límites (3 casos). Cálculo puro sin ScottPlot |
| `Helpers/ConicPlotService.cs` | Helper | Renderiza curvas cónicas en AvaPlot: llama a ConicPlotter, segmenta por NaN, autoescala |
| `Helpers/LimitPlotService.cs` | Helper | Renderiza funciones de límite en AvaPlot: llama a LimitPlotter, autoescala |
| `Helpers/PlotRenderer.cs` | Helper | Extension methods de AvaPlot: DrawAxes(), AddScatterNoMarkers(), ClearAndReset() |

## Reglas del proyecto

- **No se permite System.Math** — todos los cálculos (trig, raíces, PI, etc.) son implementación manual en RutGeoMath
- Formato Allman consistente en todo el código
- Namespaces file-scoped (`namespace X.Y;`)
- MVVM con CommunityToolkit.Mvvm
- Inyección de dependencias vía Microsoft.Extensions.DependencyInjection
