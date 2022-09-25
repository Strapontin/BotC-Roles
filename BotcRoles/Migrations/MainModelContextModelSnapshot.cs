﻿// <auto-generated />
using System;
using BotcRoles.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BotcRoles.Migrations
{
    [DbContext(typeof(ModelContext))]
    partial class MainModelContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("BotcRoles.Models.Game", b =>
                {
                    b.Property<long>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<long>("ModuleId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<long>("StoryTellerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GameId");

                    b.HasIndex("ModuleId");

                    b.HasIndex("StoryTellerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("BotcRoles.Models.Module", b =>
                {
                    b.Property<long>("ModuleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ModuleId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("BotcRoles.Models.Player", b =>
                {
                    b.Property<long>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PlayerId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("BotcRoles.Models.PlayerRoleGame", b =>
                {
                    b.Property<long>("PlayerRoleGameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("FinalAlignment")
                        .HasColumnType("INTEGER");

                    b.Property<long>("GameId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("RoleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PlayerRoleGameId");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("RoleId");

                    b.ToTable("PlayerRoleGames");
                });

            modelBuilder.Entity("BotcRoles.Models.Role", b =>
                {
                    b.Property<long>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DefaultAlignment")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("RoleId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("BotcRoles.Models.RoleModule", b =>
                {
                    b.Property<long>("RoleId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ModuleId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("RoleModuleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("RoleId", "ModuleId");

                    b.HasIndex("ModuleId");

                    b.HasIndex("RoleId", "ModuleId")
                        .IsUnique();

                    b.ToTable("RoleModules");
                });

            modelBuilder.Entity("BotcRoles.Models.Game", b =>
                {
                    b.HasOne("BotcRoles.Models.Module", "Module")
                        .WithMany("Games")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BotcRoles.Models.Player", "StoryTeller")
                        .WithMany("GamesStoryTelling")
                        .HasForeignKey("StoryTellerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Module");

                    b.Navigation("StoryTeller");
                });

            modelBuilder.Entity("BotcRoles.Models.PlayerRoleGame", b =>
                {
                    b.HasOne("BotcRoles.Models.Game", "Game")
                        .WithMany("PlayerRoleGames")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BotcRoles.Models.Player", "Player")
                        .WithMany("PlayerRoleGames")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BotcRoles.Models.Role", "Role")
                        .WithMany("PlayerRoleGames")
                        .HasForeignKey("RoleId");

                    b.Navigation("Game");

                    b.Navigation("Player");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BotcRoles.Models.RoleModule", b =>
                {
                    b.HasOne("BotcRoles.Models.Module", "Module")
                        .WithMany("RoleModules")
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BotcRoles.Models.Role", "Role")
                        .WithMany("RoleModules")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Module");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BotcRoles.Models.Game", b =>
                {
                    b.Navigation("PlayerRoleGames");
                });

            modelBuilder.Entity("BotcRoles.Models.Module", b =>
                {
                    b.Navigation("Games");

                    b.Navigation("RoleModules");
                });

            modelBuilder.Entity("BotcRoles.Models.Player", b =>
                {
                    b.Navigation("GamesStoryTelling");

                    b.Navigation("PlayerRoleGames");
                });

            modelBuilder.Entity("BotcRoles.Models.Role", b =>
                {
                    b.Navigation("PlayerRoleGames");

                    b.Navigation("RoleModules");
                });
#pragma warning restore 612, 618
        }
    }
}
