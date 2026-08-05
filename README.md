# EcoFood API

API REST para el control de despensa doméstica, orientada a reducir el desperdicio de alimentos. Proyecto final del Módulo 4 del Diplomado en .NET — **ODS 12: Producción y consumo responsables**.

## Problema

Muchas personas compran alimentos que terminan venciéndose porque no llevan un control organizado de su despensa. EcoFood API permite registrar alimentos con su fecha de vencimiento, consultar cuáles están próximos a caducar, y sugiere recetas mediante IA para aprovechar los productos disponibles antes de que se dañen.

Ver el detalle completo del planteamiento en [`docs/taller_avances.md`](docs/taller_avances.md).

## Tecnologías

- .NET 8 / ASP.NET Core Web API (Controllers)
- Entity Framework Core 8 + SQLite
- Swashbuckle (Swagger / OpenAPI)
- Groq API (Llama 3) para sugerencia de recetas por IA

## Estructura del proyecto

```
EcoFoodApi/
├── EcoFoodApi.sln
├── docs/
│   └── taller_avances.md
└── src/
    └── EcoFoodApi.Api/
        ├── Models/          # Entidades (Alimento, HistorialConsumo)
        ├── Data/            # AppDbContext (EF Core)
        ├── DTOs/            # DTOs de entrada/salida
        ├── Controllers/     # AlimentosController
        └── Services/        # GroqService (integración IA)
```

## Cómo ejecutar

```bash
cd src/EcoFoodApi.Api
dotnet restore
dotnet run
```

La API queda disponible en la URL indicada en consola, con Swagger UI en `/swagger`.

La base de datos SQLite (`ecofood.db`) se crea automáticamente al iniciar (vía `EnsureCreated`).

### Configurar la API key de Groq

No se debe commitear una API key real en `appsettings.json`. Configúrala con user-secrets:

```bash
cd src/EcoFoodApi.Api
dotnet user-secrets init
dotnet user-secrets set "Groq:ApiKey" "tu-api-key-aqui"
```

Si no se configura una key, el endpoint `POST /api/alimentos/analizar` responde igualmente con un mensaje de fallback (`"Servicio de IA no disponible en este momento"`) en lugar de fallar.

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/alimentos` | Lista todos los alimentos |
| GET | `/api/alimentos/{id}` | Obtiene un alimento por Id |
| POST | `/api/alimentos` | Registra un nuevo alimento |
| PUT | `/api/alimentos/{id}` | Actualiza un alimento |
| DELETE | `/api/alimentos/{id}` | Elimina un alimento (registra historial de consumo) |
| GET | `/api/alimentos/proximos-a-vencer?dias=3` | Alimentos próximos a vencer |
| GET | `/api/alimentos/buscar?nombre=&categoria=` | Búsqueda por nombre y/o categoría |
| GET | `/api/alimentos/estadisticas` | Estadísticas de la despensa |
| POST | `/api/alimentos/analizar` | Sugiere una receta con IA a partir de una lista de productos |

## Equipo

- Sergio Andres Bravo Duran
- Jesus De la hoz Piñeres
