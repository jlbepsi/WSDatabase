namespace EpsiLibrary2019.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ServiceEpsiContext : DbContext
    {
        public ServiceEpsiContext()
            : base("name=ServiceEpsiContext")
        {
        }

        public virtual DbSet<DatabaseDB> DatabaseDBs { get; set; }
        public virtual DbSet<DatabaseGroupUser> DatabaseGroupUsers { get; set; }
        public virtual DbSet<DatabaseServerName> DatabaseServerNames { get; set; }
        public virtual DbSet<DatabaseServerUser> DatabaseServerUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatabaseDB>()
                .Property(e => e.NomBD)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseDB>()
                .Property(e => e.Commentaire)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseDB>()
                .HasMany(e => e.DatabaseGroupUsers)
                .WithRequired(e => e.DatabaseDB)
                .HasForeignKey(e => e.DbId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DatabaseGroupUser>()
                .Property(e => e.SqlLogin)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseGroupUser>()
                .Property(e => e.UserLogin)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseGroupUser>()
                .Property(e => e.UserFullName)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseGroupUser>()
                .Property(e => e.AddedByUserLogin)
                .IsUnicode(false);

            modelBuilder.Entity<DatabaseServerName>()
                .Property(e => e.Code)
                .IsFixedLength()
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
                .HasMany(e => e.DatabaseDBs)
                .WithRequired(e => e.DatabaseServerName)
                .HasForeignKey(e => e.ServerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DatabaseServerName>()
                .HasMany(e => e.DatabaseServerUsers)
                .WithRequired(e => e.DatabaseServerName)
                .HasForeignKey(e => e.ServerId)
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
