﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PeanutsEveryDay.Infrastructure.Persistence;

#nullable disable

namespace PeanutsEveryDay.Infrastructure.Migrations
{
    [DbContext(typeof(PeanutsContext))]
    partial class PeanutsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PeanutsEveryDay.Domain.Models.Comic", b =>
                {
                    b.Property<DateOnly>("PublicationDate")
                        .HasColumnType("date");

                    b.Property<int>("Source")
                        .HasColumnType("integer");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PublicationDate", "Source");

                    b.HasIndex("PublicationDate", "Source")
                        .IsUnique();

                    b.ToTable("Comics");
                });

            modelBuilder.Entity("PeanutsEveryDay.Domain.Models.Metric", b =>
                {
                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<int>("RegisteredUsers")
                        .HasColumnType("integer");

                    b.Property<long>("SendedComics")
                        .HasColumnType("bigint");

                    b.HasKey("Date");

                    b.HasIndex("Date")
                        .IsUnique();

                    b.ToTable("Metrics");
                });

            modelBuilder.Entity("PeanutsEveryDay.Domain.Models.ParserState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("LastParsedAcomics")
                        .HasColumnType("integer");

                    b.Property<int>("LastParsedAcomicsBegins")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("LastParsedGocomics")
                        .HasColumnType("date");

                    b.Property<DateOnly>("LastParsedGocomicsBegins")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("ParserStates");
                });

            modelBuilder.Entity("PeanutsEveryDay.Domain.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Language")
                        .HasColumnType("integer");

                    b.Property<string>("NativeLanguage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PeanutsEveryDay.Domain.Models.UserProgress", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<DateOnly>("LastWatchedComicDate")
                        .HasColumnType("date");

                    b.Property<int>("TotalComicsWatched")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UsersProgress");
                });

            modelBuilder.Entity("PeanutsEveryDay.Domain.Models.UserSettings", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<int>("Period")
                        .HasColumnType("integer");

                    b.Property<int>("Sources")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UsersSettings");
                });

            modelBuilder.Entity("PeanutsEveryDay.Domain.Models.UserProgress", b =>
                {
                    b.HasOne("PeanutsEveryDay.Domain.Models.User", null)
                        .WithOne("Progress")
                        .HasForeignKey("PeanutsEveryDay.Domain.Models.UserProgress", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PeanutsEveryDay.Domain.Models.UserSettings", b =>
                {
                    b.HasOne("PeanutsEveryDay.Domain.Models.User", null)
                        .WithOne("Settings")
                        .HasForeignKey("PeanutsEveryDay.Domain.Models.UserSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PeanutsEveryDay.Domain.Models.User", b =>
                {
                    b.Navigation("Progress")
                        .IsRequired();

                    b.Navigation("Settings")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
