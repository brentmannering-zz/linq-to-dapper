using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Contrib.Linq2Dapper.Helpers
{
    internal class MethodCall
    {
        internal const string EndsWith = "EndsWith";
        internal const string StartsWith = "StartsWith";
        internal const string Contains = "Contains";
        internal const string IsNullOrEmpty = "IsNullOrEmpty";
        internal const string Join = "Join";
        internal const string Single = "Single";
        internal const string First = "First";
        internal const string FirstOrDefault = "FirstOrDefault";
        internal const string Take = "Take";
        internal const string Distinct = "Distinct";
        internal const string OrderBy = "OrderBy";
        internal const string ThenBy = "ThenBy";
        internal const string OrderByDescending = "OrderByDescending";
        internal const string ThenByDescending = "ThenByDescending";
    }
}
