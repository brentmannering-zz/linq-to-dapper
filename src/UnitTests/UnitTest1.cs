using System;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using Dapper.Contrib.Linq2Dapper;
using Dapper.Contrib.Linq2Dapper.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Models;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            SqlMapper.AddTypeHandler(DateTimeHandler.Default);
        }
        [TestMethod]
        public void TestMethod1()
        {
            var dbFolder = Path.GetFullPath(@"..\..\..\Resources\");
            var dbFile = Path.Combine(dbFolder, "test.db");
            var tmpDbFile = Path.Combine(dbFolder, "test_tmp.db");

            File.Copy(dbFile,tmpDbFile,true);

            var connectionString = $@"Filename={tmpDbFile}";
            var connection = new SqliteConnection(connectionString);

            connection.Execute(
                "insert into Machine (Id,Name,DeliveredOn,IsCool) values (@Id,@Name,@DeliveredOn,@IsCool)",
                new { Id = 0, Name = "A", DeliveredOn = new DateTime(2000, 1, 1).Ticks, IsCool = false });
            connection.Execute(
                "insert into Machine (Id,Name,DeliveredOn,IsCool) values (@Id,@Name,@DeliveredOn,@IsCool)",
                new { Id = 1, Name = "B", DeliveredOn = new DateTime(2000, 1, 2).Ticks, IsCool = false });
            connection.Execute(
                "insert into Machine (Id,Name,DeliveredOn,IsCool) values (@Id,@Name,@DeliveredOn,@IsCool)",
                new { Id = 2, Name = "C", DeliveredOn = new DateTime(2000, 1, 3).Ticks, IsCool = true });
            connection.Execute(
                "insert into Machine (Id,Name,DeliveredOn,IsCool) values (@Id,@Name,@DeliveredOn,@IsCool)",
                new { Id = 3, Name = "D", DeliveredOn = new DateTime(2000, 1, 4).Ticks, IsCool = false });

            var query = connection.Query<Machine>(x => x.Id == 2);
           
            var results = query.AsList();
            Assert.AreEqual(1,results.Count);
            Assert.AreEqual(2, results.First().Id);
            Assert.AreEqual(new DateTime(2000, 1, 3), results.First().DeliveredOn);


        }

        public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
        {
            public static readonly DateTimeHandler Default = new DateTimeHandler();

            public override void SetValue(IDbDataParameter parameter, DateTime value)
            {

                parameter.Value = value.Ticks;
            }

            public override DateTime Parse(object value)
            {
                var ticks = Convert.ToInt64(value);
                //var ticks = (long)value;
                return new DateTime(ticks);
            }
        }
    }
}
