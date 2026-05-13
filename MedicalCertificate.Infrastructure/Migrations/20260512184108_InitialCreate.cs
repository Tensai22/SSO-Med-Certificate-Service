using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalCertificate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "certificatestatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certificatestatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Edu_EducationTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoBDID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edu_EducationTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Edu_OrgUnitTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edu_OrgUnitTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Edu_Positions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lectures = table.Column<int>(type: "int", nullable: false),
                    Practices = table.Column<int>(type: "int", nullable: false),
                    Labs = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edu_Positions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Edu_Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOB = table.Column<DateOnly>(type: "date", nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Male = table.Column<bool>(type: "bit", nullable: true),
                    HomePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoFileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileContainerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MobilePushID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    oldId = table.Column<int>(type: "int", nullable: true),
                    ESUVOID = table.Column<int>(type: "int", nullable: true),
                    ExtraFileContainerID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Resident = table.Column<bool>(type: "bit", nullable: false),
                    Hero_Person_ID = table.Column<int>(type: "int", nullable: true),
                    IsReadTeamsNotif = table.Column<bool>(type: "bit", nullable: true),
                    NationalityID = table.Column<int>(type: "int", nullable: true),
                    MaritalStatusID = table.Column<int>(type: "int", nullable: true),
                    MessengerTypeID = table.Column<int>(type: "int", nullable: true),
                    CitizenshipCountryID = table.Column<int>(type: "int", nullable: true),
                    CitizenCategoryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edu_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Operation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    System = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "storedfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bucket = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjectKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storedfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Edu_OrgUnits",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentID = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    ShortTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edu_OrgUnits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Edu_OrgUnits_Edu_OrgUnitTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "Edu_OrgUnitTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Edu_Employees",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    IsAdvisor = table.Column<bool>(type: "bit", nullable: false),
                    RoleGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edu_Employees", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Edu_Employees_Edu_Users_ID",
                        column: x => x.ID,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Edu_Students",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    SpecialityID = table.Column<int>(type: "int", nullable: true),
                    StatusID = table.Column<int>(type: "int", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    NeedsDorm = table.Column<bool>(type: "bit", nullable: false),
                    AltynBelgi = table.Column<bool>(type: "bit", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    RupID = table.Column<int>(type: "int", nullable: true),
                    EntryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    GPA = table.Column<double>(type: "float", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GraduatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcademicStatusEndsOn = table.Column<DateOnly>(type: "date", nullable: true),
                    AcademicStatusStartsOn = table.Column<DateOnly>(type: "date", nullable: true),
                    GPA_Y = table.Column<double>(type: "float", nullable: true),
                    IsPersonalDataComplete = table.Column<bool>(type: "bit", nullable: true),
                    HosterPrivelegeID = table.Column<int>(type: "int", nullable: true),
                    MinorSpecialityID = table.Column<int>(type: "int", nullable: true),
                    EnrollmentTypeId = table.Column<int>(type: "int", nullable: true),
                    EctsGPA = table.Column<double>(type: "float", nullable: true),
                    EctsGPA_Y = table.Column<double>(type: "float", nullable: true),
                    IsScholarship = table.Column<bool>(type: "bit", nullable: true),
                    ScholarshipTypeID = table.Column<int>(type: "int", nullable: true),
                    ScholarshipOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScholarshipOrderDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ScholarshipDateStart = table.Column<DateOnly>(type: "date", nullable: true),
                    ScholarshipDateEnd = table.Column<DateOnly>(type: "date", nullable: true),
                    FundingID = table.Column<int>(type: "int", nullable: true),
                    IsKNB = table.Column<bool>(type: "bit", nullable: true),
                    EducationTypeID = table.Column<int>(type: "int", nullable: true),
                    EducationPaymentTypeID = table.Column<int>(type: "int", nullable: true),
                    GrantTypeID = table.Column<int>(type: "int", nullable: true),
                    EducationDurationID = table.Column<int>(type: "int", nullable: true),
                    StudyLanguageID = table.Column<int>(type: "int", nullable: true),
                    AcademicStatusID = table.Column<int>(type: "int", nullable: true),
                    AdvisorID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edu_Students", x => x.StudentID);
                    table.ForeignKey(
                        name: "FK_Edu_Students_Edu_Users_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleOperation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleOperation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleOperation_Operation_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleOperation_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IIN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EduUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_Edu_Users_EduUserId",
                        column: x => x.EduUserId,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_users_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "certificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Clinic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePathId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ReviewerComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificateStatusId = table.Column<int>(type: "int", nullable: true),
                    StoredFileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_certificates_Edu_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_certificates_certificatestatuses_CertificateStatusId",
                        column: x => x.CertificateStatusId,
                        principalTable: "certificatestatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_certificates_certificatestatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "certificatestatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_certificates_storedfiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalTable: "storedfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Edu_EmployeePositions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    EndedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: true),
                    IsMainPosition = table.Column<bool>(type: "bit", nullable: true),
                    HrOrderId = table.Column<int>(type: "int", nullable: true),
                    OrgUnitID = table.Column<int>(type: "int", nullable: false),
                    PositionID = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Edu_EmployeePositions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Edu_EmployeePositions_Edu_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Edu_Employees",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Edu_EmployeePositions_Edu_OrgUnits_OrgUnitID",
                        column: x => x.OrgUnitID,
                        principalTable: "Edu_OrgUnits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Edu_EmployeePositions_Edu_Positions_PositionID",
                        column: x => x.PositionID,
                        principalTable: "Edu_Positions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "certificatestatushistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CertificateId = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ChangedBy = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_certificatestatushistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_certificatestatushistories_certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "certificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_certificatestatushistories_certificatestatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "certificatestatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_certificatestatushistories_users_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Office Registrar" },
                    { 2, "Student" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_certificates_CertificateStatusId",
                table: "certificates",
                column: "CertificateStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_certificates_StatusId",
                table: "certificates",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_certificates_StoredFileId",
                table: "certificates",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_certificates_UserId",
                table: "certificates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_certificatestatushistories_CertificateId",
                table: "certificatestatushistories",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_certificatestatushistories_ChangedBy",
                table: "certificatestatushistories",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_certificatestatushistories_StatusId",
                table: "certificatestatushistories",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Edu_EmployeePositions_EmployeeID",
                table: "Edu_EmployeePositions",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Edu_EmployeePositions_OrgUnitID",
                table: "Edu_EmployeePositions",
                column: "OrgUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_Edu_EmployeePositions_PositionID",
                table: "Edu_EmployeePositions",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_Edu_OrgUnits_TypeID",
                table: "Edu_OrgUnits",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperation_OperationId",
                table: "RoleOperation",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperation_RoleId",
                table: "RoleOperation",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_EduUserId",
                table: "users",
                column: "EduUserId",
                unique: true,
                filter: "[EduUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId",
                table: "users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificatestatushistories");

            migrationBuilder.DropTable(
                name: "Edu_EducationTypes");

            migrationBuilder.DropTable(
                name: "Edu_EmployeePositions");

            migrationBuilder.DropTable(
                name: "Edu_Students");

            migrationBuilder.DropTable(
                name: "RoleOperation");

            migrationBuilder.DropTable(
                name: "certificates");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "Edu_Employees");

            migrationBuilder.DropTable(
                name: "Edu_OrgUnits");

            migrationBuilder.DropTable(
                name: "Edu_Positions");

            migrationBuilder.DropTable(
                name: "Operation");

            migrationBuilder.DropTable(
                name: "certificatestatuses");

            migrationBuilder.DropTable(
                name: "storedfiles");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "Edu_Users");

            migrationBuilder.DropTable(
                name: "Edu_OrgUnitTypes");
        }
    }
}
