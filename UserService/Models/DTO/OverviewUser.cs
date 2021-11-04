using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models.DTO
{
    /// <summary>
    /// The data of a user that is displayed on the users overview
    /// </summary>
    public class OverviewUser
    {
        /// <summary>
        /// The student number of the user
        /// </summary>
        public int StudentNumber { get; set; }

        /// <summary>
        /// A date will be set when the user is banned
        /// It will be null when the user is not banned
        /// </summary>
        public DateTime? BannedUntil { get; set; }

        /// <summary>
        /// Used to know what role the users has
        /// Refrences to a Role class
        /// </summary>
        public Role Role { get; set; }
    }
}
