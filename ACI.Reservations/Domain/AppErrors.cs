namespace ACI.Reservations.Domain
{
    public static class AppErrors
    {
        // Product errors
        public static readonly IError ProductNotFoundError = new IError(100, "Product not found");
        public static readonly IError ProductDoesNotExist = new IError(101, "The id is not linked to an existing product");

        // Reservation errors
        public static readonly IError InvalidStartDate = new IError(201, "Invalid start date");
        public static readonly IError InvalidEndDate = new IError(200, "Invalid end date");
        public static readonly IError EndDateBeforeStartDate = new IError(202, "End date cannot be before start date");
        public static readonly IError StartDateInWeekend = new IError(203, "Start date can not be in weekend");
        public static readonly IError EndDateInWeekend = new IError(204, "End date can not be in weekend");
        public static readonly IError ReservationIsTooLong = new IError(205, "Can not reserve a product for more than 5 days");
        public static readonly IError ReservationIsOverlapping = new IError(206, "There is already an existing reservation for this item at that date");
        public static readonly IError FailedToFindReservation = new IError(207, "The application failed to find a(ny) reservation");
        public static readonly IError FailedToSaveReservation = new IError(208, "The application failed to save the reservation");
        public static readonly IError InvalidReservationAction = new IError(209, "The action that was executed was invalid");
        public static readonly IError InvalidPickupDate = new IError(210, "The pickup date is either before or after the reservation");
        public static readonly IError InvalidReturnDate = new IError(211, "The return date is either before or after the reservation");
    }
}
