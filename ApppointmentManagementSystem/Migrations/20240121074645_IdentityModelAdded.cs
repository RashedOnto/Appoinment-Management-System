using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentManagementSystem.Migrations
{
    public partial class IdentityModelAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Doctor_CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_CreatedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_DeletedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_DeletedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_Mobile",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_UpdatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_UpdatedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ModifyAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserInfo_CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserInfo_Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserInfo_Mobile",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserInfo_Name",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Doctor_CreatedAt",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Doctor_CreatedBy",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "Doctor_DeletedAt",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Doctor_DeletedBy",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Doctor_IsActive",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Doctor_Mobile",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Doctor_Name",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "Doctor_UpdatedAt",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Doctor_UpdatedBy",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyAt",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UserInfo_CreatedAt",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserInfo_Gender",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserInfo_Mobile",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserInfo_Name",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
