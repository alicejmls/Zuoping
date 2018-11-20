using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MemberIntegralMS.Models
{
    public class Users
    {
        [Key]
        public int U_ID { get; set; }
        public Nullable<int> S_ID { get; set; }
        public string U_LoginName { get; set; }
        public string U_Password { get; set; }
        public string U_RealName { get; set; }
        public string U_Sex { get; set; }
        public string U_Telephone { get; set; }
        public Nullable<int> U_Role { get; set; }
        public Nullable<bool> U_CanDelete { get; set; }
    }
}