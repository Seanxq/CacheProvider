namespace CacheProvider
{
    public static class GlobalUtilities
    {
        public static bool DoesPropertyExist(dynamic obj, string name)
        {
            return obj.GetType().GetProperty(name) != null;
        }

    }
}