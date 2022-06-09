namespace ACI.Reservations.Models
{
    #pragma warning disable SA1602 // Enumeration items should be documented
    public enum ReservationStatus
    {
        RESERVED = 0,
        ACTIVE = 1,
        RETURNED = 2,
        CANCELLED = 3,
    }
}
