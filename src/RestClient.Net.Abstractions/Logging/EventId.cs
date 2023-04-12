#pragma warning disable CA1815 // Override equals and operator equals on value types
#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace Microsoft.Extensions.Logging
{
    public readonly struct EventId
    {
        public EventId(int id, string? name = null)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string? Name { get; }
    }
}

