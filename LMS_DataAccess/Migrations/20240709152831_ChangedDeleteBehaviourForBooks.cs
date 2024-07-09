using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDeleteBehaviourForBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookCollections_CollectionId",
                table: "Books");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookCollections_CollectionId",
                table: "Books",
                column: "CollectionId",
                principalTable: "BookCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookCollections_CollectionId",
                table: "Books");

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "CollectionId", "Genre", "Status", "Title", "Year" },
                values: new object[] { 1, "Good Boi", null, "Some genre", 0, "Good book", 1941 });

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookCollections_CollectionId",
                table: "Books",
                column: "CollectionId",
                principalTable: "BookCollections",
                principalColumn: "Id");
        }
    }
}
