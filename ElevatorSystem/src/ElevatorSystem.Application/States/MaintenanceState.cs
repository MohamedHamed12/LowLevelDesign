using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.States;

/// <summary>
/// Represents the maintenance state of an elevator
/// </summary>
public class MaintenanceState : IElevatorState
{
    public string StateName => "Maintenance";

    public void Handle(Elevator elevator)
    {
        // Do nothing - elevator is out of service
    }

    public bool CanAcceptRequest() => false;
}
