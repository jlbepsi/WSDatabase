namespace EpsiLibrary2019.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DatabaseServerUser")]
    public partial class DatabaseServerUser
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ServerId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string SqlLogin { get; set; }

        [Required]
        [StringLength(30)]
        public string UserLogin { get; set; }

        public virtual DatabaseServerName DatabaseServerName { get; set; }
    }
}
