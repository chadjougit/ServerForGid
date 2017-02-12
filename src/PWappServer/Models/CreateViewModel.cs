using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWappServer.Models
{
    /// <summary>
    /// Class required to create a new user.
    /// </summary>
    public class SignUpModel
    {
        public string email { get; set; }

        public string username { get; set; }

        public string name { get; set; }

        public string password { get; set; }

    }
}
