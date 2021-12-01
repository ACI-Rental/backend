using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    /// <summary>
    /// <b>UserInfo</b> class used for EF Core to map its landscape for the database.
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// [Key]: Identification key for a Userinfo entry in the UserInfo table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// The name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The studentnumber of the user
        /// If the user is not a student, this should be null
        /// </summary>
        public string Studentnumber { get; set; }

        /// <summary>
        /// Password for the user
        /// </summary>
        public string Password { get; set; }
    }
}
