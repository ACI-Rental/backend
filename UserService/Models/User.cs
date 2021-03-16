using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    /// <summary>
    /// <b>User</b> class used for EF Core to map its landscape for the database.
    /// </summary>
    public class User
    {
        /// <summary>
        /// [Key]: Identification key for a User entry in the User table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public Guid StudentNumber { get; set; }

        /// <summary>
        /// Token used to refresh the login session of the user.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// A date will be set when the user is banned
        /// Will be null when the user is not banned
        /// </summary>
        public DateTime BannedUntil { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// [Column] gives the colomn a specific name
        /// Used to know what role the users has
        /// Refrences to a Role class
        /// </summary>
        [Required]
        [Column("RoleId")]
        public virtual Guid Role { get; set; }
    }
}
