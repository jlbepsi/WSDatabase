namespace EpsiLibrary2019.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DatabaseDB")]
    public partial class DatabaseDB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DatabaseDB()
        {
            DatabaseGroupUser = new HashSet<DatabaseGroupUser>();
        }

        public int Id { get; set; }

        public int ServerId { get; set; }

        [Required]
        [StringLength(50)]
        public string NomBD { get; set; }

        [Required]
        [StringLength(30)]
        public string UserLogin { get; set; }

        [StringLength(30)]
        public string Nom { get; set; }

        [StringLength(30)]
        public string Prenom { get; set; }

        public DateTime? DateCreation { get; set; }

        public string Commentaire { get; set; }

        public virtual DatabaseServerName DatabaseServerName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatabaseGroupUser> DatabaseGroupUser { get; set; }
    }
}
