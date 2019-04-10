﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SeymourBot.DataAccess.Storage.Filter;

namespace SeymourBot.Migrations.Filter
{
    [DbContext(typeof(FilterContext))]
    [Migration("20190405162622_FilterContextInitialCreate")]
    partial class FilterContextInitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity("SeymourBot.DataAccess.Storage.Filter.Tables.FilterTable", b =>
                {
                    b.Property<int>("FilterId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FilterName");

                    b.Property<string>("FilterPattern");

                    b.Property<int>("FilterType");

                    b.HasKey("FilterId");

                    b.ToTable("filterTables");
                });
#pragma warning restore 612, 618
        }
    }
}