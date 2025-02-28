using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TesteUnecont.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LogGuid = table.Column<Guid>(nullable: false),
                    Provider = table.Column<string>(nullable: false),
                    HttpMethod = table.Column<string>(nullable: false),
                    StatusCode = table.Column<int>(nullable: false),
                    UriPath = table.Column<string>(nullable: false),
                    TimeTaken = table.Column<int>(nullable: false),
                    ResponseSize = table.Column<int>(nullable: false),
                    CacheStatus = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEntries");
        }
    }
}
