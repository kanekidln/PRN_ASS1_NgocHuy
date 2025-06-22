using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BusinessObjects
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext()
        {
        }

        public HotelDbContext(DbContextOptions<HotelDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RoomType> RoomTypes { get; set; }
        public virtual DbSet<RoomInformation> RoomInformations { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<BookingReservation> BookingReservations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("HotelDB"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure RoomType entity
            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.HasKey(e => e.RoomTypeID);
                entity.Property(e => e.RoomTypeID).UseIdentityColumn();
                entity.Property(e => e.RoomTypeName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TypeDescription).HasMaxLength(200);
                entity.Property(e => e.TypeNote).HasMaxLength(200);
            });

            // Configure RoomInformation entity
            modelBuilder.Entity<RoomInformation>(entity =>
            {
                entity.HasKey(e => e.RoomID);
                entity.Property(e => e.RoomID).UseIdentityColumn();
                entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.RoomDescription).HasMaxLength(200);
                entity.Property(e => e.RoomPricePerDate).HasColumnType("decimal(18,2)");
                
                // Define relationship with RoomType
                entity.HasOne(d => d.RoomType)
                      .WithMany()
                      .HasForeignKey(d => d.RoomTypeID)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerID);
                entity.Property(e => e.CustomerID).UseIdentityColumn();
                entity.Property(e => e.CustomerFullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Telephone).HasMaxLength(20);
                entity.Property(e => e.EmailAddress).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(50);
            });

            // Configure BookingReservation entity
            modelBuilder.Entity<BookingReservation>(entity =>
            {
                entity.HasKey(e => e.BookingReservationID);
                entity.Property(e => e.BookingReservationID).UseIdentityColumn();
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                
                // Define relationship with Customer
                entity.HasOne(d => d.Customer)
                      .WithMany()
                      .HasForeignKey(d => d.CustomerID)
                      .OnDelete(DeleteBehavior.ClientSetNull);
                
                // Define relationship with RoomInformation
                entity.HasOne(d => d.Room)
                      .WithMany()
                      .HasForeignKey(d => d.RoomID)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
} 