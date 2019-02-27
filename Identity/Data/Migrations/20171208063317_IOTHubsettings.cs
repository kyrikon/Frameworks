using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PLEXOS.Identity.Data.Migrations
{
    public partial class IOTHubsettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "DeviceID",
            //    table: "UserApiAccess",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "DeviceKey",
            //    table: "UserApiAccess",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "NotificationEndpoint",
            //    table: "UserApiAccess",
            //    nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceID",
                table: "UserApiAccess");

            migrationBuilder.DropColumn(
                name: "DeviceKey",
                table: "UserApiAccess");

            migrationBuilder.DropColumn(
                name: "NotificationEndpoint",
                table: "UserApiAccess");
        }
    }
}
