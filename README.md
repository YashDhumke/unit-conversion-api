# Unit Conversion API

An ASP.NET Core Web API for converting numeric values between supported units of measurement.

This solution was designed using a layered architecture to keep responsibilities separated and to support future extensibility. While unit definitions are currently hardcoded as allowed by the assessment requirements, the application is structured so that units can later be loaded from a database, configuration file, or external service with minimal changes.

## Architecture

- Controllers are thin HTTP endpoints responsible for handling requests and responses.
- API services adapt API models to domain services.
- Domain service interfaces live in `UnitConversion.Domain.Core`.
- Domain service and repository implementations live in `UnitConversion.Domain.Infrastructure`.
- Domain models live in `UnitConversion.Domain.Models`.
- Request and response API models live in `UnitConversion.ApiModels`.

## Supported Categories

### Length

- Meter (m)
- Kilometer (km)
- Centimeter (cm)
- Millimeter (mm)
- Foot (ft)
- Inch (in)
- Yard (yd)
- Mile (mi)

### Temperature

- Celsius (°C)
- Fahrenheit (°F)
- Kelvin (K)

### Weight / Mass

- Kilogram (kg)
- Gram (g)
- Milligram (mg)
- Pound (lb)
- Ounce (oz)

### Volume

- Liter (L)
- Milliliter (mL)
- Centiliter (cL)
- Cubic Meter (m³)
- Gallon (gal)

### Time

- Second (s)
- Minute (min)
- Hour (hr)
- Day (day)

### Speed

- Meters per Second (m/s)
- Kilometers per Hour (km/h)
- Miles per Hour (mph)
- Knot (kn)

## Run Locally

### Prerequisites

- .NET 8 SDK

### Clone Repository

```powershell
git clone https://github.com/YashDhumke/unit-conversion-api
cd unit-conversion-api
```

### Restore Dependencies

```powershell
dotnet restore
```

### Build the Solution

```powershell
dotnet build
```

### Run the Tests

```powershell
dotnet test
```

### Run the API

```powershell
dotnet run --project .\UnitConversion.Api\UnitConversion.Api.csproj
```

The API will start on the URLs shown in the terminal, commonly:

```text
https://localhost:7260
http://localhost:5078
```

### Swagger UI

After starting the application, open:

```text
https://localhost:7260/swagger
http://localhost:5078/swagger
```

(or the HTTPS URL shown in your terminal)

Swagger can be used to explore and test all available endpoints.

---

## API Endpoints

All endpoints are versioned under `/api/v1/`.

### Health Check

**GET** `/health`

Returns `200 Healthy` when the service is running. Suitable for use with load balancers and container orchestrators.

### Convert a Value

**POST** `/api/v1/UnitConversion/convert`

The optional `precision` field controls the number of decimal places in the result (0–12, default 6).

#### Request

```json
{
  "value": 1,
  "fromUnit": "m",
  "toUnit": "ft",
  "precision": 4
}
```

#### Minimal Request (no precision — defaults to 6 decimal places)

```json
{
  "value": 100,
  "fromUnit": "kph",
  "toUnit": "mph"
}
```

#### Example Response

```json
{
  "category": "length",
  "fromUnit": {
    "code": "m",
    "name": "Meter",
    "symbol": "m",
    "category": "length",
    "factorToBase": 1,
    "offsetToBase": 0
  },
  "toUnit": {
    "code": "ft",
    "name": "Foot",
    "symbol": "ft",
    "category": "length",
    "factorToBase": 0.3048,
    "offsetToBase": 0
  },
  "inputValue": 1,
  "convertedValue": 3.2808
}
```

### List Categories

**GET** `/api/v1/UnitConversion/categories`

Returns all supported conversion categories.

### List Units

**GET** `/api/v1/UnitConversion/units`

Returns all supported units and their metadata.

---

## Design Decisions

### Layered Architecture

The solution is organized into multiple projects to separate concerns and improve maintainability.

```text
UnitConversion.Api
  ApiServices
  Controllers

UnitConversion.ApiModels
  Conversion

UnitConversion.Domain.Core
  Repositories
  Services

UnitConversion.Domain.Infrastructure
  Repositories
  Services

UnitConversion.Domain.Models
  Conversion
```

This structure helps ensure that API concerns, business logic, and data access responsibilities remain independent.

### Repository Abstraction

The application uses the Repository Pattern through `IUnitCatalogRepository`.

The current implementation stores unit definitions in memory because the assessment explicitly allows hardcoded conversion data.

This design allows the repository implementation to be replaced later with:

- SQL Server
- PostgreSQL
- JSON configuration files
- External APIs
- Distributed caching solutions

without changing the business logic.

### Generic Conversion Engine

Each conversion category uses a base unit.

| Category    | Base Unit |
| ----------- | --------- |
| Length      | Meter     |
| Weight/Mass | Kilogram  |
| Temperature | Celsius   |
| Volume      | Liter     |
| Time        | Second    |

Conversions are performed in two steps:

1. Convert the source value to the category's base unit.
2. Convert the base unit value to the target unit.

This approach keeps the conversion logic generic and allows new units to be added without modifying the conversion algorithm.

### Temperature Handling

Temperature conversions require offsets in addition to conversion factors.

Examples:

- Fahrenheit ↔ Celsius
- Kelvin ↔ Celsius

To support this, each unit contains both:

- FactorToBase
- OffsetToBase

This allows the same conversion engine to handle both standard and temperature-based conversions.

---

## Validation

The API validates:

- Unknown unit codes
- Conversions between different categories
- Invalid request payloads
- Invalid precision values

Appropriate HTTP status codes and error messages are returned when validation fails.

---

## Trade-offs

For simplicity and to align with the assessment requirements, unit definitions are stored in an in-memory repository.

### Advantages

- Simple setup
- No database dependency
- Fast lookups
- Easy to understand

### Limitations

- Units cannot be managed dynamically
- Changes require application redeployment
- Not suitable for large-scale administration scenarios

For a production system, unit definitions would likely be stored in a database and managed through an administrative interface.

---

## Future Improvements

Potential enhancements for a production-ready version include:

- Persist unit definitions in a database
- Administrative UI for managing units and categories
- Authentication and authorization
- Caching frequently used conversion data
- Audit logging
- Integration test coverage for the HTTP layer
- Docker support
- CI/CD pipeline integration
- OpenTelemetry monitoring and tracing

---

## Technologies Used

- ASP.NET Core 8
- C#
- Dependency Injection
- Swagger / OpenAPI
- Repository Pattern
- Layered Architecture

---

## Assumptions

- Unit definitions are hardcoded as permitted by the assessment requirements.
- All conversions occur within the same category.
- Precision controls the number of decimal places returned in the response.
- The API is intended for local execution and demonstration purposes.

