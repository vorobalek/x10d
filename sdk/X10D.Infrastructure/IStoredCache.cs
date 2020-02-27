namespace X10D.Infrastructure
{
    public interface IStoredCache
    {
        string this[string key] { get; set; }
    }
}