﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SeymourBot.DataAccess.Storage.Information;

namespace SeymourBot.Migrations
{
    [DbContext(typeof(InfoCommandContext))]
    partial class InfoCommandContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity("SeymourBot.Storage.Information.InfoCommandTable", b =>
                {
                    b.Property<int>("CommandID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommandContent");

                    b.Property<string>("CommandName");

                    b.HasKey("CommandID");

                    b.ToTable("InfoCommandTable");
                });
#pragma warning restore 612, 618
        }
    }
}
