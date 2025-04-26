using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Domain.EnumTypes;

namespace Final_year_Project.Persistence.DbContexts;

public partial class DeviceServiceContext : DbContext
{
    public DeviceServiceContext(DbContextOptions<DeviceServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Camera> Cameras { get; set; }

    public virtual DbSet<Computer> Computers { get; set; }

    public virtual DbSet<ControlUnit> ControlUnits { get; set; }

    public virtual DbSet<Gate> Gates { get; set; }

    public virtual DbSet<Lane> Lanes { get; set; }

    public virtual DbSet<LaneCamera> LaneCameras { get; set; }

    public virtual DbSet<LaneControlUnit> LaneControlUnits { get; set; }

    public virtual DbSet<Led> Leds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Camera>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cameras__3213E83F059B2647");

            entity.ToTable("cameras");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.ComputerId).HasColumnName("computer_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(255)
                .HasColumnName("ip_address");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Resolution)
                .HasMaxLength(50)
                .HasColumnName("resolution");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (CameraType)Enum.Parse(typeof(CameraType), v)
                )
                .HasMaxLength(225)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");

            entity.HasOne(d => d.Computer).WithMany(p => p.Cameras)
                .HasForeignKey(d => d.ComputerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cameras__compute__7D0E9093");
        });

        modelBuilder.Entity<Computer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__computer__3213E83F2FD73D59");

            entity.ToTable("computers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.GateId).HasColumnName("gate_id");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(255)
                .HasColumnName("ip_address");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Gate).WithMany(p => p.Computers)
                .HasForeignKey(d => d.GateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__computers__gate___76619304");
        });

        modelBuilder.Entity<ControlUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__control___3213E83FC2A3C7D5");

            entity.ToTable("control_units");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Baudrate).HasColumnName("baudrate");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.Comport)
                .HasMaxLength(255)
                .HasColumnName("comport");
            entity.Property(e => e.ComputerId).HasColumnName("computer_id");
            entity.Property(e => e.ConnectionProtocol)
                .HasConversion(
                    v => v.ToString(),
                    v => (ControlUnitConnectionProtocolType)Enum.Parse(typeof(ControlUnitConnectionProtocolType), v)
                )
                .HasMaxLength(255)
                .HasColumnName("connection_protocol");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (ControlUnitType)Enum.Parse(typeof (ControlUnitType), v))
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");

            entity.HasOne(d => d.Computer).WithMany(p => p.ControlUnits)
                .HasForeignKey(d => d.ComputerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__control_u__compu__03BB8E22");
        });

        modelBuilder.Entity<Gate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__gates__3213E83F75529161");

            entity.ToTable("gates");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Lane>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__lanes__3213E83F8FD144C2");

            entity.ToTable("lanes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutoOpenBarrier)
                .HasConversion(
                    v => v.ToString(),
                    v => (LaneAutoOpenBarrier)Enum.Parse(typeof(LaneAutoOpenBarrier), v))
                .HasMaxLength(255)
                .HasColumnName("auto_open_barrier");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.ComputerId).HasColumnName("computer_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.DisplayLed).HasColumnName("display_led");
            entity.Property(e => e.Loop).HasColumnName("loop");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.ReverseLane).HasColumnName("reverse_lane");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (LaneType)Enum.Parse(typeof(LaneType), v))
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Computer).WithMany(p => p.Lanes)
                .HasForeignKey(d => d.ComputerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lanes__computer___0A688BB1");
        });

        modelBuilder.Entity<LaneCamera>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__lane_cam__3213E83F64141219");

            entity.ToTable("lane_cameras");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CameraId).HasColumnName("camera_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayPosition).HasColumnName("display_position");
            entity.Property(e => e.LaneId).HasColumnName("lane_id");
            entity.Property(e => e.Purpose)
                .HasConversion(
                    v => v.ToString(),
                    v => (LaneCameraPurpose)Enum.Parse(typeof(LaneCameraPurpose), v))
                .HasMaxLength(255)
                .HasColumnName("purpose");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Camera).WithMany(p => p.LaneCameras)
                .HasForeignKey(d => d.CameraId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__lane_came__camer__318258D2");

            entity.HasOne(d => d.Lane).WithMany(p => p.LaneCameras)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__lane_came__lane___308E3499");
        });

        modelBuilder.Entity<LaneControlUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__lane_con__3213E83FBC77630F");

            entity.ToTable("lane_control_units");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Alarm)
                .HasMaxLength(255)
                .HasColumnName("alarm");
            entity.Property(e => e.Barrier)
                .HasMaxLength(255)
                .HasColumnName("barrier");
            entity.Property(e => e.ControlUnitId).HasColumnName("control_unit_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Input)
                .HasMaxLength(255)
                .HasColumnName("input");
            entity.Property(e => e.LaneId).HasColumnName("lane_id");
            entity.Property(e => e.Reader)
                .HasMaxLength(255)
                .HasColumnName("reader");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ControlUnit).WithMany(p => p.LaneControlUnits)
                .HasForeignKey(d => d.ControlUnitId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__lane_cont__contr__373B3228");

            entity.HasOne(d => d.Lane).WithMany(p => p.LaneControlUnits)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__lane_cont__lane___36470DEF");
        });

        modelBuilder.Entity<Led>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__leds__3213E83FA1CAF947");

            entity.ToTable("leds");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Baudrate).HasColumnName("baudrate");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.Comport)
                .HasMaxLength(255)
                .HasColumnName("comport");
            entity.Property(e => e.ComputerId).HasColumnName("computer_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (LedType)Enum.Parse(typeof(LedType), v))
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Computer).WithMany(p => p.Leds)
                .HasForeignKey(d => d.ComputerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__leds__computer_i__11158940");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
