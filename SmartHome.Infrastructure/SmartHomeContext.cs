
using Microsoft.EntityFrameworkCore;
using SmartHome.Infrastructure.Models;

namespace SmartHome.Infrastructure;

public class SmartHomeContext : DbContext
{
    public SmartHomeContext(
        DbContextOptions<SmartHomeContext> options)
        : base(options)
    {
    }

    public DbSet<RoomModel> Rooms => Set<RoomModel>();
    public DbSet<DeviceModel> Devices => Set<DeviceModel>();
    public DbSet<DeviceSettingsModel> DeviceSettings
        => Set<DeviceSettingsModel>();
    public DbSet<TagModel> Tags => Set<TagModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Один-до-багатьох: Room -> Devices
        modelBuilder.Entity<RoomModel>()
            .HasMany(r => r.Devices)
            .WithOne(d => d.Room)
            .HasForeignKey(d => d.RoomId);

        // Один-до-одного: Device -> Settings
        modelBuilder.Entity<DeviceModel>()
            .HasOne(d => d.Settings)
            .WithOne(s => s.Device)
            .HasForeignKey<DeviceSettingsModel>(
                s => s.DeviceId);

        // Багато-до-багатьох: Devices <-> Tags
        modelBuilder.Entity<DeviceModel>()
            .HasMany(d => d.Tags)
            .WithMany(t => t.Devices)
            .UsingEntity(j => j.ToTable("DeviceTags"));

        // Обов'язкові назви
        modelBuilder.Entity<RoomModel>()
            .Property(r => r.Name)
            .IsRequired();

        modelBuilder.Entity<DeviceModel>()
            .Property(d => d.Name)
            .IsRequired();

        modelBuilder.Entity<TagModel>()
            .Property(t => t.Name)
            .IsRequired();
    }
}
