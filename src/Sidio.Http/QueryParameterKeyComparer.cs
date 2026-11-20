namespace Sidio.Http;

internal sealed class QueryParameterKeyComparer : IEqualityComparer<EncodedQueryParameterKey>
{
    public bool Equals(EncodedQueryParameterKey x, EncodedQueryParameterKey y) => string.Equals(
        x.Value,
        y.Value,
        StringComparison.OrdinalIgnoreCase);

    public int GetHashCode(EncodedQueryParameterKey obj) =>
        obj.Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
}