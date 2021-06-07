using ReservationService.Models.DTO;

namespace ReservationService.Controllers
{
    public class ReservationActionModel
    {
        /// <summary>
        /// Id of the reservation
        /// </summary>
        public int ReservationId { get; set; }

        /// <summary>
        /// Action that is done to the reservation
        /// 0: Cancel
        /// 1: Pickup
        /// 2: Return
        /// </summary>
        public ReservationAction Action { get; set; }
    }
}