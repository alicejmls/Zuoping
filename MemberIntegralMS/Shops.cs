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
    
    public partial class Shops
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Shops()
        {
            this.ConsumeOrders = new HashSet<ConsumeOrders>();
            this.ExchangGifts = new HashSet<ExchangGifts>();
            this.ExchangLogs = new HashSet<ExchangLogs>();
            this.MemCards = new HashSet<MemCards>();
            this.TransferLogs = new HashSet<TransferLogs>();
            this.Users = new HashSet<Users>();
        }
    
        public int S_ID { get; set; }
        public string S_Name { get; set; }
        public Nullable<int> S_Category { get; set; }
        public string S_ContactName { get; set; }
        public string S_ContactTel { get; set; }
        public string S_Address { get; set; }
        public string S_Remark { get; set; }
        public Nullable<bool> S_IsHasSetAdmin { get; set; }
        public Nullable<System.DateTime> S_CreateTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConsumeOrders> ConsumeOrders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExchangGifts> ExchangGifts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExchangLogs> ExchangLogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MemCards> MemCards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TransferLogs> TransferLogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Users> Users { get; set; }
    }
}
