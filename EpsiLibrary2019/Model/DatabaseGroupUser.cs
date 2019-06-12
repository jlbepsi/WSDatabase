namespace EpsiLibrary2019.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DatabaseGroupUser")]
    public partial class DatabaseGroupUser
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DbId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string SqlLogin { get; set; }

        [StringLength(30)]
        public string UserLogin { get; set; }

        [StringLength(60)]
        public string UserFullName { get; set; }

        public int GroupType { get; set; }

        [Required]
        [StringLength(30)]
        public string AddedByUserLogin { get; set; }

        public virtual DatabaseDB DatabaseDB { get; set; }
    }
}
