using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dapper.Contrib.Linq2Dapper.Helpers
{
    internal static class CacheHelper
    {
        private static readonly ConcurrentDictionary<Type, List<string>> _typePropertyList;
        private static readonly ConcurrentDictionary<Type, string> _typeTableName;

        static CacheHelper()
        {
            if (_typePropertyList == null)
                _typePropertyList = new ConcurrentDictionary<Type, List<string>>();

            if (_typeTableName == null)
                _typeTableName = new ConcurrentDictionary<Type, string>();
        }

        internal static bool TryAddPropertyList<T>(List<string> properties)
        {
            return _typePropertyList.TryAdd(typeof(T), properties);
        }

        internal static List<string> TryGetPropertyList<T>()
        {
            List<string> propList;

            if (!_typePropertyList.TryGetValue(typeof(T), out propList))
                throw new Exception("Invalid type: " + typeof(T));

            return propList;
        }

        internal static bool TryAddTableName<T>(string tableName)
        {
            return _typeTableName.TryAdd(typeof(T), tableName);
        }

        internal static string TryGetTableName<T>()
        {
            string tableName;
            if (!_typeTableName.TryGetValue(typeof(T), out tableName))
                throw new Exception("Invalid type: " + typeof(T));

            return tableName;
        }
    }
}
