using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;
using System.Collections.Concurrent;

namespace ElevatorSystem.Domain.Entities;

/// <summary>
/// Represents an elevator in the system
/// </summary>
public class Elevator
{
    private readonly object _lock = new();
    private readonly List<IElevatorObserver> _observers = new();
    private readonly SortedSet<int> _upRequests = new();
    private readonly SortedSet<int> _downRequests = new();
    private IElevatorState? _currentState;

    /// <summary>
    /// Gets the unique identifier for this elevator
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the current floor
    /// </summary>
    public int CurrentFloor { get; private set; }

    /// <summary>
    /// Gets the current direction
    /// </summary>
    public Direction Direction { get; private set; }

    /// <summary>
    /// Gets the current door status
    /// </summary>
    public DoorStatus DoorStatus { get; private set; }

    /// <summary>
    /// Gets the maximum capacity
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the current load
    /// </summary>
    public int CurrentLoad { get; private set; }

    /// <summary>
    /// Gets the total floors in the building
    /// </summary>
    public int TotalFloors { get; }

    /// <summary>
    /// Gets whether the elevator is available
    /// </summary>
    public bool IsAvailable => _currentState?.CanAcceptRequest() ?? false;

    /// <summary>
    /// Gets the current state name
    /// </summary>
    public string CurrentStateName => _currentState?.StateName ?? "Unknown";

    public Elevator(int id, int totalFloors, int capacity = 10)
    {
        Id = id;
        TotalFloors = totalFloors;
        Capacity = capacity;
        CurrentFloor = 0; // Start at ground floor
        Direction = Direction.None;
        DoorStatus = DoorStatus.Closed;
        CurrentLoad = 0;
    }

    /// <summary>
    /// Sets the current state of the elevator
    /// </summary>
    public void SetState(IElevatorState state)
    {
        lock (_lock)
        {
            _currentState = state;
            NotifyObservers();
        }
    }

    /// <summary>
    /// Adds a request to the elevator
    /// </summary>
    public void AddRequest(int targetFloor)
    {
        lock (_lock)
        {
            if (targetFloor == CurrentFloor)
                return;

            if (targetFloor > CurrentFloor)
            {
                _upRequests.Add(targetFloor);
            }
            else
            {
                _downRequests.Add(targetFloor);
            }
        }
    }

    /// <summary>
    /// Gets the next target floor based on current direction
    /// </summary>
    public int? GetNextTargetFloor()
    {
        lock (_lock)
        {
            if (Direction == Direction.Up && _upRequests.Count > 0)
            {
                return _upRequests.Min;
            }
            else if (Direction == Direction.Down && _downRequests.Count > 0)
            {
                return _downRequests.Max;
            }
            else if (_upRequests.Count > 0)
            {
                Direction = Direction.Up;
                return _upRequests.Min;
            }
            else if (_downRequests.Count > 0)
            {
                Direction = Direction.Down;
                return _downRequests.Max;
            }

            Direction = Direction.None;
            return null;
        }
    }

    /// <summary>
    /// Checks if there are any pending requests
    /// </summary>
    public bool HasPendingRequests()
    {
        lock (_lock)
        {
            return _upRequests.Count > 0 || _downRequests.Count > 0;
        }
    }

    /// <summary>
    /// Moves the elevator up one floor
    /// </summary>
    public void MoveUp()
    {
        lock (_lock)
        {
            if (CurrentFloor < TotalFloors - 1)
            {
                CurrentFloor++;
                Direction = Direction.Up;
                CheckAndRemoveRequest();
                NotifyObservers();
            }
        }
    }

    /// <summary>
    /// Moves the elevator down one floor
    /// </summary>
    public void MoveDown()
    {
        lock (_lock)
        {
            if (CurrentFloor > 0)
            {
                CurrentFloor--;
                Direction = Direction.Down;
                CheckAndRemoveRequest();
                NotifyObservers();
            }
        }
    }

    /// <summary>
    /// Opens the elevator doors
    /// </summary>
    public void OpenDoor()
    {
        lock (_lock)
        {
            DoorStatus = DoorStatus.Opening;
            Thread.Sleep(500); // Simulate door opening
            DoorStatus = DoorStatus.Open;
            CheckAndRemoveRequest();
            NotifyObservers();
        }
    }

    /// <summary>
    /// Closes the elevator doors
    /// </summary>
    public void CloseDoor()
    {
        lock (_lock)
        {
            DoorStatus = DoorStatus.Closing;
            Thread.Sleep(500); // Simulate door closing
            DoorStatus = DoorStatus.Closed;
            NotifyObservers();
        }
    }

    /// <summary>
    /// Checks if current floor has a request and removes it
    /// </summary>
    private void CheckAndRemoveRequest()
    {
        _upRequests.Remove(CurrentFloor);
        _downRequests.Remove(CurrentFloor);
    }

    /// <summary>
    /// Adds an observer to be notified of state changes
    /// </summary>
    public void AddObserver(IElevatorObserver observer)
    {
        lock (_lock)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }
    }

    /// <summary>
    /// Removes an observer
    /// </summary>
    public void RemoveObserver(IElevatorObserver observer)
    {
        lock (_lock)
        {
            _observers.Remove(observer);
        }
    }

    /// <summary>
    /// Notifies all observers of state change
    /// </summary>
    private void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }

    /// <summary>
    /// Processes the current state
    /// </summary>
    public void ProcessState()
    {
        _currentState?.Handle(this);
    }

    /// <summary>
    /// Gets the number of pending requests
    /// </summary>
    public int GetPendingRequestCount()
    {
        lock (_lock)
        {
            return _upRequests.Count + _downRequests.Count;
        }
    }

    /// <summary>
    /// Calculates distance to a floor
    /// </summary>
    public int DistanceToFloor(int floor)
    {
        return Math.Abs(CurrentFloor - floor);
    }

    public override string ToString()
    {
        return $"Elevator {Id}: Floor {CurrentFloor}, Direction: {Direction}, " +
               $"Door: {DoorStatus}, State: {CurrentStateName}, " +
               $"Pending: {GetPendingRequestCount()}";
    }
}
