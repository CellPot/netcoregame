using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GameUserDB.Data
{
    public class ApplicationUser: IdentityUser
    { 
        public int Score { get; set; }
        public int Position { get; set; }
        public string Sequence { get; set; }
    }
}
