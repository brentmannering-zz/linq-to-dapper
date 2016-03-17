using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.Model;

namespace UnitTest
{
    public class EfDbContext : DbContext
    {
        public EfDbContext(string connectionString) : base(connectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataType>();
            modelBuilder.Entity<Document>();
            modelBuilder.Entity<Field>();
        }

    }

    [TestClass]
    public class EFComparison
    {
        private static string ConnectionString { get { return "Data Source=.;Database=Test;Trusted_Connection=True;"; } }
        private readonly DbContext _context;

        public EFComparison()
        {
            _context = new EfDbContext(ConnectionString);
        }

        [TestMethod]
        public void SelectAllRecords()
        {
            var results = _context.Set<DataType>().ToList();
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void SelectAllRecords2()
        {
            var results = _context.Set<DataType>().Where(x => x.Name == "text").ToList();
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void JoinWhere()
        {
            var cntx = new DataContext(ConnectionString);

            var results = (from d in _context.Set<DataType>()
                           join a in _context.Set<Field>() on d.DataTypeId equals a.DataTypeId
                           where a.DataTypeId == 1
                           select d).ToList();

            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void MultiJoinWhere()
        {
            var results = (from d in _context.Set<DataType>()
                           join a in _context.Set<Field>() on d.DataTypeId equals a.DataTypeId
                           join b in _context.Set<Document>() on a.FieldId equals b.FieldId
                           where a.DataTypeId == 1 && b.FieldId == 1
                           select d).ToList();

            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void WhereContains()
        {
            var r = (from a in _context.Set<DataType>()
                     where new[] { "text", "int", "random" }.Contains(a.Name)
                     orderby a.Name
                     select a).ToList();

            Assert.AreEqual(2, r.Count);

        }

        [TestMethod]
        public void WhereEquals()
        {
            foreach (var item in new[] { "text", "int" })
            {
                var results = _context.Set<DataType>().Where(x => x.Name == item).ToList();
                Assert.AreEqual(1, results.Count);
            }
        }


        [TestMethod]
        public void Top1Statement()
        {
            var results = _context.Set<DataType>().FirstOrDefault(m => !m.IsActive);
            Assert.IsNotNull(results);
        }


        [TestMethod]
        public void Top10A()
        {
            var results = _context.Set<DataType>().Take(5).ToList();
            Assert.AreEqual(5, results.Count);

        }

        [TestMethod]
        public void Top10B()
        {
            const int topCount = 10;
            var results = _context.Set<DataType>().Take(topCount).ToList();
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void Top10C()
        {
            for (int topCount = 1; topCount < 5; topCount++)
            {
                var results = _context.Set<DataType>().Take(topCount).ToList();
                Assert.AreEqual(topCount, results.Count);
            }
        }

        [TestMethod]
        public void Distinct()
        {
            var results = _context.Set<DataType>().Distinct().ToList();
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void OrderBy()
        {
            var results = _context.Set<DataType>().OrderBy(m => m.Name).ToList();
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void OrderByAndThenBy()
        {
            var results = _context.Set<DataType>().OrderBy(m => m.Name).ThenBy(m => m.DataTypeId).ToList();
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void OrderByWithTop()
        {
            var results = _context.Set<DataType>().OrderBy(m => m.Name).ThenBy(m => m.DataTypeId).Take(5).ToList();
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void WhereSimpleEqual()
        {
            var results = _context.Set<DataType>().Where(m => m.Name == "text").ToList();
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("text", results[0].Name);
        }

        [TestMethod]
        public void WhereSimpleEqualWithoutParameter()
        {
            var results = _context.Set<DataType>().Where(m => m.DataTypeId == m.DataTypeId).ToList();
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void WhereIsNullOrEmpty()
        {
            var results = _context.Set<DataType>().Where(m => String.IsNullOrEmpty(m.Name)).ToList();
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void WhereNotIsNullOrEmpty()
        {
            var results = _context.Set<DataType>().Where(m => !String.IsNullOrEmpty(m.Name)).ToList();
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void WhereHasValue()
        {
            var results = _context.Set<DataType>().Where(m => m.Created.HasValue).ToList();
            Assert.AreEqual(5, results.Count);
        }

        [TestMethod]
        public void WhereNotHasValue()
        {
            var results = _context.Set<DataType>().Where(m => !m.Created.HasValue).ToList();
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void WhereLike()
        {
            var results = _context.Set<DataType>().Where(m => m.Name.Contains("te")).ToList();
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void WhereEndsWith()
        {

            var results = _context.Set<DataType>().Where(m => m.Name.StartsWith("te")).ToList();
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void WhereStartsWith()
        {

            var results = _context.Set<DataType>().Where(m => m.Name.EndsWith("xt")).ToList();
            Assert.AreEqual(1, results.Count);
        }

        //[TestMethod]
        //public void WhereEndsWithAndComparison()
        //{

        //    //var results = _context.Set<DataType>().Where(m => m.Name.StartsWith("te", StringComparison.OrdinalIgnoreCase)).ToList();
        //    //Assert.AreEqual(1, results.Count);
        //}

        //[TestMethod]
        //public void WhereStartsWithAndComparison()
        //{

        //    //var results = _context.Set<DataType>().Where(m => m.Name.EndsWith("xt", StringComparison.OrdinalIgnoreCase)).ToList();
        //    //Assert.AreEqual(1, results.Count);

        //}

        [TestMethod]
        public void WhereNotLike()
        {

            var results = _context.Set<DataType>().Where(m => !m.Name.Contains("te")).ToList();
            Assert.AreEqual(3, results.Count);

        }

        [TestMethod]
        public void WhereNotEndsWith()
        {

            var results = _context.Set<DataType>().Where(m => !m.Name.StartsWith("te")).ToList();
            Assert.AreEqual(4, results.Count);
        }

        [TestMethod]
        public void WhereNotStartsWith()
        {
            var results = _context.Set<DataType>().Where(m => !m.Name.EndsWith("xt")).ToList();
            Assert.AreEqual(4, results.Count);
        }

        [TestMethod]
        public void TwoPartWhereAnd()
        {
            var results = _context.Set<DataType>().Where(m => m.Name == "text" && m.Created.HasValue).ToList();
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void TwoPartWhereOr()
        {

            var results = _context.Set<DataType>().Where(m => m.Name == "text" || m.Name == "int").ToList();
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void MultiPartWhereAndOr()
        {
            var results = _context.Set<DataType>().Where(m => m.Name == "text" && (m.Name == "int" || m.Created.HasValue)).ToList();
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void MultiPartWhereAndOr2()
        {
            var results = _context.Set<DataType>().Where(m => m.Name != "text" && (m.Name == "int" || m.Created.HasValue)).ToList();
            Assert.AreEqual(4, results.Count);

        }

        [TestMethod]
        public void Single()
        {

            var result = _context.Set<DataType>().Single(m => m.Name == "text");
            Assert.AreEqual("text", result.Name);

        }
    }
}
