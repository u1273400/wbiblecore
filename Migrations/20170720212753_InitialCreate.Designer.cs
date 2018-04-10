using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using wbible.DataContexts;

namespace wbible.Migrations
{
    [DbContext(typeof(WBibleContext))]
    [Migration("20170720212753_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("wbible.Models.BookStats", b =>
                {
                    b.Property<int>("BookStatsId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BookCode");

                    b.Property<int>("ChaptCount");

                    b.Property<int>("WordCount");

                    b.Property<int>("XCount");

                    b.HasKey("BookStatsId");

                    b.ToTable("Stats");
                });

            modelBuilder.Entity("wbible.Models.Corpus", b =>
                {
                    b.Property<int>("CorpusId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Audio");

                    b.Property<int?>("BookStatsId");

                    b.Property<string>("CText");

                    b.Property<int>("Chapt");

                    b.Property<string>("PText");

                    b.Property<int?>("ReadersId");

                    b.Property<string>("UText");

                    b.HasKey("CorpusId");

                    b.HasIndex("BookStatsId");

                    b.HasIndex("ReadersId");

                    b.ToTable("Corpus");
                });

            modelBuilder.Entity("wbible.Models.Readers", b =>
                {
                    b.Property<int>("ReadersId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AgeRange");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("Surname");

                    b.Property<string>("Telephone");

                    b.HasKey("ReadersId");

                    b.ToTable("Readers");
                });

            modelBuilder.Entity("wbible.Models.Corpus", b =>
                {
                    b.HasOne("wbible.Models.BookStats", "Book")
                        .WithMany()
                        .HasForeignKey("BookStatsId");

                    b.HasOne("wbible.Models.Readers", "Reader")
                        .WithMany()
                        .HasForeignKey("ReadersId");
                });
        }
    }
}
