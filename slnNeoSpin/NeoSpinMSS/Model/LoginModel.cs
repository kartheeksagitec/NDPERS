using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Neo.Model
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }  

        public string Message { get; set; }

        public string Language { get; set; }

        public string LoginWindowName { get; set; }

        public string Banner { get; set; }
        [Required]
        public int PersonId { get; set; }

        public string KTRSID { get; set; }

        public string istrIsActiveAccountSeleted { get; set; }
    }
}
