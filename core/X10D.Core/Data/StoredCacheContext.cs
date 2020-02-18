using Microsoft.EntityFrameworkCore;

namespace X10D.Core.Data
{
    internal sealed class StoredCacheContext : DbContext
    {
        public StoredCacheContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new System.ArgumentNullException(nameof(modelBuilder));
            }

            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<StoredCacheString>()
                .HasKey(e => e.Key);
        }

        internal DbSet<StoredCacheString> Strings { get; set; }

        internal string this[string key]
        {
            get
            {
                return Find<StoredCacheString>(key)?.Value;
            }
            set
            {
                if (Find<StoredCacheString>(key) is StoredCacheString note)
                {
                    note.Value = value;
                    Update(note);
                }
                else
                {
                    Add(new StoredCacheString(key, value));
                }
                SaveChanges();
            }
        }
    }
}
