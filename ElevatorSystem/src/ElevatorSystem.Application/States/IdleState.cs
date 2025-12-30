using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.States;

/// <summary>
/// Represents the idle state of an elevator
/// </summary>
public class IdleState : IElevatorState
{
    public string StateName => "Idle";

    public void Handle(Elevator elevator)
    {
        if (!elevator.HasPendingRequests())
        {
            // Remain idle
            return;
        }

        var nextFloor = elevator.GetNextTargetFloor();
        if (nextFloor.HasValue)
        {
            if (nextFloor.Value > elevator.CurrentFloor)
            {
                elevator.SetState(new MovingUpState());
            }
            else if (nextFloor.Value < elevator.CurrentFloor)
            {
                elevator.SetState(new MovingDownState());
            }
        }
    }

    public bool CanAcceptRequest() => true;
}
