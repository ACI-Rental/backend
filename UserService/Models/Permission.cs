using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    /// <summary>
    /// <b>Permission</b> class used for EF Core to map its landscape for the database.
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// [Key]: Identification key for a Permission entry in the Permission table
        /// [Required]: cannot be null
        /// Identification key
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// [Required]: cannot be null
        /// Discribes the permission
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// A list of all the Roles this permission is added to
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }
    }
}
