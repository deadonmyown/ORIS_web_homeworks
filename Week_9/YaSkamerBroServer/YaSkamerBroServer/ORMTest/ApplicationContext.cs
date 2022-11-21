using Microsoft.EntityFrameworkCore;
using MyORM.Model;

namespace YaSkamerBroServer.ORM;

public class ApplicationContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ServerDB;Integrated Security=True;",
            options => options.EnableRetryOnFailure());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().Property(a => a.Id).HasColumnName("Id").UseIdentityColumn(1, 1);
        modelBuilder.Entity<Account>().Property(a => a.Name).HasColumnName("Name").IsRequired();
        modelBuilder.Entity<Account>().Property(a => a.Email).HasColumnName("Email").HasDefaultValue("no email");
        modelBuilder.Entity<Account>().Property(a => a.Phone).HasColumnName("Phone").HasDefaultValue("no phone");
        modelBuilder.Entity<Account>().Property(a => a.Password).HasColumnName("Password").IsRequired();

        base.OnModelCreating(modelBuilder);
    }
}