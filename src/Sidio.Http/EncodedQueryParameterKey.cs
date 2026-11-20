using System.Diagnostics;

namespace Sidio.Http;

[DebuggerDisplay("{Value}")]
internal readonly record struct EncodedQueryParameterKey
{
    private EncodedQueryParameterKey(string value, bool encode)
    {
        Value = encode ? Uri.EscapeDataString(value) : value;
    }

    public string Value { get; }

    public override string ToString() => Value;

    public static EncodedQueryParameterKey FromString(string value, bool encode)
    {
        // technically, a query parameter key can be empty or white spaces, but we will not allow that in this implementation
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new EncodedQueryParameterKey(value, encode);
    }
}