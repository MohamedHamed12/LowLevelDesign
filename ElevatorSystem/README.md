# Elevator System - Low-Level Design

A comprehensive elevator system implementation demonstrating **SOLID principles**, **object-oriented design**, and **design patterns** in **.NET 9** using **C#**.

## 🎯 Overview

This project implements a fully functional elevator system capable of managing multiple elevators, handling concurrent requests, and intelligently scheduling elevator movement using various strategies.

## 🏗️ Architecture

The solution follows a **clean layered architecture**:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│   (Console App, User Interface)         │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│         Application Layer               │
│   (Services, Strategies, States)        │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│           Domain Layer                  │
│   (Entities, Interfaces, Rules)         │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│       Infrastructure Layer              │
│   (Logging, Configuration)              │
└─────────────────────────────────────────┘
```

## 📋 Features

### Functional Requirements ✅

- ✅ **Elevator Request Handling** - Internal & external requests
- ✅ **Elevator Movement** - Up, down, and idle states
- ✅ **Floor Tracking** - Real-time position monitoring
- ✅ **Multiple Elevators** - Support for N elevators
- ✅ **Door Operations** - Open/close with timing
- ✅ **Intelligent Scheduling** - Multiple dispatch algorithms

### Non-Functional Requirements ✅

- ✅ **Scalability** - Configurable elevators and floors
- ✅ **Extensibility** - Pluggable scheduling strategies
- ✅ **Maintainability** - Clean code with clear separation
- ✅ **Thread Safety** - Concurrent request handling
- ✅ **Testability** - Comprehensive unit & integration tests

## 🎨 Design Patterns

### 1. **State Pattern**
Manages elevator states with clean transitions:
- `IdleState`
- `MovingUpState`
- `MovingDownState`
- `MaintenanceState`

### 2. **Strategy Pattern**
Pluggable scheduling algorithms:
- `NearestElevatorStrategy` - Selects closest elevator
- `LeastLoadedStrategy` - Balances load across elevators
- `OptimalPathStrategy` - Considers direction and distance

### 3. **Observer Pattern**
Event notification system:
- `LoggingObserver` - Logs all state changes
- `DisplayPanelObserver` - Visual status display
- `MetricsObserver` - Performance tracking

### 4. **Factory Pattern**
Elevator creation:
- `DefaultElevatorFactory` - Standardized elevator creation

## 🔧 SOLID Principles

### Single Responsibility Principle (SRP)
- Each class has one clear purpose
- `Elevator` manages only elevator state
- `ElevatorController` coordinates elevators only

### Open/Closed Principle (OCP)
- New strategies can be added without modifying existing code
- New states can be added by implementing `IElevatorState`

### Liskov Substitution Principle (LSP)
- Any `ISchedulingStrategy` can replace another
- All state implementations are interchangeable

### Interface Segregation Principle (ISP)
- Focused interfaces: `IElevatorState`, `ISchedulingStrategy`, `IElevatorObserver`
- No client forced to depend on unused methods

### Dependency Inversion Principle (DIP)
- `ElevatorController` depends on `ISchedulingStrategy` abstraction
- Easy to swap implementations

## 🚀 Getting Started

### Prerequisites

- .NET 9 SDK or later
- C# 12

### Build the Solution

```bash
cd /workspaces/LowLevelDesign/ElevatorSystem
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Run the Application

#### Interactive Mode
```bash
dotnet run --project src/ElevatorSystem.Console
```

#### Demo Mode
```bash
dotnet run --project src/ElevatorSystem.Console -- --demo
```

## 📁 Project Structure

```
ElevatorSystem/
├── src/
│   ├── ElevatorSystem.Domain/           # Core business entities
│   │   ├── Entities/                    # Elevator, Building, Request, Floor
│   │   ├── Enums/                       # ElevatorState, Direction, DoorStatus
│   │   ├── Interfaces/                  # Core abstractions
│   │   └── Exceptions/                  # Domain exceptions
│   │
│   ├── ElevatorSystem.Application/      # Business logic
│   │   ├── Services/                    # ElevatorController
│   │   ├── Strategies/                  # Scheduling algorithms
│   │   ├── States/                      # State implementations
│   │   ├── Observers/                   # Event observers
│   │   └── Factories/                   # Object creation
│   │
│   ├── ElevatorSystem.Infrastructure/   # External concerns
│   │   ├── Logging/                     # File logging
│   │   └── Configuration/               # Settings
│   │
│   └── ElevatorSystem.Console/          # UI layer
│       └── Program.cs                   # Entry point
│
├── tests/
│   └── ElevatorSystem.Tests/
│       ├── Domain/                      # Domain tests
│       ├── Application/                 # Application tests
│       └── Integration/                 # Integration tests
│
├── DESIGN.md                            # Detailed design document
└── README.md                            # This file
```

## 🎮 Usage Examples

### Example 1: Request an Elevator

```csharp
// Request elevator to floor 5, going up
controller.RequestElevator(5, Direction.Up);
```

### Example 2: Select Destination Inside Elevator

```csharp
// In elevator #1, select floor 7
controller.RequestFloor(1, 7);
```

### Example 3: Change Scheduling Strategy

```csharp
var strategy = new LeastLoadedStrategy();
var controller = new ElevatorController(elevators, building, strategy);
```

## 🧪 Testing

The project includes comprehensive tests:

- **Unit Tests** - Domain entities, strategies, states
- **Integration Tests** - Controller coordination
- **37 total tests** - All passing ✅

Run specific test categories:
```bash
# Domain tests only
dotnet test --filter "FullyQualifiedName~Domain"

# Application tests only
dotnet test --filter "FullyQualifiedName~Application"
```

## 📊 Configuration

Customize via `ElevatorConfiguration`:

```csharp
var config = new ElevatorConfiguration
{
    NumberOfElevators = 3,
    TotalFloors = 10,
    MinFloor = 0,
    ElevatorCapacity = 10,
    SchedulingStrategy = "OptimalPath",
    EnableLogging = true,
    EnableMetrics = true
};
```

## 🔍 Key Classes

### Elevator
Core entity managing elevator state and behavior.

### ElevatorController
Coordinates multiple elevators and processes requests.

### Request
Encapsulates elevator request details.

### Building
Represents building structure and validates floors.

## 📈 Performance Metrics

The system tracks:
- Total elevator movements
- Door operations
- Unique floors visited
- Request processing time

## 🛠️ Future Enhancements

- [ ] Machine learning-based scheduling
- [ ] Energy optimization algorithms
- [ ] Web-based monitoring dashboard
- [ ] Predictive maintenance alerts
- [ ] Zone-based allocation
- [ ] Peak/off-peak mode optimization

## 📚 Documentation

See [DESIGN.md](DESIGN.md) for:
- Detailed requirements
- UML diagrams
- Design decisions
- Performance considerations
- Testing strategy

## 👥 Contributing

This is a demonstration project showcasing software architecture best practices.

## 📄 License

Educational/Portfolio Project

## 🙏 Acknowledgments

Built following industry best practices for:
- Clean Architecture
- Domain-Driven Design
- SOLID Principles
- Design Patterns
- Test-Driven Development

---

**Built with ❤️ using .NET 9 and C# 12**
