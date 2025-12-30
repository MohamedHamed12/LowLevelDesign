using RideSharing.Domain.Entities;
using RideSharing.Domain.Enums;
using RideSharing.Domain.Exceptions;
using RideSharing.Domain.Interfaces;

namespace RideSharing.Application.States;

public class RequestedState : ITripState
{
    public string StateName => "Requested";

    public void Handle(Trip trip)
    {
        // In requested state, waiting for driver assignment
    }

    public bool CanCancel() => true;

    public bool CanTransitionTo(string newState)
    {
        return newState == "Accepted" || newState == "Cancelled";
    }
}
