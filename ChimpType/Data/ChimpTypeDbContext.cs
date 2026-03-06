using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChimpType.Data;

public partial class ChimpTypeDbContext : DbContext
{
    public ChimpTypeDbContext()
    {
    }

    public ChimpTypeDbContext(DbContextOptions<ChimpTypeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TestsTaken> TestsTakens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<TestsTaken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tests_taken_pkey");

            entity.ToTable("Tests_taken", tb => tb.HasComment("List of all typing tests taken"));

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Accuracy).HasColumnName("accuracy");
            entity.Property(e => e.CorrectCharacters).HasColumnName("correct_characters");
            entity.Property(e => e.ExtraCharacters).HasColumnName("extra_characters");
            entity.Property(e => e.MissedCharacters).HasColumnName("missed_characters");
            entity.Property(e => e.MistakesNumber).HasColumnName("mistakes_number");
            entity.Property(e => e.TakenOn)
                .HasDefaultValueSql("now()")
                .HasColumnName("taken_on");
            entity.Property(e => e.TestType).HasColumnName("test_type");
            entity.Property(e => e.TotalTime).HasColumnName("total_time");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Wpm).HasColumnName("wpm");
            entity.Property(e => e.WrongCharacters).HasColumnName("wrong_characters");

            entity.HasOne(d => d.User).WithMany(p => p.TestsTakens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Tests_taken_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.HasIndex(e => e.Email, "Users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "Users_username_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(true)
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.IsAdmin)
                .HasDefaultValue(false)
                .HasColumnName("is_admin");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.SessionExpires).HasColumnName("session_expires");
            entity.Property(e => e.SessionTokenId).HasColumnName("session_token_id");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
