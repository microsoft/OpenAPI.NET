using Json.Schema;

namespace Microsoft.OpenApi.Draft4Support;

internal static class Draft4SupportData
{
    // This is kind of a hack since SpecVersion is an enum.
    // Maybe it should be defined as string constants.
    public const SpecVersion Draft4Version = (SpecVersion)(1 << 10);
}
