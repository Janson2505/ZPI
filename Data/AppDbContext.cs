using Microsoft.EntityFrameworkCore;
using Contacts.Application.Domain;
namespace Contacts.Data;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Email> Emails => Set<Email>();
    public DbSet<Phone> Phones => Set<Phone>();
    public DbSet<ImportantDate> ImportantDates => Set<ImportantDate>();
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(contactBuilder =>
        {
            contactBuilder
            .HasKey(contact => contact.Id);
            
            contactBuilder.OwnsOne(
                contact => contact.FirstName,
                firstNameBuilder => firstNameBuilder.Property(fn => fn.Value).IsRequired()
            );
            
            contactBuilder.OwnsOne(
                contact => contact.LastName,
                lastNameBuilder => lastNameBuilder.Property(ln => ln.Value).IsRequired()
            );
        });

        modelBuilder.Entity<Category>(categoryBuilder =>
        {
            categoryBuilder
            .HasKey(category => category.Id);
            categoryBuilder
            .Property(category => category.Name)
            .IsRequired();
        });

        modelBuilder.Entity<Email>(emailBuilder =>
        {
            emailBuilder
            .HasKey(email => email.Id);
        });

        modelBuilder.Entity<Email>().OwnsOne(email => email.Address);

        modelBuilder.Entity<Contact>()
            .HasMany(c => c.Emails)
            .WithOne(e => e.Contact)
            .HasForeignKey(e => e.ContactId);

        // NOWE: jawna relacja Email -> Category z blokadą usuwania
        modelBuilder.Entity<Email>()
            .HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contact>().OwnsOne(
            contact => contact.Age,
            ageBuilder => ageBuilder.Property(age => age.Value)
        );

        // konfiguracja Phone
        modelBuilder.Entity<Phone>(phoneBuilder =>
        {
            phoneBuilder.HasKey(phone => phone.Id);
        });

        modelBuilder.Entity<Phone>().OwnsOne(phone => phone.Number);

        modelBuilder.Entity<Contact>()
            .HasMany(c => c.Phones)
            .WithOne(p => p.Contact)
            .HasForeignKey(p => p.ContactId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Phone>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // konfiguracja ImportantDate
        modelBuilder.Entity<ImportantDate>(dateBuilder =>
        {
            dateBuilder.HasKey(d => d.Id);
            dateBuilder.Property(d => d.Date).IsRequired();
            dateBuilder.Property(d => d.Type).IsRequired();
            dateBuilder.Property(d => d.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<Contact>()
            .HasMany(c => c.ImportantDates)
            .WithOne(d => d.Contact)
            .HasForeignKey(d => d.ContactId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}