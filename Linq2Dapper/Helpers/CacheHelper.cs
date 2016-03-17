using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dapper.Contrib.Linq2Dapper.Exceptions;

namespace Dapper.Contrib.Linq2Dapper.Helpers
{
    internal class TableHelper
    {
        internal string Name { get; set; }
        internal Dictionary<string, string> Columns { get; set; }
        internal string Identifier { get; set; }
    }

    internal static class CacheHelper
    {
        internal static int Size {
            get { return _typeList.Count; }
        }

        private static readonly ConcurrentDictionary<Type, TableHelper> _typeList;

        static CacheHelper()
        {
            if (_typeList == null)
                _typeList = new ConcurrentDictionary<Type, TableHelper>();
        }

        internal static bool HasCache<T>()
        {
            return HasCache(typeof (T));
        }

        internal static bool HasCache(Type type)
        {
            TableHelper table;
            return _typeList.TryGetValue(type, out table);
        }

        internal static bool TryAddTable<T>(TableHelper table)
        {
            return TryAddTable(typeof(T), table);
        }

        internal static bool TryAddTable(Type type, TableHelper table)
        {
            return _typeList.TryAdd(type, table);
        }

        internal static TableHelper TryGetTable<T>()
        {
            return TryGetTable(typeof(T));
        }

        internal static TableHelper TryGetTable(Type type)
        {
            TableHelper table;
            return !_typeList.TryGetValue(type, out table) ? new TableHelper() : table;
        }

        internal static string TryGetIdentifier<T>()
        {
            return TryGetIdentifier(typeof(T));
        }

        internal static string TryGetIdentifier(Type type)
        {
            return TryGetTable(type).Identifier;
        }

        internal static Dictionary<string, string> TryGetPropertyList<T>()
        {
            return TryGetPropertyList(typeof(T));
        }

        internal static Dictionary<string, string> TryGetPropertyList(Type type)
        {
            return TryGetTable(type).Columns;
        }

        internal static string TryGetTableName<T>()
        {
            return TryGetTableName(typeof(T));
        }

        internal static string TryGetTableName(Type type)
        {
            return TryGetTable(type).Name;
        }

    }
}
