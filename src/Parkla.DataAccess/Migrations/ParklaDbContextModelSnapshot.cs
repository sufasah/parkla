﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Parkla.DataAccess.Contexts;

#nullable disable

namespace Parkla.DataAccess.Migrations
{
    [DbContext(typeof(ParklaDbContext))]
    partial class ParklaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Parkla.Core.Entities.City", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("cities", "public");
                });

            modelBuilder.Entity("Parkla.Core.Entities.District", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<int?>("CityId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("city_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("districts", "public");
                });

            modelBuilder.Entity("Parkla.Core.Entities.Park", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<float?>("AvaragePrice")
                        .HasPrecision(30, 2)
                        .HasColumnType("real")
                        .HasColumnName("avarage_price");

                    b.Property<int?>("EmptySpace")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("empty_space");

                    b.Property<string[]>("Extras")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("text[]")
                        .HasColumnName("extras");

                    b.Property<double?>("Latitude")
                        .IsRequired()
                        .HasPrecision(2, 15)
                        .HasColumnType("double precision")
                        .HasColumnName("latitude");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("location");

                    b.Property<double?>("Longitude")
                        .IsRequired()
                        .HasPrecision(2, 15)
                        .HasColumnType("double precision")
                        .HasColumnName("longitude");

                    b.Property<float?>("MaxPrice")
                        .HasPrecision(30, 2)
                        .HasColumnType("real")
                        .HasColumnName("max_price");

                    b.Property<float?>("MinPrice")
                        .HasPrecision(30, 2)
                        .HasColumnType("real")
                        .HasColumnName("min_price");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<int?>("OccupiedSpace")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("occupied_space");

                    b.Property<int?>("ReservedSpace")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("reserved_space");

                    b.Property<DateTime?>("StatusUpdateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        .HasColumnName("status_update_time");

                    b.Property<int?>("UserId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("parks", "public");

                    b.HasCheckConstraint("CK_LATITUDE_AND_LONGITUDE_ARE_VALID", "latitude >= -90 and latitude <= 90 and longitude >= -180 and longitude <= 180");

                    b.HasCheckConstraint("CK_PRICES_VALID", "min_price <= avarage_price and avarage_price <= max_price");

                    b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC", "status_update_time < (now() at time zone 'utc')");
                });

            modelBuilder.Entity("Parkla.Core.Entities.ParkArea", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<float?>("AvaragePrice")
                        .HasPrecision(30, 2)
                        .HasColumnType("real")
                        .HasColumnName("avarage_price");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("description");

                    b.Property<int?>("EmptySpace")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("empty_space");

                    b.Property<float?>("MaxPrice")
                        .HasPrecision(30, 2)
                        .HasColumnType("real")
                        .HasColumnName("max_price");

                    b.Property<float?>("MinPrice")
                        .HasPrecision(30, 2)
                        .HasColumnType("real")
                        .HasColumnName("min_price");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<int?>("OccupiedSpace")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("occupied_space");

                    b.Property<int?>("ParkId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("park_id");

                    b.Property<bool?>("ReservationsEnabled")
                        .IsRequired()
                        .HasColumnType("boolean")
                        .HasColumnName("reservations_enabled");

                    b.Property<int?>("ReservedSpace")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("reserved_space");

                    b.Property<DateTime?>("StatusUpdateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        .HasColumnName("status_update_time");

                    b.Property<string>("TemplateImage")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("template_image");

                    b.HasKey("Id");

                    b.HasIndex("ParkId");

                    b.ToTable("park_areas", "public");

                    b.HasCheckConstraint("CK_PRICES_VALID", "min_price <= avarage_price and avarage_price <= max_price");

                    b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC", "status_update_time < (now() at time zone 'utc')");
                });

            modelBuilder.Entity("Parkla.Core.Entities.ParkSpace", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<int?>("AreaId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("area_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("name");

                    b.Property<int?>("RealSpaceId")
                        .HasColumnType("integer")
                        .HasColumnName("real_space_id");

                    b.Property<string>("SpacePath")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("space_path");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");

                    b.Property<DateTime?>("StatusUpdateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        .HasColumnName("status_update_time");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("park_spaces", "public");

                    b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC", "status_update_time < (now() at time zone 'utc')");
                });

            modelBuilder.Entity("Parkla.Core.Entities.Pricing", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<int?>("Amount")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("amount");

                    b.Property<int?>("AreaId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("area_id");

                    b.Property<float?>("Price")
                        .IsRequired()
                        .HasPrecision(30, 2)
                        .HasColumnType("real")
                        .HasColumnName("price");

                    b.Property<int>("Unit")
                        .HasColumnType("integer")
                        .HasColumnName("unit");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("pricings", "public");
                });

            modelBuilder.Entity("Parkla.Core.Entities.RealParkSpace", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("name");

                    b.Property<int?>("ParkId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("park_id");

                    b.Property<int?>("SpaceId")
                        .HasColumnType("integer")
                        .HasColumnName("space_id");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");

                    b.Property<DateTime?>("StatusUpdateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        .HasColumnName("status_update_time");

                    b.HasKey("Id");

                    b.HasIndex("ParkId");

                    b.HasIndex("SpaceId")
                        .IsUnique();

                    b.ToTable("real_park_spaces", "public");

                    b.HasCheckConstraint("CK_UPDATE_TIME_LESS_THAN_NOW_UTC", "status_update_time < (now() at time zone 'utc')");
                });

            modelBuilder.Entity("Parkla.Core.Entities.ReceivedSpaceStatus", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("DateTime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                        .HasColumnName("datetime");

                    b.Property<int?>("RealSpaceId")
                        .HasColumnType("integer")
                        .HasColumnName("real_space_id");

                    b.Property<int?>("SpaceId")
                        .HasColumnType("integer")
                        .HasColumnName("space_id");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1)
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.HasIndex("RealSpaceId");

                    b.HasIndex("SpaceId");

                    b.ToTable("received_space_statusses", "public");

                    b.HasCheckConstraint("CK_DATETIME_LESS_THAN_NOW_UTC", "datetime < (now() at time zone 'utc')");
                });

            modelBuilder.Entity("Parkla.Core.Entities.Reservation", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("EndTime")
                        .IsRequired()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_time");

                    b.Property<int?>("PricingId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("pricing_id");

                    b.Property<int?>("SpaceId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("space_id");

                    b.Property<DateTime?>("StartTime")
                        .IsRequired()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_time");

                    b.Property<int?>("UserId")
                        .IsRequired()
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("PricingId");

                    b.HasIndex("SpaceId");

                    b.HasIndex("UserId");

                    b.ToTable("reservaions", "public");

                    b.HasCheckConstraint("CK_STARTTIME_LESS_THAN_ENDTIME", "start_time < end_time");
                });

            modelBuilder.Entity("Parkla.Core.Entities.User", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int?>("Id"));

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("address");

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birthdate");

                    b.Property<int?>("CityId")
                        .HasColumnType("integer")
                        .HasColumnName("city_id");

                    b.Property<int?>("DistrictId")
                        .HasColumnType("integer")
                        .HasColumnName("district_id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("character varying(320)")
                        .HasColumnName("email");

                    b.Property<int?>("Gender")
                        .HasColumnType("integer")
                        .HasColumnName("gender");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("character varying(80)")
                        .HasColumnName("password");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("phone");

                    b.Property<string>("RefreshTokenSignature")
                        .HasMaxLength(400)
                        .HasColumnType("character varying(400)")
                        .HasColumnName("refresh_token_signature");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("surname");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("username");

                    b.Property<string>("VerificationCode")
                        .HasColumnType("text")
                        .HasColumnName("verify_code");

                    b.Property<float?>("Wallet")
                        .IsRequired()
                        .HasPrecision(30, 2)
                        .HasColumnType("real")
                        .HasColumnName("wallet");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("DistrictId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("users", "public");
                });

            modelBuilder.Entity("Parkla.Core.Entities.District", b =>
                {
                    b.HasOne("Parkla.Core.Entities.City", "City")
                        .WithMany("Districts")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("Parkla.Core.Entities.Park", b =>
                {
                    b.HasOne("Parkla.Core.Entities.User", "User")
                        .WithMany("Parks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Parkla.Core.Entities.ParkArea", b =>
                {
                    b.HasOne("Parkla.Core.Entities.Park", "Park")
                        .WithMany("Areas")
                        .HasForeignKey("ParkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Park");
                });

            modelBuilder.Entity("Parkla.Core.Entities.ParkSpace", b =>
                {
                    b.HasOne("Parkla.Core.Entities.ParkArea", "Area")
                        .WithMany("Spaces")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("Parkla.Core.Entities.Pricing", b =>
                {
                    b.HasOne("Parkla.Core.Entities.ParkArea", "Area")
                        .WithMany("Pricings")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("Parkla.Core.Entities.RealParkSpace", b =>
                {
                    b.HasOne("Parkla.Core.Entities.Park", "Park")
                        .WithMany("RealSpaces")
                        .HasForeignKey("ParkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parkla.Core.Entities.ParkSpace", "Space")
                        .WithOne("RealSpace")
                        .HasForeignKey("Parkla.Core.Entities.RealParkSpace", "SpaceId");

                    b.Navigation("Park");

                    b.Navigation("Space");
                });

            modelBuilder.Entity("Parkla.Core.Entities.ReceivedSpaceStatus", b =>
                {
                    b.HasOne("Parkla.Core.Entities.RealParkSpace", "RealSpace")
                        .WithMany("ReceivedSpaceStatuses")
                        .HasForeignKey("RealSpaceId");

                    b.HasOne("Parkla.Core.Entities.ParkSpace", "Space")
                        .WithMany("ReceivedSpaceStatuses")
                        .HasForeignKey("SpaceId");

                    b.Navigation("RealSpace");

                    b.Navigation("Space");
                });

            modelBuilder.Entity("Parkla.Core.Entities.Reservation", b =>
                {
                    b.HasOne("Parkla.Core.Entities.Pricing", "Pricing")
                        .WithMany("Reservations")
                        .HasForeignKey("PricingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parkla.Core.Entities.ParkSpace", "Space")
                        .WithMany("Reservations")
                        .HasForeignKey("SpaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Parkla.Core.Entities.User", "User")
                        .WithMany("Reservations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pricing");

                    b.Navigation("Space");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Parkla.Core.Entities.User", b =>
                {
                    b.HasOne("Parkla.Core.Entities.City", "City")
                        .WithMany("Users")
                        .HasForeignKey("CityId");

                    b.HasOne("Parkla.Core.Entities.District", "District")
                        .WithMany("Users")
                        .HasForeignKey("DistrictId");

                    b.Navigation("City");

                    b.Navigation("District");
                });

            modelBuilder.Entity("Parkla.Core.Entities.City", b =>
                {
                    b.Navigation("Districts");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Parkla.Core.Entities.District", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Parkla.Core.Entities.Park", b =>
                {
                    b.Navigation("Areas");

                    b.Navigation("RealSpaces");
                });

            modelBuilder.Entity("Parkla.Core.Entities.ParkArea", b =>
                {
                    b.Navigation("Pricings");

                    b.Navigation("Spaces");
                });

            modelBuilder.Entity("Parkla.Core.Entities.ParkSpace", b =>
                {
                    b.Navigation("RealSpace");

                    b.Navigation("ReceivedSpaceStatuses");

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("Parkla.Core.Entities.Pricing", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("Parkla.Core.Entities.RealParkSpace", b =>
                {
                    b.Navigation("ReceivedSpaceStatuses");
                });

            modelBuilder.Entity("Parkla.Core.Entities.User", b =>
                {
                    b.Navigation("Parks");

                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}
