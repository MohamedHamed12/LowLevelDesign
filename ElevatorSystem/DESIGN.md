# Elevator System - Low-Level Design

## Table of Contents
1. [Requirements](#requirements)
2. [Low-Level Design](#low-level-design)
3. [Design Patterns](#design-patterns)
4. [Architecture](#architecture)
5. [Class Diagram](#class-diagram)

---

## 1. Requirements

### Functional Requirements

1. **Elevator Request Handling**
   - Handle internal requests (button presses inside elevator)
   - Handle external requests (hall call buttons on each floor)
   - Support both up and down direction requests

2. **Elevator Movement**
   - Move up when serving upper floor requests
   - Move down when serving lower floor requests
   - Remain idle when no requests pending
   - Track current floor at all times

3. **Floor Tracking**
   - Track current floor of each elevator
   - Validate floor numbers (within building bounds)
   - Support configurable number of floors

4. **Multiple Elevators Support**
   - Manage multiple elevators independently
   - Coordinate elevator assignments
   - Avoid duplicate assignments

5. **Door Open/Close Behavior**
   - Open doors when reaching destination floor
   - Close doors before movement
   - Implement door safety mechanisms
   - Time-based auto-close

6. **Scheduling & Dispatch Logic**
   - Intelligent elevator selection algorithm
   - Minimize wait time for passengers
   - Optimize for energy efficiency
   - Handle priority requests

### Non-Functional Requirements

1. **Scalability**
   - Support 1 to N elevators
   - Support 1 to M floors
   - Handle high request volumes
   - Efficient resource utilization

2. **Extensibility**
   - Pluggable scheduling algorithms
   - Configurable elevator behavior
   - Easy to add new features
   - Support different elevator types

3. **Maintainability**
   - Clean code structure
   - Clear separation of concerns
   - Well-documented code
   - Consistent naming conventions

4. **Thread Safety**
   - Handle concurrent requests
   - Thread-safe state management
   - No race conditions
   - Deadlock prevention

5. **Testability**
   - Unit testable components
   - Mockable dependencies
   - Integration test support
   - High code coverage potential

---

## 2. Low-Level Design

### Core Entities

#### 1. Elevator
**Responsibilities:**
- Maintain current state (floor, direction, door status)
- Process movement commands
- Manage internal requests
- Notify observers of state changes

**Properties:**
- `Id`: Unique identifier
- `CurrentFloor`: Current position
- `CurrentState`: Idle/Moving/Maintenance
- `Direction`: Up/Down/None
- `DoorStatus`: Open/Closed
- `Capacity`: Maximum weight/persons
- `Requests`: Queue of pending requests

#### 2. Floor
**Responsibilities:**
- Represent a building floor
- Track external requests
- Validate floor numbers

**Properties:**
- `FloorNumber`: Floor identifier
- `HasUpRequest`: Boolean flag
- `HasDownRequest`: Boolean flag

#### 3. Request
**Responsibilities:**
- Encapsulate elevator request details
- Support both internal and external requests

**Properties:**
- `SourceFloor`: Originating floor
- `DestinationFloor`: Target floor (for internal requests)
- `Direction`: Up/Down
- `RequestType`: Internal/External
- `Timestamp`: Request creation time

#### 4. ElevatorController
**Responsibilities:**
- Coordinate multiple elevators
- Process external requests
- Delegate to scheduling strategy
- Monitor system status

**Properties:**
- `Elevators`: List of managed elevators
- `Scheduler`: Scheduling strategy
- `Building`: Building configuration

#### 5. Building
**Responsibilities:**
- Define building configuration
- Validate floor numbers
- Provide building metadata

**Properties:**
- `TotalFloors`: Number of floors
- `MinFloor`: Minimum floor (can be negative)
- `MaxFloor`: Maximum floor

---

## 3. Design Patterns

### 3.1 State Pattern
**Purpose:** Manage elevator states (Idle, Moving, Maintenance)

**Implementation:**
```
IElevatorState (interface)
  ├── IdleState
  ├── MovingUpState
  ├── MovingDownState
  └── MaintenanceState
```

**Benefits:**
- Clean state transitions
- State-specific behavior encapsulation
- Easy to add new states

### 3.2 Strategy Pattern
**Purpose:** Pluggable scheduling algorithms

**Implementation:**
```
ISchedulingStrategy (interface)
  ├── NearestElevatorStrategy
  ├── LeastLoadedStrategy
  ├── ZoneBasedStrategy
  └── OptimalPathStrategy
```

**Benefits:**
- Easy to swap algorithms
- Algorithm testing isolation
- Open/Closed principle adherence

### 3.3 Observer Pattern
**Purpose:** Event notification system

**Implementation:**
```
IElevatorObserver (interface)
  ├── DisplayPanelObserver
  ├── LoggingObserver
  └── MetricsObserver
```

**Benefits:**
- Loose coupling
- Real-time updates
- Multiple subscribers support

### 3.4 Factory Pattern
**Purpose:** Elevator creation and initialization

**Implementation:**
```
IElevatorFactory (interface)
  └── DefaultElevatorFactory
```

**Benefits:**
- Centralized creation logic
- Consistent initialization
- Easy to add elevator types

### 3.5 Command Pattern
**Purpose:** Encapsulate elevator operations

**Implementation:**
```
IElevatorCommand (interface)
  ├── MoveUpCommand
  ├── MoveDownCommand
  ├── OpenDoorCommand
  └── CloseDoorCommand
```

**Benefits:**
- Operation history tracking
- Undo/redo capability
- Request queuing

---

## 4. Architecture

### Layered Architecture

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│   (Console App, API, UI)                │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│         Application Layer               │
│   (Services, Use Cases, DTOs)           │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│           Domain Layer                  │
│   (Entities, Interfaces, Rules)         │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│       Infrastructure Layer              │
│   (Logging, Persistence, External)      │
└─────────────────────────────────────────┘
```

### Project Structure

```
ElevatorSystem/
├── ElevatorSystem.sln
├── DESIGN.md
├── README.md
│
├── src/
│   ├── ElevatorSystem.Domain/
│   │   ├── Entities/
│   │   │   ├── Elevator.cs
│   │   │   ├── Floor.cs
│   │   │   ├── Request.cs
│   │   │   └── Building.cs
│   │   ├── Enums/
│   │   │   ├── ElevatorState.cs
│   │   │   ├── Direction.cs
│   │   │   ├── DoorStatus.cs
│   │   │   └── RequestType.cs
│   │   ├── Interfaces/
│   │   │   ├── IElevatorState.cs
│   │   │   ├── ISchedulingStrategy.cs
│   │   │   ├── IElevatorObserver.cs
│   │   │   └── IElevatorFactory.cs
│   │   └── Exceptions/
│   │       ├── InvalidFloorException.cs
│   │       └── ElevatorOverloadException.cs
│   │
│   ├── ElevatorSystem.Application/
│   │   ├── Services/
│   │   │   ├── ElevatorController.cs
│   │   │   └── ElevatorService.cs
│   │   ├── Strategies/
│   │   │   ├── NearestElevatorStrategy.cs
│   │   │   ├── LeastLoadedStrategy.cs
│   │   │   └── OptimalPathStrategy.cs
│   │   ├── States/
│   │   │   ├── IdleState.cs
│   │   │   ├── MovingUpState.cs
│   │   │   ├── MovingDownState.cs
│   │   │   └── MaintenanceState.cs
│   │   ├── Observers/
│   │   │   ├── DisplayPanelObserver.cs
│   │   │   └── LoggingObserver.cs
│   │   └── Factories/
│   │       └── DefaultElevatorFactory.cs
│   │
│   ├── ElevatorSystem.Infrastructure/
│   │   ├── Logging/
│   │   │   └── FileLogger.cs
│   │   └── Configuration/
│   │       └── ElevatorConfiguration.cs
│   │
│   └── ElevatorSystem.Console/
│       ├── Program.cs
│       ├── Simulation/
│       │   └── ElevatorSimulator.cs
│       └── UI/
│           └── ConsoleDisplay.cs
│
└── tests/
    └── ElevatorSystem.Tests/
        ├── Domain/
        ├── Application/
        └── Integration/
```

---

## 5. Class Diagram

### ASCII Class Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        ElevatorController                        │
├─────────────────────────────────────────────────────────────────┤
│ - elevators: List<Elevator>                                     │
│ - scheduler: ISchedulingStrategy                                │
│ - building: Building                                            │
├─────────────────────────────────────────────────────────────────┤
│ + RequestElevator(floor, direction): void                       │
│ + ProcessRequests(): void                                       │
│ + GetElevatorStatus(id): ElevatorStatus                         │
└─────────────────────────────────────────────────────────────────┘
                             │
                             │ uses
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                     <<interface>>                                │
│                   ISchedulingStrategy                            │
├─────────────────────────────────────────────────────────────────┤
│ + SelectElevator(elevators, request): Elevator                  │
└─────────────────────────────────────────────────────────────────┘
                             △
                             │ implements
            ┌────────────────┼────────────────┐
            │                │                │
┌───────────────────┐ ┌─────────────┐ ┌──────────────┐
│ NearestElevator   │ │ LeastLoaded │ │ OptimalPath  │
│ Strategy          │ │ Strategy    │ │ Strategy     │
└───────────────────┘ └─────────────┘ └──────────────┘


┌─────────────────────────────────────────────────────────────────┐
│                          Elevator                                │
├─────────────────────────────────────────────────────────────────┤
│ - id: int                                                        │
│ - currentFloor: int                                              │
│ - currentState: IElevatorState                                   │
│ - direction: Direction                                           │
│ - doorStatus: DoorStatus                                         │
│ - requests: PriorityQueue<Request>                               │
│ - observers: List<IElevatorObserver>                             │
├─────────────────────────────────────────────────────────────────┤
│ + MoveUp(): void                                                 │
│ + MoveDown(): void                                               │
│ + OpenDoor(): void                                               │
│ + CloseDoor(): void                                              │
│ + AddRequest(request): void                                      │
│ + ProcessNextRequest(): void                                     │
│ + NotifyObservers(): void                                        │
│ + SetState(state): void                                          │
└─────────────────────────────────────────────────────────────────┘
                             │
                             │ has-a
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                     <<interface>>                                │
│                    IElevatorState                                │
├─────────────────────────────────────────────────────────────────┤
│ + Handle(elevator): void                                         │
│ + CanAcceptRequest(): bool                                       │
└─────────────────────────────────────────────────────────────────┘
                             △
                             │ implements
            ┌────────────────┼────────────────┐
            │                │                │
┌───────────────┐  ┌──────────────┐  ┌──────────────┐
│  IdleState    │  │ MovingUpState│  │MovingDownState│
└───────────────┘  └──────────────┘  └──────────────┘


┌─────────────────────────────────────────────────────────────────┐
│                          Request                                 │
├─────────────────────────────────────────────────────────────────┤
│ - sourceFloor: int                                               │
│ - destinationFloor: int?                                         │
│ - direction: Direction                                           │
│ - requestType: RequestType                                       │
│ - timestamp: DateTime                                            │
├─────────────────────────────────────────────────────────────────┤
│ + GetPriority(): int                                             │
└─────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────┐
│                         Building                                 │
├─────────────────────────────────────────────────────────────────┤
│ - totalFloors: int                                               │
│ - minFloor: int                                                  │
│ - maxFloor: int                                                  │
│ - floors: List<Floor>                                            │
├─────────────────────────────────────────────────────────────────┤
│ + IsValidFloor(floor): bool                                      │
│ + GetFloor(number): Floor                                        │
└─────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────┐
│                     <<interface>>                                │
│                   IElevatorObserver                              │
├─────────────────────────────────────────────────────────────────┤
│ + Update(elevator): void                                         │
└─────────────────────────────────────────────────────────────────┘
                             △
                             │ implements
            ┌────────────────┼────────────────┐
            │                │                │
┌───────────────────┐ ┌─────────────┐ ┌──────────────┐
│ DisplayPanel      │ │   Logging   │ │   Metrics    │
│ Observer          │ │  Observer   │ │  Observer    │
└───────────────────┘ └─────────────┘ └──────────────┘
```

---

## 6. SOLID Principles Application

### Single Responsibility Principle (SRP)
- **Elevator**: Manages only its own state and movement
- **ElevatorController**: Coordinates elevators only
- **ISchedulingStrategy**: Handles only scheduling logic
- **Request**: Represents only request data

### Open/Closed Principle (OCP)
- New scheduling strategies can be added without modifying existing code
- New elevator states can be added by implementing IElevatorState
- New observers can be added without changing Elevator class

### Liskov Substitution Principle (LSP)
- Any ISchedulingStrategy implementation can replace another
- Any IElevatorState can substitute another
- All observers are interchangeable

### Interface Segregation Principle (ISP)
- Separate interfaces for different concerns (State, Strategy, Observer)
- No client forced to depend on methods it doesn't use
- Focused, cohesive interfaces

### Dependency Inversion Principle (DIP)
- High-level modules (ElevatorController) depend on abstractions (ISchedulingStrategy)
- Low-level modules (concrete strategies) depend on abstractions
- Dependencies injected via constructors

---

## 7. Key Design Decisions

### 1. Request Queue Management
- Use PriorityQueue for efficient request ordering
- Priority based on distance and direction
- FIFO within same priority level

### 2. Concurrency Handling
- Use locks for state modifications
- Thread-safe collections for request queues
- Async/await for I/O operations

### 3. Scheduling Algorithm
- Default: Nearest elevator with same direction
- Fallback: Idle elevator
- Configurable via strategy pattern

### 4. State Transitions
- Explicit state machine
- Valid transitions enforced
- State-specific behavior isolation

### 5. Error Handling
- Custom exceptions for domain errors
- Validation at boundaries
- Graceful degradation

---

## 8. Performance Considerations

### Optimization Strategies

1. **Request Batching**
   - Group requests in same direction
   - Minimize direction changes
   - Optimize floor stops

2. **Caching**
   - Cache elevator distances
   - Cache building metadata
   - Memoize scheduling decisions

3. **Load Balancing**
   - Distribute load across elevators
   - Zone-based allocation
   - Peak hour optimization

4. **Monitoring**
   - Track average wait time
   - Monitor elevator utilization
   - Identify bottlenecks

---

## 9. Testing Strategy

### Unit Tests
- Test each component in isolation
- Mock dependencies
- Edge case coverage
- State transition testing

### Integration Tests
- Test elevator controller with real elevators
- Test scheduling strategies
- Test concurrent requests

### Scenario Tests
- Rush hour simulation
- Single elevator failure
- Multiple simultaneous requests
- Edge floor requests

---

## 10. Future Enhancements

1. **Advanced Scheduling**
   - Machine learning-based prediction
   - Historical pattern analysis
   - Dynamic zone allocation

2. **Energy Optimization**
   - Sleep mode for idle elevators
   - Regenerative braking simulation
   - Peak/off-peak modes

3. **Accessibility**
   - Priority for accessibility requests
   - Extended door open times
   - Audio/visual feedback

4. **Maintenance**
   - Predictive maintenance alerts
   - Scheduled maintenance mode
   - Performance degradation detection

5. **Real-time Monitoring**
   - Web-based dashboard
   - Real-time metrics
   - Alert system
   - Remote control capabilities

---

## Conclusion

This design provides a robust, scalable, and maintainable elevator system following industry best practices. The use of SOLID principles and design patterns ensures extensibility and testability, while the layered architecture promotes clean separation of concerns.
