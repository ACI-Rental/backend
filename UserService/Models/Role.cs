using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    /// <summary>
    /// <b>Role</b> class used for EF Core to map its landscape for the database.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// [Key]: Identification key for a Role entry in the Role table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Discribes the role
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// A list of all the permissions the role has
        /// </summary>
        public List<Guid> Permissions { get; set; }
    }
}
