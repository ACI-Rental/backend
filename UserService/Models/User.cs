using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    /// <summary>
    /// A user of the system.
    /// 
    /// </summary>
    public class User
    {
        [Key]
        [Required]
        public int StudentNumber { get; set; }

        public string RefreshToken { get; set; }

        public DateTime BannedUntil { get; set; }

        [Required]
        public int roleId { get; set; }
    }
}
