namespace Microsoft.OpenApi.YamlReader;

/// <summary>
/// Tracks the resource budget consumed while materializing a single YAML document.
/// </summary>
/// <remarks>
/// <para>
/// A budget instance is scoped to one document and is not thread safe. Callers charge the budget
/// <em>before</em> allocating, so a document that would breach a limit is rejected without the
/// allocation ever happening.
/// </para>
/// <para>
/// Every limit breach throws <see cref="OpenApiReaderException"/>, which the reader converts into an
/// <c>OpenApiDiagnostic</c>. That is the whole point of this type: hostile input produces a reportable
/// diagnostic rather than an unrecoverable process failure.
/// </para>
/// </remarks>
internal sealed class YamlConversionBudget
{
    private readonly uint _maxDepth;
    private readonly uint _maxNodeCount;
    private readonly uint _maxAliasExpansionNodeCount;
    private uint _nodeCount;
    private uint _aliasExpansionNodeCount;

    /// <summary>
    /// Initializes a budget for a single document.
    /// </summary>
    /// <param name="maxDepth">Maximum nesting depth. Bounds stack and structural growth.</param>
    /// <param name="maxNodeCount">Maximum total nodes materialized from the document.</param>
    /// <param name="maxAliasExpansionNodeCount">
    /// Maximum nodes materialized specifically by expanding aliases. This is the anti-amplification
    /// limit and is deliberately far smaller than <paramref name="maxNodeCount"/>: a large document is
    /// legitimate, but a small document that <em>expands</em> into a large one is not.
    /// </param>
    public YamlConversionBudget(uint maxDepth, uint maxNodeCount, uint maxAliasExpansionNodeCount)
    {
        _maxDepth = maxDepth;
        _maxNodeCount = maxNodeCount;
        _maxAliasExpansionNodeCount = maxAliasExpansionNodeCount;
    }

    /// <summary>
    /// Charges one node at the supplied depth.
    /// </summary>
    /// <param name="depth">Nesting depth of the node being materialized.</param>
    /// <exception cref="OpenApiReaderException">The depth or total node limit would be exceeded.</exception>
    public void EnterNode(uint depth)
    {
        if (depth > _maxDepth)
        {
            throw new OpenApiReaderException($"The YAML document exceeds the maximum supported nesting depth of {_maxDepth}.");
        }

        AddNodes(1);
    }

    /// <summary>
    /// Charges the full cost of expanding an alias, against both the alias budget and the total budget.
    /// </summary>
    /// <param name="depth">Nesting depth at which the alias appears.</param>
    /// <param name="expandedNodeCount">Number of nodes the alias will materialize when cloned.</param>
    /// <exception cref="OpenApiReaderException">The depth, alias, or total node limit would be exceeded.</exception>
    /// <remarks>
    /// Must be called before the clone is taken. Charging afterwards would allow the very allocation
    /// this limit exists to prevent.
    /// </remarks>
    public void EnterAlias(uint depth, uint expandedNodeCount)
    {
        if (depth > _maxDepth)
        {
            throw new OpenApiReaderException($"The YAML document exceeds the maximum supported nesting depth of {_maxDepth}.");
        }

        if (expandedNodeCount > _maxAliasExpansionNodeCount - _aliasExpansionNodeCount)
        {
            throw new OpenApiReaderException($"The YAML document expands aliases to more than the maximum supported number of nodes ({_maxAliasExpansionNodeCount}).");
        }

        _aliasExpansionNodeCount += expandedNodeCount;
        AddNodes(expandedNodeCount);
    }

    /// <summary>
    /// Charges <paramref name="count"/> nodes against the total node budget.
    /// </summary>
    /// <remarks>
    /// The remaining headroom is compared as <c>count &gt; _maxNodeCount - _nodeCount</c> rather than
    /// <c>_nodeCount + count &gt; _maxNodeCount</c>. Both operands are unsigned, so the latter form could
    /// wrap and silently admit an over-budget document; the invariant <c>_nodeCount &lt;= _maxNodeCount</c>
    /// makes the subtraction used here safe from underflow.
    /// </remarks>
    private void AddNodes(uint count)
    {
        if (count > _maxNodeCount - _nodeCount)
        {
            throw new OpenApiReaderException($"The YAML document expands to more than the maximum supported number of nodes ({_maxNodeCount}). This may indicate a YAML anchor/alias expansion attack.");
        }

        _nodeCount += count;
    }
}
