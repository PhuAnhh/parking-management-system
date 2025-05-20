using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Final_year_Project.Entities;

public partial class ParkingManagementContext : DbContext
{
    public ParkingManagementContext()
    {
    }

    public ParkingManagementContext(DbContextOptions<ParkingManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Camera> Cameras { get; set; }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<CardGroup> CardGroups { get; set; }

    public virtual DbSet<CardGroupLane> CardGroupLanes { get; set; }

    public virtual DbSet<Computer> Computers { get; set; }

    public virtual DbSet<ControlUnit> ControlUnits { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }

    public virtual DbSet<EntryLog> EntryLogs { get; set; }

    public virtual DbSet<ExitLog> ExitLogs { get; set; }

    public virtual DbSet<Gate> Gates { get; set; }

    public virtual DbSet<Lane> Lanes { get; set; }

    public virtual DbSet<LaneCamera> LaneCameras { get; set; }

    public virtual DbSet<LaneControlUnit> LaneControlUnits { get; set; }

    public virtual DbSet<Led> Leds { get; set; }

    public virtual DbSet<RevenueReport> RevenueReports { get; set; }

    public virtual DbSet<WarningEvent> WarningEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=parking_management;Integrated Security=False;User ID=sa;Password=yYwL48ze;TrustServerCertificate=True;");

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

        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__cards__3213E83FD0D0BF2B");

            entity.ToTable("cards");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardGroupId).HasColumnName("card_group_id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CardGroup).WithMany(p => p.Cards)
                .HasForeignKey(d => d.CardGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__cards__card_grou__0FEC5ADD");

            entity.HasOne(d => d.Customer).WithMany(p => p.Cards)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__cards__customer___10E07F16");
        });

        modelBuilder.Entity<CardGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__card_gro__3213E83F4E9DAA2F");

            entity.ToTable("card_groups");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.FirstBlockMinutes).HasColumnName("first_block_minutes");
            entity.Property(e => e.FirstBlockPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("first_block_price");
            entity.Property(e => e.FreeMinutes).HasColumnName("free_minutes");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NextBlockMinutes).HasColumnName("next_block_minutes");
            entity.Property(e => e.NextBlockPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("next_block_price");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleType)
                .HasMaxLength(255)
                .HasColumnName("vehicle_type");
        });

        modelBuilder.Entity<CardGroupLane>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__card_gro__3213E83F9BBFB3F3");

            entity.ToTable("card_group_lanes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardGroupId).HasColumnName("card_group_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LaneId).HasColumnName("lane_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CardGroup).WithMany(p => p.CardGroupLanes)
                .HasForeignKey(d => d.CardGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__card_grou__card___093F5D4E");

            entity.HasOne(d => d.Lane).WithMany(p => p.CardGroupLanes)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__card_grou__lane___0A338187");
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

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__customer__3213E83F2DDF420D");

            entity.ToTable("customers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerGroupId).HasColumnName("customer_group_id");
            entity.Property(e => e.Deleted).HasColumnName("deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CustomerGroup).WithMany(p => p.Customers)
                .HasForeignKey(d => d.CustomerGroupId)
                .HasConstraintName("FK__customers__custo__7EC1CEDB");
        });

        modelBuilder.Entity<CustomerGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__customer__3213E83F17EBC6F4");

            entity.ToTable("customer_groups");

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
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<EntryLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__entry_lo__3213E83F8ED33DC9");

            entity.ToTable("entry_logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardGroupId).HasColumnName("card_group_id");
            entity.Property(e => e.CardId).HasColumnName("card_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EntryTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("entry_time");
            entity.Property(e => e.Exited).HasColumnName("exited");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.LaneId).HasColumnName("lane_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.PlateNumber)
                .HasMaxLength(255)
                .HasColumnName("plate_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CardGroup).WithMany(p => p.EntryLogs)
                .HasForeignKey(d => d.CardGroupId)
                .HasConstraintName("FK__entry_log__card___178D7CA5");

            entity.HasOne(d => d.Card).WithMany(p => p.EntryLogs)
                .HasForeignKey(d => d.CardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__entry_log__card___1699586C");

            entity.HasOne(d => d.Customer).WithMany(p => p.EntryLogs)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__entry_log__custo__1881A0DE");

            entity.HasOne(d => d.Lane).WithMany(p => p.EntryLogs)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__entry_log__lane___1975C517");
        });

        modelBuilder.Entity<ExitLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__exit_log__3213E83FF9DD5CCA");

            entity.ToTable("exit_logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardGroupId).HasColumnName("card_group_id");
            entity.Property(e => e.CardId).HasColumnName("card_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EntryLaneId).HasColumnName("entry_lane_id");
            entity.Property(e => e.EntryLogId).HasColumnName("entry_log_id");
            entity.Property(e => e.EntryTime)
                .HasColumnType("datetime")
                .HasColumnName("entry_time");
            entity.Property(e => e.ExitLaneId).HasColumnName("exit_lane_id");
            entity.Property(e => e.ExitPlateNumber)
                .HasMaxLength(255)
                .HasColumnName("exit_plate_number");
            entity.Property(e => e.ExitTime)
                .HasColumnType("datetime")
                .HasColumnName("exit_time");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.TotalDuration).HasColumnName("total_duration");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_price");

            entity.HasOne(d => d.CardGroup).WithMany(p => p.ExitLogs)
                .HasForeignKey(d => d.CardGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__exit_logs__card___370627FE");

            entity.HasOne(d => d.Card).WithMany(p => p.ExitLogs)
                .HasForeignKey(d => d.CardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__exit_logs__card___361203C5");

            entity.HasOne(d => d.EntryLane).WithMany(p => p.ExitLogEntryLanes)
                .HasForeignKey(d => d.EntryLaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__exit_logs__entry__37FA4C37");

            entity.HasOne(d => d.EntryLog).WithMany(p => p.ExitLogs)
                .HasForeignKey(d => d.EntryLogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__exit_logs__entry__351DDF8C");

            entity.HasOne(d => d.ExitLane).WithMany(p => p.ExitLogExitLanes)
                .HasForeignKey(d => d.ExitLaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__exit_logs__exit___38EE7070");
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
                .HasMaxLength(255)
                .HasColumnName("purpose");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Camera).WithMany(p => p.LaneCameras)
                .HasForeignKey(d => d.CameraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lane_came__camer__318258D2");

            entity.HasOne(d => d.Lane).WithMany(p => p.LaneCameras)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lane_cont__contr__373B3228");

            entity.HasOne(d => d.Lane).WithMany(p => p.LaneControlUnits)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
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

        modelBuilder.Entity<RevenueReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__revenue___3213E83F4C1D649E");

            entity.ToTable("revenue_reports");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardGroupId).HasColumnName("card_group_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExitCount).HasColumnName("exit_count");
            entity.Property(e => e.Revenue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("revenue");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CardGroup).WithMany(p => p.RevenueReports)
                .HasForeignKey(d => d.CardGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__revenue_r__card___436BFEE3");
        });

        modelBuilder.Entity<WarningEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__warning___3213E83F5A3B4BC4");

            entity.ToTable("warning_events");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.LaneId).HasColumnName("lane_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.PlateNumber)
                .HasMaxLength(50)
                .HasColumnName("plate_number");
            entity.Property(e => e.WarningType)
                .HasMaxLength(255)
                .HasColumnName("warning_type");

            entity.HasOne(d => d.Lane).WithMany(p => p.WarningEvents)
                .HasForeignKey(d => d.LaneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__warning_e__lane___3CBF0154");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
