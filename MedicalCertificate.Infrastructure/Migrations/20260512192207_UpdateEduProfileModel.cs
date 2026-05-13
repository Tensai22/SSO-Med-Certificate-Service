using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalCertificate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEduProfileModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "users");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [certificatestatuses] WHERE [Id] = 1)
BEGIN
    SET IDENTITY_INSERT [certificatestatuses] ON;
    INSERT INTO [certificatestatuses] ([Id], [Status]) VALUES (1, N'В обработке');
    SET IDENTITY_INSERT [certificatestatuses] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [certificatestatuses] WHERE [Id] = 2)
BEGIN
    SET IDENTITY_INSERT [certificatestatuses] ON;
    INSERT INTO [certificatestatuses] ([Id], [Status]) VALUES (2, N'Принято');
    SET IDENTITY_INSERT [certificatestatuses] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [certificatestatuses] WHERE [Id] = 3)
BEGIN
    SET IDENTITY_INSERT [certificatestatuses] ON;
    INSERT INTO [certificatestatuses] ([Id], [Status]) VALUES (3, N'Отклонено');
    SET IDENTITY_INSERT [certificatestatuses] OFF;
END");

            migrationBuilder.CreateIndex(
                name: "IX_Edu_Students_SpecialityID",
                table: "Edu_Students",
                column: "SpecialityID");

            migrationBuilder.CreateIndex(
                name: "IX_Edu_OrgUnits_ParentID",
                table: "Edu_OrgUnits",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Edu_OrgUnits_Edu_OrgUnits_ParentID",
                table: "Edu_OrgUnits",
                column: "ParentID",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Edu_Students_Edu_OrgUnits_SpecialityID",
                table: "Edu_Students",
                column: "SpecialityID",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edu_OrgUnits_Edu_OrgUnits_ParentID",
                table: "Edu_OrgUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_Edu_Students_Edu_OrgUnits_SpecialityID",
                table: "Edu_Students");

            migrationBuilder.DropIndex(
                name: "IX_Edu_Students_SpecialityID",
                table: "Edu_Students");

            migrationBuilder.DropIndex(
                name: "IX_Edu_OrgUnits_ParentID",
                table: "Edu_OrgUnits");

            migrationBuilder.DeleteData(
                table: "certificatestatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "certificatestatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "certificatestatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
