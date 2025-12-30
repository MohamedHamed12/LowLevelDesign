using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.States;

/// <summary>
/// Represents the moving up state of an elevator
/// </summary>
public class MovingUpState : IElevatorState
{
    public string StateName => "Moving Up";

    public void Handle(Elevator elevator)
    {
        var nextFloor = elevator.GetNextTargetFloor();
        
        if (!nextFloor.HasValue)
        {
            // No more requests, become idle
            elevator.SetState(new IdleState());
            return;
        }

        if (nextFloor.Value < elevator.CurrentFloor)
        {
            // Need to change direction
            elevator.SetState(new MovingDownState());
            return;
        }

        if (nextFloor.Value == elevator.CurrentFloor)
        {
            // Reached target floor
            elevator.OpenDoor();
            Thread.Sleep(2000); // Keep door open for 2 seconds
            elevator.CloseDoor();

            // Check for next request
            if (elevator.HasPendingRequests())
            {
                var next = elevator.GetNextTargetFloor();
                if (next.HasValue && next.Value < elevator.CurrentFloor)
                {
                    elevator.SetState(new MovingDownState());
                }
            }
            else
            {
                elevator.SetState(new IdleState());
            }
        }
        else
        {
            // Continue moving up
            elevator.MoveUp();
        }
    }

    public bool CanAcceptRequest() => true;
}
