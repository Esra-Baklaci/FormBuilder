using DynamicFormBuilder.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace DynamicFormBuilder.DataAccess.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<DynamicForm> Forms => Set<DynamicForm>();
    public DbSet<FormField> FormFields => Set<FormField>();
    public DbSet<FormResponse> FormResponses => Set<FormResponse>();
    public DbSet<FormResponseValue> FormResponseValues => Set<FormResponseValue>();
    public DbSet<ConditionalLogic> ConditionalLogics => Set<ConditionalLogic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DynamicForm>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.NotificationEmail).HasMaxLength(256);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.PrimaryColor).HasMaxLength(20);
            entity.Property(e => e.BackgroundColor).HasMaxLength(20);
            entity.Property(e => e.ButtonColor).HasMaxLength(20);
        });

        modelBuilder.Entity<FormField>(entity =>
        {
            entity.Property(e => e.ClientId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Label).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Placeholder).HasMaxLength(300);
            entity.Property(e => e.Width).HasMaxLength(20);
            entity.Property(e => e.CssClass).HasMaxLength(100);
            entity.Property(e => e.RegexPattern).HasMaxLength(500);
            entity.Property(e => e.MinValue).HasPrecision(18, 4);
            entity.Property(e => e.MaxValue).HasPrecision(18, 4);
            entity.HasIndex(e => new { e.FormId, e.ClientId }).IsUnique();
            entity.HasOne(e => e.Form)
                .WithMany(f => f.Fields)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FormResponse>(entity =>
        {
            entity.HasOne(e => e.Form)
                .WithMany(f => f.Responses)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FormResponseValue>(entity =>
        {
            entity.Property(e => e.FieldLabel).HasMaxLength(300).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(4000);
            entity.HasOne(e => e.Response)
                .WithMany(r => r.Values)
                .HasForeignKey(e => e.ResponseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Field)
                .WithMany()
                .HasForeignKey(e => e.FieldId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConditionalLogic>(entity =>
        {
            entity.Property(e => e.SourceFieldClientId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TargetFieldClientId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Value).HasMaxLength(500).IsRequired();
            entity.HasOne(e => e.Form)
                .WithMany(f => f.ConditionalLogics)
                .HasForeignKey(e => e.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
