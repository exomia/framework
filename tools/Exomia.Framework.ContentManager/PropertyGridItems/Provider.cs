namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A provider.
    /// </summary>
    public static class Provider
    {
        public delegate T Value<out T>();
        public static Value<T> Static<T>(T item)
        {
            return () => item;
        }
    }
}
