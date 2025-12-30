using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Exceptions;
using RideSharing.Domain.Interfaces;

namespace RideSharing.Application.States;

public class CompletedState : ITripState
{
    public string StateName => "Completed";

    public void Handle(Trip trip)
    {
        // Trip is completed, terminal state
    }

    public bool CanCancel() => false;

    public bool CanTransitionTo(string newState)
    {
        return false; // Terminal state
    }
}
