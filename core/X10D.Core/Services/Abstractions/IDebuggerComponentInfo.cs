namespace X10D.Core.Services
{
    internal interface IDebuggerCompomnentInfo
    {
        string Key { get; }
        string Description { get; }
        static IDebuggerCompomnentInfo Create(string key, string description)
        {
            return new DebuggerComponentInfo()
            {
                Key = key,
                Description = description,
            };
        }

        private sealed class DebuggerComponentInfo : IDebuggerCompomnentInfo
        {
            public string Key { get; set; }
            public string Description { get; set; }
        }
    }
}
