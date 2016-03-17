using System;
using System.Collections.Generic;

namespace Dapper.Contrib.Linq2Dapper.Helpers
{
    internal static class TypeSystem
    {
        internal static Type GetElementType(Type seqType)
        {
            var ienum = FindIEnumerable(seqType);
            return ienum == null ? seqType : ienum.GetGenericArguments()[0];
        }

        internal static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof (string))
                return null;
            if (seqType.IsArray)
                return typeof (IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (var arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof (IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            var ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (var iface in ifaces)
                {
                    var ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof (object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }
}
