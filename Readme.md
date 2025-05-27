# PortfolioValuationApp

A .NET 6 console application that calculates an investor’s portfolio value on a given date using CSV-based data.  

**Key Aspects:**
- **Layered architecture**: Domain / Infrastructure / Application / ConsoleApp  
- **SOLID & Clean Code**: single responsibility, interface–driven, open/closed  
- **Design Patterns**: Repository Pattern, Dependency Injection, Lazy Initialization  
- **No hard-coded values**: file paths, separators, log messages centralized in constants  
- **Comprehensive testing**: xUnit + Moq, covers CSV parsing, repositories, service logic  

---


##  Design Patterns & Principles

- **Repository Pattern**  
  - Abstracts data access behind interfaces (`IInvestmentRepository`, etc.)  
  - Allows swapping CSV for database/web-API implementations without changing business logic

- **Dependency Injection**  
  - All dependencies (repos, logger, service) registered in `Program.cs`  
  - Encourages testability by mocking interfaces (Moq in tests)

- **Lazy Initialization**  
  - CSV files are loaded on first call (not in constructor) to avoid unnecessary I/O and improve startup time

- **No Hard-Coded Values**  
  - File paths, CSV separators, and log message templates defined in constants or configuration  
  - Ensures easy changes (e.g. change `;` to `,` by updating one constant)

- **SOLID / Clean Code**  
  - **Single Responsibility**: each class has one reason to change  
  - **Open/Closed**: new repository implementations can be added without modifying existing code  
  - **Interface Segregation**: small, focused interfaces  
  - **Dependency Inversion**: high-level modules depend on abstractions, not concretes  

---

##  Testing Strategy

**Test Coverage:**
1. **CsvReaderTests**  
   - Normal parsing  
   - Empty file → returns empty list (no exception swallowing except parse errors)  
   - Missing file → throws `FileNotFoundException`  
   - Malformed rows → skipped, others parsed  
   - Extra columns → ignored

2. **InvestmentRepositoryTests**  
   - Lazy load from temp CSV  
   - `GetAll`, `GetById`, `GetInvestmentsByInvestor`, `GetInvestmentsByFund`

3. **TransactionRepositoryTests**  
   - Date filtering (`<= referenceDate`), investment filtering

4. **QuoteRepositoryTests**  
   - Exact date match, nearest prior date, unknown ISIN, no data

5. **PortfolioServiceTests**  
   - No investments → returns 0  
   - Single stock → shares × price  
   - Real estate → estate + building  
   - Fund → sum(sub-investments) × total percentage  
   - Unknown type → logs warning, returns 0

**Test Tools & Setup:**
- **xUnit** for assertions  
- **Moq** for mocking repositories & CSV reader where needed  
- Temporary CSV files in `Path.GetTempPath()`, cleaned up via `IDisposable`  
- **Test SDK** & **xunit.runner.visualstudio** in test `.csproj`  

---

##  How to Run

1. **Build & restore**  
   ```bash
   dotnet restore
   dotnet build
example
Investor0
2016-01-01



