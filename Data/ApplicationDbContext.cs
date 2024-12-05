using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectParticipation> ProjectParticipations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Employee
      modelBuilder.Entity<Employee>(entity =>
      {
        entity.ToTable("employees");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.FirstName).IsRequired();
        entity.Property(e => e.LastName).IsRequired();
        entity.Property(e => e.Email).IsRequired();

        entity.HasOne(e => e.Department)
          .WithMany(d => d.Employees)
          .HasForeignKey(e => e.DepartmentId)
          .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(e => e.Position)
          .WithMany(p => p.Employees)
          .HasForeignKey(e => e.PositionId)
          .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.ProjectParticipations)
          .WithOne(pp => pp.Employee)
          .HasForeignKey(pp => pp.EmployeeId);
      });
      // Department
      modelBuilder.Entity<Department>(entity =>
            {
              entity.ToTable("departments");
              entity.HasKey(d => d.Id);
              entity.Property(d => d.Name).IsRequired();

              entity.HasOne(d => d.Manager)
                  .WithMany()
                  .HasForeignKey(d => d.ManagerId)
                  .OnDelete(DeleteBehavior.SetNull);
            });

      // Position
      modelBuilder.Entity<Position>(entity =>
      {
        entity.ToTable("positions");
        entity.HasKey(p => p.Id);
        entity.Property(p => p.Title).IsRequired();
        entity.Property(p => p.SalaryRange).IsRequired();
      });

      // Project
      modelBuilder.Entity<Project>(entity =>
      {
        entity.ToTable("projects");
        entity.HasKey(p => p.Id);
        entity.Property(p => p.Name).IsRequired();
      });

      // ProjectParticipation
      modelBuilder.Entity<ProjectParticipation>(entity =>
      {
        entity.ToTable("project_participations");
        entity.HasKey(pp => pp.Id);

        entity.HasOne(pp => pp.Employee)
                  .WithMany()
                  .HasForeignKey(pp => pp.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(pp => pp.Project)
                  .WithMany(p => p.Participants)
                  .HasForeignKey(pp => pp.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
      });
    }
  }
}