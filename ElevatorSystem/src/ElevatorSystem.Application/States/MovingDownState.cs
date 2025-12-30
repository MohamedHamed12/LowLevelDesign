using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.Enums;
using ElevatorSystem.Domain.Interfaces;

namespace ElevatorSystem.Application.States;

/// <summary>
/// Represents the moving down state of an elevator
/// </summary>
public class MovingDownState : IElevatorState
{
    public string StateName => "Moving Down";

    public void Handle(Elevator elevator)
    {
        var nextFloor = elevator.GetNextTargetFloor();
        
        if (!nextFloor.HasValue)
        {
            // No more requests, become idle
            elevator.SetState(new IdleState());
            return;
        }

        if (nextFloor.Value > elevator.CurrentFloor)
        {
            // Need to change direction
            elevator.SetState(new MovingUpState());
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
                if (next.HasValue && next.Value > elevator.CurrentFloor)
                {
                    elevator.SetState(new MovingUpState());
                }
            }
            else
            {
                elevator.SetState(new IdleState());
            }
        }
        else
        {
            // Continue moving down
            elevator.MoveDown();
        }
    }

    public bool CanAcceptRequest() => true;
}
