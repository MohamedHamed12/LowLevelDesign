using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Exceptions;
using RideSharing.Domain.Interfaces;

namespace RideSharing.Application.States;

public class InProgressState : ITripState
{
    public string StateName => "InProgress";

    public void Handle(Trip trip)
    {
        // Trip is in progress
    }

    public bool CanCancel() => false; // Cannot cancel in-progress trips

    public bool CanTransitionTo(string newState)
    {
        return newState == "Completed";
    }
}
