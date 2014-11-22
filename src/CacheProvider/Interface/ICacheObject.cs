namespace CacheProvider.Interface
{
    public interface ICacheObject
    {
        string Validator { get; set; }
        byte[] Item { get; set; }
    }
}