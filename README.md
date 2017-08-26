# Linq2Dapper

A simple lightweight Linq provider for Dapper.Net
Light weight Linq support for a light weight ORM. Linq2Dapper allows you to use type-safe Linq queries without the overhead of Linq2SQL or EntityFramework.

## Implementation

Add the Linq2Dapper Nuget package from nuget.org

    install-package Linq2Dapper

### Using the extension methods

Include the extensions reference in your code

```C#
using Dapper.Contrib.Linq2Dapper.Extensions
```

Then choose your flavour; because Linq2Dapper implements `IQueryable<T>` you can use either Linq or lambda queries to achieve clean results

```C#
var thing = _connection.Query<ModelName>(a => a.Id == 1).Single();
    
var thing = (from a in _connection.Query<ModelName>()
	    where a.Id == 1
	    select a).Single();
```

And better yet it doesn't interfere with the standard Dapper extensions, meaning you mix Linq and hard coded SQL into code without the fear of either breaking

```C#
_connection.Query<ModelName>("SELECT Id, Name FROM ModelName WHERE Id = @Id", new { Id = 1});
_connection.Query<ModelName>(a => a.Id == 1);
```


### Using a data context

This approach gives a similar feel to implementing EntityFramework `IDBContext` types, only with a lot less fuss and no black box magic going on behind the scenes

```C#
public class DataContext : IDisposable
{
    private readonly SqlConnection _connection;

    private Linq2Dapper<ModelName> _modelNames;
    public Linq2Dapper<ModelName> ModelNames => 
	     _modelNames ?? (_modelNames= CreateObject<ModelName>());

    public DataContext(string connectionString) : 
    	this(new SqlConnection(connectionString)) { }

    public DataContext(SqlConnection connection)
    {
    	_connection = connection;
    }

    private Linq2Dapper<T> CreateObject<T>()
    {
    	return new Linq2Dapper<T>(_connection);
    }

    public void Dispose()
    {
    	_connection.Dispose();
    }
}
    
    
//using the context type in your code 
var context = new Datacontext("ConnectionString");
context.ModelNames.Where(a => a.Name == "Name");
```

## Clean SQL no bloat

Linq2Dapper outputs clean and simple SQL; no nested queries or selects, no unoptimised code.

For example a Linq query like this

```C#
var results = (from d in cntx.DataTypes
               join a in cntx.Fields on d.DataTypeId equals a.DataTypeId
               join b in cntx.Documents on a.FieldId equals b.FieldId
               where a.DataTypeId == 1 && b.FieldId == 1
	       select d).ToList();
```

Will produce the following SQL statement

```SQL
SELECT t1.[DataTypeId], t1.[Name], t1.[IsActive], t1.[Created] 
FROM [DataType] t1 
JOIN [Field] t2 ON t1.[DataTypeId] = t2.[DataTypeId] 
JOIN [Document] t3 ON t2.[FieldId] = t3.[FieldId] 
WHERE ((t2.[DataTypeId] = @ld__1) AND (t3.[FieldId] = @ld__2))
```

### Query support

**TOP**

```C#
.Take(1..n)
```

**DISTINCT**

```C#
.Distinct()
```

**LIKE** and **NOT LIKE**

```C#
.StartsWith("...")
.EndsWith("...")
.Contains("...")
```
Use the `!` operator for `NOT LIKE` 

**JOIN**

```C#
from ... in ...
join ... in ... on ... equals ...
select ...

.Join(...)
```

**ORDERBY**

```C#
.OrderBy[Descending](...)
.ThenBy[Descending](..)
```
