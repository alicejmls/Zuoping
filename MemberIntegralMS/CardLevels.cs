//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MemberIntegralMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class CardLevels
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CardLevels()
        {
            this.MemCards = new HashSet<MemCards>();
        }
    
        public int CL_ID { get; set; }
        public string CL_LevelName { get; set; }
        public string CL_NeedPoint { get; set; }
        public Nullable<double> CL_Point { get; set; }
        public Nullable<double> CL_Percent { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MemCards> MemCards { get; set; }
    }
}