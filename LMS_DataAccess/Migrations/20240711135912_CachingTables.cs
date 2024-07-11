using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CachingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    CREATE TABLE [dbo].[LMSCache] (
                    [Id] NVARCHAR(449) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL PRIMARY KEY,
                    [Value] VARBINARY(MAX) NOT NULL,
                    [ExpiresAtTime] DATETIMEOFFSET NOT NULL,
                    [SlidingExpirationInSeconds] BIGINT NULL,
                    [AbsoluteExpiration] DATETIMEOFFSET NULL
                );
            ");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE [dbo].[LMSCache];
            ");
        }
    }
}
