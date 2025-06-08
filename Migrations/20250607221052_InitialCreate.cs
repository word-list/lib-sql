using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WordList.Data.Sql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Types",
                columns: table => new
                {
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("types", x => x.Text);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Text = table.Column<string>(type: "text", nullable: false),
                    Commonness = table.Column<int>(type: "integer", nullable: false),
                    Offensiveness = table.Column<int>(type: "integer", nullable: false),
                    Sentiment = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("words", x => x.Text);
                });

            migrationBuilder.CreateTable(
                name: "WordWordType",
                columns: table => new
                {
                    WordTypesText = table.Column<string>(type: "text", nullable: false),
                    WordsText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordWordType", x => new { x.WordTypesText, x.WordsText });
                    table.ForeignKey(
                        name: "FK_WordWordType_Types_WordTypesText",
                        column: x => x.WordTypesText,
                        principalTable: "Types",
                        principalColumn: "Text",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordWordType_Words_WordsText",
                        column: x => x.WordsText,
                        principalTable: "Words",
                        principalColumn: "Text",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordWordType_WordsText",
                table: "WordWordType",
                column: "WordsText");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordWordType");

            migrationBuilder.DropTable(
                name: "Types");

            migrationBuilder.DropTable(
                name: "Words");
        }
    }
}
