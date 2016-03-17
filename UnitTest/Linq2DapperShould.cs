using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper.Contrib.Linq2Dapper.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.Model;

namespace UnitTest
{
    [TestClass]
    public class Linq2DapperShould
    {
        private static string ConnectionString { get { return "Data Source=.;Database=Test;Trusted_Connection=True;"; } }

        [TestMethod]
        public void SelectAllRecords()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>().ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void SelectAllRecords2()
        {
            var cntx = new DataContext(ConnectionString);

            var results = cntx.DataTypes.Where(x => x.Name == "text").ToList();
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void SelectSpeedTest()
        {
            for (int i = 0; i < 500; i++)
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();
                    var results = cn.Query<DataType>().ToList();
                    Assert.AreEqual(5, results.Count);
                }
            }
        }

        [TestMethod]
        public void JoinWhere()
        {
            var cntx = new DataContext(ConnectionString);

            var results = (from d in cntx.DataTypes
                           join a in cntx.Fields on d.DataTypeId equals a.DataTypeId
                           where a.DataTypeId == 1
                           select d).ToList();

            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void MultiJoinWhere()
        {
            var cntx = new DataContext(ConnectionString);

            var results = (from d in cntx.DataTypes
                           join a in cntx.Fields on d.DataTypeId equals a.DataTypeId
                           join b in cntx.Documents on a.FieldId equals b.FieldId
                           where a.DataTypeId == 1 && b.FieldId == 1
                           select d).ToList();

            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void WhereContains()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var r = (from a in cn.Query<DataType>()
<<<<<<< HEAD
                         where new[] { "text", "int", "random" }.Contains(a.Name)
                         orderby a.Name
                         select a).ToList();

=======
                        where new[] { "text", "int", "random" }.Contains(a.Name)
                        orderby a.Name
                        select a).ToList();
                    
>>>>>>> 34cf3c7824d309860f0fd3becc8e7c4ccb8fe424
                Assert.AreEqual(2, r.Count);
            }
        }

        [TestMethod]
        public void WhereEquals()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                foreach (var item in new[] { "text", "int" })
                {
                    var results = cn.Query<DataType>(x => x.Name == item).ToList();
                    Assert.AreEqual(1, results.Count);
                }
            }
        }


        [TestMethod]
        public void Top1Statement()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>().FirstOrDefault(m => !m.IsActive);
                Assert.IsNotNull(results);
            }
        }


        [TestMethod]
        public void Top10A()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>().Take(5).ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void Top10B()
        {
            const int topCount = 10;
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>().Take(topCount).ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void Top10C()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                for (int topCount = 1; topCount < 5; topCount++)
                {
                    var results = cn.Query<DataType>().Take(topCount).ToList();
                    Assert.AreEqual(topCount, results.Count);
                }
            }
        }

        [TestMethod]
        public void Distinct()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>().Distinct().ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void OrderBy()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>().OrderBy(m => m.Name).ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void OrderByAndThenBy()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>().OrderBy(m => m.Name).ThenBy(m => m.DataTypeId).ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void OrderByWithTop()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>().OrderBy(m => m.Name).ThenBy(m => m.DataTypeId).Take(5).ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void WhereSimpleEqual()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name == "text").ToList();
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual("text", results[0].Name);
            }
        }

        [TestMethod]
        public void WhereSimpleEqualWithoutParameter()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.DataTypeId == m.DataTypeId).ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void WhereIsNullOrEmpty()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => String.IsNullOrEmpty(m.Name)).ToList();
                Assert.AreEqual(0, results.Count);
            }
        }

        [TestMethod]
        public void WhereNotIsNullOrEmpty()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => !String.IsNullOrEmpty(m.Name)).ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void WhereHasValue()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Created.HasValue).ToList();
                Assert.AreEqual(5, results.Count);
            }
        }

        [TestMethod]
        public void WhereNotHasValue()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => !m.Created.HasValue).ToList();
                Assert.AreEqual(0, results.Count);
            }
        }

        [TestMethod]
        public void WhereLike()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name.Contains("te")).ToList();
                Assert.AreEqual(2, results.Count);
            }
        }

        [TestMethod]
        public void WhereEndsWith()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name.StartsWith("te")).ToList();
                Assert.AreEqual(1, results.Count);
            }
        }

        [TestMethod]
        public void WhereStartsWith()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name.EndsWith("xt")).ToList();
                Assert.AreEqual(1, results.Count);
            }
        }

        [TestMethod]
        public void WhereEndsWithAndComparison()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name.StartsWith("te", StringComparison.OrdinalIgnoreCase)).ToList();
                Assert.AreEqual(1, results.Count);
            }
        }

        [TestMethod]
        public void WhereStartsWithAndComparison()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name.EndsWith("xt", StringComparison.OrdinalIgnoreCase)).ToList();
                Assert.AreEqual(1, results.Count);
            }
        }

        [TestMethod]
        public void WhereNotLike()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => !m.Name.Contains("te")).ToList();
                Assert.AreEqual(3, results.Count);
            }
        }

        [TestMethod]
        public void WhereNotEndsWith()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => !m.Name.StartsWith("te")).ToList();
                Assert.AreEqual(4, results.Count);
            }
        }

        [TestMethod]
        public void WhereNotStartsWith()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => !m.Name.EndsWith("xt")).ToList();
                Assert.AreEqual(4, results.Count);
            }
        }

        [TestMethod]
        public void TwoPartWhereAnd()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name == "text" && m.Created.HasValue).ToList();
                Assert.AreEqual(1, results.Count);
            }
        }

        [TestMethod]
        public void TwoPartWhereOr()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name == "text" || m.Name == "int").ToList();
                Assert.AreEqual(2, results.Count);
            }
        }

        [TestMethod]
        public void MultiPartWhereAndOr()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name == "text" && (m.Name == "int" || m.Created.HasValue)).ToList();
                Assert.AreEqual(1, results.Count);
            }
        }

        [TestMethod]
        public void MultiPartWhereAndOr2()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var results = cn.Query<DataType>(m => m.Name != "text" && (m.Name == "int" || m.Created.HasValue)).ToList();
                Assert.AreEqual(4, results.Count);
            }
        }

        [TestMethod]
        public void Single()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                cn.Open();
                var result = cn.Query<DataType>().Single(m => m.Name == "text");
                Assert.AreEqual("text", result.Name);
            }
        }
    }
}
