using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace wbible.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    BookStatsId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookCode = table.Column<string>(nullable: true),
                    ChaptCount = table.Column<int>(nullable: false),
                    WordCount = table.Column<int>(nullable: false),
                    XCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.BookStatsId);
                });

            migrationBuilder.CreateTable(
                name: "Readers",
                columns: table => new
                {
                    ReadersId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AgeRange = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readers", x => x.ReadersId);
                });

            migrationBuilder.CreateTable(
                name: "Corpus",
                columns: table => new
                {
                    CorpusId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Audio = table.Column<string>(nullable: true),
                    BookStatsId = table.Column<int>(nullable: true),
                    CText = table.Column<string>(nullable: true),
                    Chapt = table.Column<int>(nullable: false),
                    PText = table.Column<string>(nullable: true),
                    ReadersId = table.Column<int>(nullable: true),
                    UText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corpus", x => x.CorpusId);
                    table.ForeignKey(
                        name: "FK_Corpus_Stats_BookStatsId",
                        column: x => x.BookStatsId,
                        principalTable: "Stats",
                        principalColumn: "BookStatsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Corpus_Readers_ReadersId",
                        column: x => x.ReadersId,
                        principalTable: "Readers",
                        principalColumn: "ReadersId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Corpus_BookStatsId",
                table: "Corpus",
                column: "BookStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_Corpus_ReadersId",
                table: "Corpus",
                column: "ReadersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Corpus");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Readers");
        }
    }
}
