using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagement.Models;

public partial class EmployeeLeaveManagementContext : DbContext
{
    public EmployeeLeaveManagementContext()
    {
    }

    public EmployeeLeaveManagementContext(
        DbContextOptions<EmployeeLeaveManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId)
                .HasName("PK__Employee__7AD04F117688E2F5");

            entity.Property(e => e.Department)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.EmployeeName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.JoiningDate)
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.LeaveRequestId)
                .HasName("PK__LeaveReq__609421EE5C8C4521");

            entity.Property(e => e.AppliedDate)
                .HasColumnType("timestamp");

            entity.Property(e => e.Reason)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee)
                .WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Employee");

            entity.HasOne(d => d.LeaveType)
                .WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.LeaveTypeId)
                .HasConstraintName("FK_LeaveType");
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId)
                .HasName("PK__LeaveTyp__43BE8F148B72FCE9");

            entity.Property(e => e.LeaveTypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId)
                .HasName("PK_Roles");

            entity.Property(e => e.RoleName)
                .HasMaxLength(50);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId)
                .HasName("PK_Permissions");

            entity.Property(e => e.ModuleName)
                .HasMaxLength(100);

            entity.Property(e => e.PermissionName)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId)
                .HasName("PK_RolePermissions");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RolePermissions_Roles");

            entity.HasOne(d => d.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("FK_RolePermissions_Permissions");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId)
                .HasName("PK_Users");

            entity.Property(e => e.Username)
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .HasMaxLength(100);

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(e => e.Password)
                .IsRequired();

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}