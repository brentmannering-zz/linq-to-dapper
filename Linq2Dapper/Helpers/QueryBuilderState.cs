using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Contrib.Linq2Dapper.Helpers
{
    internal class QueryBuilderState
    {

    }

    internal class SelectPart
    {
        
    }

    internal class JoinPart
    {
        internal string Table;
        internal string OnField;
        internal string EqualsField;
        internal string Indentifier;
    }

    internal class WherePart
    {
        internal string Name;
        internal string Operator;
        internal string Value;
    }

    internal class OrderPart
    {
        
    }


}
