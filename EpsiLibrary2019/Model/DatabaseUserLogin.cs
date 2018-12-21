namespace EpsiLibrary2019.Model
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DatabaseUserLogin")]
    public partial class DatabaseUserLogin
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DatabaseUserLogin()
        {
            DatabaseServerUser = new HashSet<DatabaseServerUser>();
        }

        [Key]
        [StringLength(30)]
        public string UserLogin { get; set; }

        [Required]
        [StringLength(60)]
        public string UserNom { get; set; }

        [Required]
        [StringLength(30)]
        public string UserPrenom { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatabaseServerUser> DatabaseServerUser { get; set; }
    }
}
