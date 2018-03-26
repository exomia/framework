using System;

namespace Exomia.Framework.ContentSerialization.Types
{
    internal interface IType
    {
        string TypeName { get; }
        Type BaseType { get; }

        bool IsPrimitive { get; }

        string CreateTypeInfo(Type type);

        Type CreateType(string genericTypeInfo);

        object Read(CSStreamReader stream, string key, string genericTypeInfo, string dimensionInfo);

        void Write(Action<string, string> writeHandler, string tabSpace, string key, object content,
            bool useTypeInfo = true);
    }
}