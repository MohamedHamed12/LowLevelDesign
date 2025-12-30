using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Exceptions;
using RideSharing.Domain.Interfaces;

namespace RideSharing.Application.States;

public class AcceptedState : ITripState
{
    public string StateName => "Accepted";

    public void Handle(Trip trip)
    {
        // In accepted state, driver is on the way
    }

    public bool CanCancel() => true;

    public bool CanTransitionTo(string newState)
    {
        return newState == "InProgress" || newState == "Cancelled";
    }
}
