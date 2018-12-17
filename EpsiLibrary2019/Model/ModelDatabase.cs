namespace EpsiLibrary2019.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelDatabase : DbContext
    {
        public ModelDatabase()
            : base("name=ModelDatabase")
        {
        }

        public virtual DbSet<DatabaseDB> DatabaseDBs { get; set; }
        public virtual DbSet<DatabaseGroupUser> DatabaseGroupUsers { get; set; }
        public virtual DbSet<DatabaseServerName> DatabaseServerNames { get; set; }
        public virtual DbSet<DatabaseServerType> DatabaseServerTypes { get; set; }
        public virtual DbSet<DatabaseServerUser> DatabaseServerUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatabaseDB>()
                .Property(e => e.NomBD)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseDB>()
                .Property(e => e.UserLogin)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseDB>()
                .Property(e => e.Nom)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseDB>()
                .Property(e => e.Prenom)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseDB>()
                .Property(e => e.Commentaire)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseDB>()
                .HasMany(e => e.DatabaseGroupUser)
                .WithRequired(e => e.DatabaseDB)
                .HasForeignKey(e => e.DbId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DatabaseGroupUser>()
                .Property(e => e.SqlLogin)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseServerName>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseServerName>()
                .Property(e => e.IPLocale)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseServerName>()
                .Property(e => e.NomDNS)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseServerName>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseServerName>()
                .HasMany(e => e.DatabaseDB)
                .WithRequired(e => e.DatabaseServerName)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DatabaseServerName>()
                .HasMany(e => e.DatabaseServerUser)
                .WithRequired(e => e.DatabaseServerName)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DatabaseServerType>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseServerType>()
                .HasMany(e => e.DatabaseServerName)
                .WithRequired(e => e.DatabaseServerType)
                .HasForeignKey(e => e.ServerTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DatabaseServerUser>()
                .Property(e => e.SqlLogin)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseServerUser>()
                .Property(e => e.UserLogin)
                .IsUnicode(false);
        }
    }
}
