using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.DatabaseTableClasses;

namespace WebAPI.Database.Interaction
{
    public static class InteractionDatabase
    {
        public static bool Uniqueness(string table, string columns, string data)
        {
            try
            {
                return !new PetaPoco.Database("DatabaseConnect").ExecuteScalar<bool>($"select count(*) from {table} where {columns}=@data", new { data });
            }
            catch { return false; }
        }
        public static bool Add(string table, string columns, string data)
        {
            try
            {
                new PetaPoco.Database("DatabaseConnect").Execute($"INSERT INTO {table}({columns}) values({data})");
                return true;
            }
            catch { return false; }
        }
    }
}