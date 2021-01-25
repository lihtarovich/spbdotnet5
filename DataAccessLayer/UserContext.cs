using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }
        
        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbPhoneNumber> PhoneNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUser>().ToTable("users");
            modelBuilder.Entity<DbPhoneNumber>().ToTable("phones");

            modelBuilder.Entity<DbUser>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<DbPhoneNumber>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<DbPhoneNumber>()
                .HasOne(x => x.User)
                .WithMany(x => x.PhoneNumbers)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("fk_phones_user")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}