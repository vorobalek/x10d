namespace X10D.Core.Services
{
    internal interface IStoredCache
    {
        string this[string key] { get; set; }
    }
}