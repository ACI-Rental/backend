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
        /// The Id of the person
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The student number of the user
        /// This should be null if the user is not a student
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// The name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Boolean to show if user is blocked
        /// True if blocked
        /// False if not blocked
        /// </summary>
        public bool Banned { get; set; }

        /// <summary>
        /// The date until which the user is blocked, if the user is blocked
        /// If the user is banned indefinitely, this will be null
        /// If the user is not banned, this should be null as well
        /// </summary>
        public DateTime? BannedUntil { get; set; }

        /// <summary>
        /// Used to know what role the users has
        /// Refrences to a Role class
        /// </summary>
        public Role Role { get; set; }
    }
}
