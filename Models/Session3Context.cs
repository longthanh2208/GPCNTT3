using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementAPI.Models;

public partial class Session3Context : DbContext
{
    public Session3Context()
    {
    }

    public Session3Context(DbContextOptions<Session3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Asset> Assets { get; set; }

    public virtual DbSet<AssetOdometer> AssetOdometers { get; set; }

    public virtual DbSet<PmscheduleModel> PmscheduleModels { get; set; }

    public virtual DbSet<PmscheduleType> PmscheduleTypes { get; set; }

    public virtual DbSet<Pmtask> Pmtasks { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Connection string được inject qua Program.cs (AddDbContext)
        // Không cần hardcode ở đây nữa
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Assets__3214EC271DF64FE9");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AssetGroupId).HasColumnName("AssetGroupID");
            entity.Property(e => e.AssetName).HasMaxLength(150);
            entity.Property(e => e.AssetSn)
                .HasMaxLength(50)
                .HasColumnName("AssetSN");
            entity.Property(e => e.DepartmentLocationId).HasColumnName("DepartmentLocationID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
        });

        modelBuilder.Entity<AssetOdometer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AssetOdo__3214EC2724EBD74A");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AssetId).HasColumnName("AssetID");
            entity.Property(e => e.OdometerAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Asset).WithMany(p => p.AssetOdometers)
                .HasForeignKey(d => d.AssetId)
                .HasConstraintName("FK__AssetOdom__Asset__656C112C");
        });

        modelBuilder.Entity<PmscheduleModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PMSchedu__3214EC27F31FB0D8");

            entity.ToTable("PMScheduleModels");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PmscheduleTypeId).HasColumnName("PMScheduleTypeID");

            entity.HasOne(d => d.PmscheduleType).WithMany(p => p.PmscheduleModels)
                .HasForeignKey(d => d.PmscheduleTypeId)
                .HasConstraintName("FK__PMSchedul__PMSch__60A75C0F");
        });

        modelBuilder.Entity<PmscheduleType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PMSchedu__3214EC2744426841");

            entity.ToTable("PMScheduleTypes");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Pmtask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PMTasks__3214EC2757B8851D");

            entity.ToTable("PMTasks");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AssetId).HasColumnName("AssetID");
            entity.Property(e => e.PmscheduleTypeId).HasColumnName("PMScheduleTypeID");
            entity.Property(e => e.ScheduleKilometer).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaskDone).HasDefaultValue(false);
            entity.Property(e => e.TaskId).HasColumnName("TaskID");

            entity.HasOne(d => d.Asset).WithMany(p => p.Pmtasks)
                .HasForeignKey(d => d.AssetId)
                .HasConstraintName("FK__PMTasks__AssetID__68487DD7");

            entity.HasOne(d => d.PmscheduleType).WithMany(p => p.Pmtasks)
                .HasForeignKey(d => d.PmscheduleTypeId)
                .HasConstraintName("FK__PMTasks__PMSched__6A30C649");

            entity.HasOne(d => d.Task).WithMany(p => p.Pmtasks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__PMTasks__TaskID__693CA210");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tasks__3214EC278D8C0078");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__Users__536C85E5DF9460D1");

            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
