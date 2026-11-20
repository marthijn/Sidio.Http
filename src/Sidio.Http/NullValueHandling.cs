namespace Sidio.Http;

public enum NullValueHandling
{
    /// <summary>
    /// Indicates that null values should be ignored and not included in the query parameters.
    /// Any existing value for the parameter will remain unchanged.
    /// </summary>
    Ignore,

    /// <summary>
    /// Indicates that null values should be included in the query parameters. Any existing value for the parameter will
    /// be replaced with null.
    /// </summary>
    Include,

    /// <summary>
    /// Indicates that null values should not be included in the query parameters, and any
    /// existing value for the parameter should be removed.
    /// </summary>
    RemoveWhenNull,
}