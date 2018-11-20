using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MemberIntegralMS.Models
{
    public class TextDb: DbContext
    {
        

        public  DbSet<MemberIntegralMS.Models.Users> CardLevels { get; set; }
    }
}