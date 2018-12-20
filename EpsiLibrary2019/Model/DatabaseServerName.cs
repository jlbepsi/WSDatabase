namespace EpsiLibrary2019.Model
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DatabaseServerName")]
    public partial class DatabaseServerName
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DatabaseServerName()
        {
            DatabaseDB = new HashSet<DatabaseDB>();
            DatabaseServerUser = new HashSet<DatabaseServerUser>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ServerId { get; set; }

        public int ServerTypeId { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string IPLocale { get; set; }

        [Required]
        [StringLength(50)]
        public string NomDNS { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatabaseDB> DatabaseDB { get; set; }

        public virtual DatabaseServerType DatabaseServerType { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatabaseServerUser> DatabaseServerUser { get; set; }
    }
}
