using System;
using System.Collections.Generic;

public interface ITypeList
{
    Type listType { get; }
}

public class TypeList<T> : List<T>, ITypeList
{
    public TypeList() : base() { }
    public TypeList(int capacty) : base(capacty) { }
    public TypeList(IEnumerable<T> collection) : base(collection) { }

    public Type listType => typeof(T);
}

public static class TypeListUtility
{
    public static TypeList<TSource> ToTypeList<TSource>(this IEnumerable<TSource> source) => new TypeList<TSource>(source);
}
