using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TodoApi.Models;

namespace TodoApi.DTO
{
    public class DBChange
    {
        
        public static ReservationsDbContext DBaseChange(string Key, IConfiguration configuration)
        {
            string DBName = "";
            //switch (Key)
            //{
            //    case "TestDbConn":
            //        DBName = "TestDbConn";
            //        break;
            //    case "ReservationsDbConn":
            //        DBName = "ReservationsDbConn";
            //        break;
            //    default:
            //        DBName = "ReservationsDbConn";
            //        break;
            //}
            if (Key == "" || Key == null)
            {
                DBName = "ReservationsDbConn";
            }
            else
            {
                DBName = Key;
            }
               
            IConfiguration _Configuration = configuration;
            var optionsBuilder = new DbContextOptionsBuilder<ReservationsDbContext>();
            string connectionstring = _Configuration.GetConnectionString(DBName);
            optionsBuilder.UseSqlServer(connectionstring);
            return new ReservationsDbContext(optionsBuilder.Options);
        }
    }
}
