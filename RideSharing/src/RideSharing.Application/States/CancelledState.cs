using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Exceptions;
using RideSharing.Domain.Interfaces;

namespace RideSharing.Application.States;

public class CancelledState : ITripState
{
    public string StateName => "Cancelled";

    public void Handle(Trip trip)
    {
        // Trip is cancelled, terminal state
    }

    public bool CanCancel() => false;

    public bool CanTransitionTo(string newState)
    {
        return false; // Terminal state
    }
}
