using GestionePrenotazioniStruttura.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionePrenotazioniStruttura.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        // DbSet principali
        public DbSet<User> Users => Set<User>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Structure> Structures => Set<Structure>();
        public DbSet<Activity> Activities => Set<Activity>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Trainer> Trainers => Set<Trainer>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Activity>()
                .HasMany(a => a.Bookings)
                .WithOne(b => b.Activity)
                .HasForeignKey(b => b.ActivityId)
                .OnDelete(DeleteBehavior.NoAction); 

            
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Structure>()
                .HasMany(s => s.Rooms)
                .WithOne(r => r.Structure)
                .HasForeignKey(r => r.StructureId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Structure>()
                .HasMany(s => s.Activities)
                .WithOne(a => a.Structure)
                .HasForeignKey(a => a.StructureId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

           
            modelBuilder.Entity<User>()
                .HasMany(u => u.Subscriptions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Subscription>()
                .HasMany(s => s.Activities)
                .WithMany(a => a.Subscriptions);

           
            modelBuilder.Entity<Trainer>()
                .HasMany(t => t.Activities)
                .WithMany(a => a.Trainers);

            
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Subscription)
                .WithOne(s => s.Payment)
                .HasForeignKey<Payment>(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
