﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SeymourBot.DataAccess.Storage.Information;

namespace SeymourBot.Migrations.InfoCommand
{
    [DbContext(typeof(InfoCommandContext))]
    [Migration("20190422161108_InitialInfoCommandContext")]
    partial class InitialInfoCommandContext
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("SeymourBot.Storage.Information.Tables.InfoCommandTable", b =>
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
