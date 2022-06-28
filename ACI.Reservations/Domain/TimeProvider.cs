using System;

namespace ACI.Reservations.Domain
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }
    }
}
