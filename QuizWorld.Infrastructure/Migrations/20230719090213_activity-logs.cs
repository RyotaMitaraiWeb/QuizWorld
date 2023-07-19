using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizWorld.Infrastructure.Migrations
{
    public partial class activitylogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9ba9bb21-c16e-4351-88ab-0ce14f94d52a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9e6b5887-25ce-464f-b5d6-9f2a8644d980"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ed99a930-9c6d-4f2a-abde-eb30f15ec959"));

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("6448a395-d249-4aef-bf74-9cdeeba69f33"), "07dc7417-c26a-49e0-ba40-ff6021a1ded9", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("90f00fd4-9966-4819-aa58-98836f0e0ddf"), "cba33ea6-94ae-402e-87a3-1a5a6cd9147d", "Moderator", "MODERATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("cace07a2-930c-4029-9465-019d78edcf68"), "336a122d-4494-4509-adaa-95de720b2657", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6448a395-d249-4aef-bf74-9cdeeba69f33"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("90f00fd4-9966-4819-aa58-98836f0e0ddf"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("cace07a2-930c-4029-9465-019d78edcf68"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("9ba9bb21-c16e-4351-88ab-0ce14f94d52a"), "cc7e1d43-a7df-4129-9b32-b472e8e77809", "Moderator", "MODERATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("9e6b5887-25ce-464f-b5d6-9f2a8644d980"), "ed03eb66-3854-431e-8e38-c92ff7852975", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("ed99a930-9c6d-4f2a-abde-eb30f15ec959"), "32b3baef-502c-4872-bb2d-7d479a11c65a", "Administrator", "ADMINISTRATOR" });
        }
    }
}
