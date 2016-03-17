using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Linq2Dapper;

namespace UnitTest.Model
{
    public class DataContext
    {
        private readonly SqlConnection _connection;

        private Linq2Dapper<DataType> _dataTypes;
        public Linq2Dapper<DataType> DataTypes {
            get { return _dataTypes ?? (_dataTypes = CreateObject<DataType>()); }
        }

        private Linq2Dapper<Field> _fields;
        public Linq2Dapper<Field> Fields
        {
            get { return _fields ?? (_fields = CreateObject<Field>()); }
        }

        private Linq2Dapper<Document> _documents;
        public Linq2Dapper<Document> Documents
        {
            get { return _documents ?? (_documents = CreateObject<Document>()); }
        }


        public DataContext(string connectionString) : this(new SqlConnection(connectionString)) { }

        public DataContext(SqlConnection connection)
        {
            _connection = connection;
        }

        private Linq2Dapper<T> CreateObject<T>()
        {
            return new Linq2Dapper<T>(_connection);
        }
        
    }
}
