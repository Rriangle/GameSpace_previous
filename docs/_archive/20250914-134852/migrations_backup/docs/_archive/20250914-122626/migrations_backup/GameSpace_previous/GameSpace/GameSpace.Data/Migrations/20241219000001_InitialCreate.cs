using Microsoft.EntityFrameworkCore.Migrations;

namespace GameSpace.Migrations
{
    /// <summary>
    /// 初始數據庫創建遷移
    /// </summary>
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 創建用戶相關表
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    User_Account = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    User_Password = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    User_EmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    User_PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    User_TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    User_AccessFailedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    User_LockoutEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    User_LockoutEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_ID);
                });

            // 創建用戶介紹表
            migrationBuilder.CreateTable(
                name: "User_Introduce",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    User_NickName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Gender = table.Column<string>(type: "char(1)", maxLength: 1, nullable: false),
                    IdNumber = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Cellphone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Create_Account = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User_Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    User_Introduce = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Introduce", x => x.User_ID);
                    table.ForeignKey(
                        name: "FK_User_Introduce_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // 創建用戶權限表
            migrationBuilder.CreateTable(
                name: "User_Rights",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    User_Status = table.Column<bool>(type: "bit", nullable: true),
                    ShoppingPermission = table.Column<bool>(type: "bit", nullable: true),
                    MessagePermission = table.Column<bool>(type: "bit", nullable: true),
                    SalesAuthority = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Rights", x => x.User_Id);
                    table.ForeignKey(
                        name: "FK_User_Rights_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // 創建用戶錢包表
            migrationBuilder.CreateTable(
                name: "User_Wallet",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    User_Point = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Wallet", x => x.User_Id);
                    table.ForeignKey(
                        name: "FK_User_Wallet_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // 創建寵物表
            migrationBuilder.CreateTable(
                name: "Pet",
                columns: table => new
                {
                    PetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    PetName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "小可愛"),
                    Level = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LevelUpTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Experience = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Hunger = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Mood = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Stamina = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Cleanliness = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Health = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SkinColor = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "#ADD8E6"),
                    SkinColorChangedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    BackgroundColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "粉藍"),
                    BackgroundColorChangedTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    PointsChanged_SkinColor = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PointsChanged_BackgroundColor = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PointsGained_LevelUp = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PointsGainedTime_LevelUp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pet", x => x.PetID);
                    table.ForeignKey(
                        name: "FK_Pet_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            // 創建索引
            migrationBuilder.CreateIndex(
                name: "IX_Users_User_Account",
                table: "Users",
                column: "User_Account",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_User_name",
                table: "Users",
                column: "User_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Introduce_Cellphone",
                table: "User_Introduce",
                column: "Cellphone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Introduce_Email",
                table: "User_Introduce",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Introduce_IdNumber",
                table: "User_Introduce",
                column: "IdNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Introduce_User_NickName",
                table: "User_Introduce",
                column: "User_NickName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pet_UserID",
                table: "Pet",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Pet");
            migrationBuilder.DropTable(name: "User_Wallet");
            migrationBuilder.DropTable(name: "User_Rights");
            migrationBuilder.DropTable(name: "User_Introduce");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
