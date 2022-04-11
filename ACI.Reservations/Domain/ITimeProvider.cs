namespace ACI.Reservations.Domain
{
    public interface ITimeProvider
    {
        public DateTime GetDateTimeNow();
    }
}
