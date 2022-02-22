using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models.DTO
{
    public class UserBlockActionModel
    {
        /// <summary>
        /// Id of the user
        /// </summary>
        public int userId { get; set; }

        /// <summary>
        /// Action done to the user
        /// 0: Block
        /// 1: Unblock
        /// </summary>
        public UserBlockAction Action { get; set; }

        /// <summary>
        /// De date until which the user will be blocked
        /// </summary>
        public DateTime blockUntil { get; set; }
    }
}
