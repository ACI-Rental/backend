using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationService.Models.DTO
{
    /// <summary>
    /// Enum to indentify the action on a reservation
    /// </summary>
    public enum ReservationAction
    {
        CANCEL = 0,
        PICKUP = 1,
        RETURN = 2
    }
}
