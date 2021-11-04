using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models.DTO
{
    /// <summary>
    /// A page of users containing a subset of all users
    /// </summary>
    public class UsersPage
    {
        /// <summary>
        /// The users contained with this page
        /// </summary>
        public IEnumerable<OverviewUser> Users { get; set; }

        /// <summary>
        /// The current page number
        /// Starts with 0
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The total number of users
        /// Not the number of users on the page
        /// </summary>
        public int TotalUsersCount { get; set; }
    
    }
}
