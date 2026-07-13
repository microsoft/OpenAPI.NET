using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V31;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests;

public class OpenApiDynamicRefTests
{
    private static async Task<OpenApiDocument> LoadDocumentAsync(string yaml)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(yaml));
        var result = await OpenApiDocument.LoadAsync(stream, "yaml", SettingsFixture.ReaderSettings);
        return result.Document;
    }

    [Fact]
    public void BareDynamicRefDeserializesAsSchemaReference()
    {
        var json =
        """
        {
            "$dynamicRef": "#category"
        }
        """;

        var hostDocument = new OpenApiDocument();
        var jsonNode = JsonNode.Parse(json);

        var result = OpenApiV31Deserializer.LoadSchema(jsonNode, hostDocument, new ParsingContext(new()));

        var reference = Assert.IsType<OpenApiSchemaReference>(result);
        Assert.Equal("#category", reference.Reference.DynamicRef);
        Assert.True(reference.Reference.IsDynamicRefOnly);
    }

    [Fact]
    public void BareDynamicRefDoesNotEmitRefOnSerialization()
    {
        var json =
        """
        {
            "$dynamicRef": "#category"
        }
        """;

        var hostDocument = new OpenApiDocument();
        var jsonNode = JsonNode.Parse(json);

        var result = OpenApiV31Deserializer.LoadSchema(jsonNode, hostDocument, new ParsingContext(new()));

        var sw = new StringWriter();
        var writer = new OpenApiJsonWriter(sw);
        result.SerializeAsV31(writer);

        var output = sw.ToString();
        Assert.Contains("$dynamicRef", output);
        Assert.DoesNotContain("$ref", output);
    }

    [Fact]
    public async Task DynamicRefResolvesToDynamicAnchorTarget()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Tree:
              $dynamicAnchor: node
              type: object
              properties:
                value:
                  type: string
                children:
                  type: array
                  items:
                    $dynamicRef: '#node'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var tree = doc.Components.Schemas["Tree"];
        var childrenItems = tree.Properties["children"].Items;

        var reference = Assert.IsType<OpenApiSchemaReference>(childrenItems);
        Assert.True(reference.Reference.IsDynamicRefOnly);
        Assert.NotNull(reference.Target);
        Assert.Same(tree, reference.Target);
    }

    [Fact]
    public async Task DynamicRefResolvesViaDefsAnchor()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Root:
              $defs:
                node:
                  $dynamicAnchor: node
                  type: object
                  properties:
                    value:
                      type: string
                    next:
                      $dynamicRef: '#node'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var root = doc.Components.Schemas["Root"];
        var nodeDef = root.Definitions["node"];
        var nextSchema = nodeDef.Properties["next"];

        var reference = Assert.IsType<OpenApiSchemaReference>(nextSchema);
        Assert.NotNull(reference.Target);
        Assert.Same(nodeDef, reference.Target);
    }

    [Fact]
    public async Task DynamicRefResolvesViaNestedPropertyAnchor()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Tree:
              type: object
              properties:
                self:
                  $dynamicAnchor: node
                  type: object
                  properties:
                    value:
                      type: string
                    next:
                      $dynamicRef: '#node'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var tree = doc.Components.Schemas["Tree"];
        var self = tree.Properties["self"];

        var nextSchema = self.Properties["next"];
        var reference = Assert.IsType<OpenApiSchemaReference>(nextSchema);
        Assert.NotNull(reference.Target);
        Assert.Same(self, reference.Target);
    }

    [Fact]
    public async Task DynamicRefResolvesViaAllOfAnchor()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Root:
              allOf:
                - $dynamicAnchor: node
                  type: object
                  properties:
                    next:
                      $dynamicRef: '#node'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var root = doc.Components.Schemas["Root"];
        var branch = root.AllOf[0];
        var nextSchema = branch.Properties["next"];

        var reference = Assert.IsType<OpenApiSchemaReference>(nextSchema);
        Assert.NotNull(reference.Target);
        Assert.Same(branch, reference.Target);
    }

    [Fact]
    public async Task DynamicRefReturnsNullWhenAnchorIsAmbiguous()
    {
        // When a single document declares the same $dynamicAnchor name on more than one subschema,
        // resolution cannot pick one without dynamic-scope evaluation, so Target returns null.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Root:
              type: object
              properties:
                a:
                  $dynamicAnchor: node
                  type: object
                b:
                  $dynamicAnchor: node
                  type: object
                ref:
                  $dynamicRef: '#node'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var root = doc.Components.Schemas["Root"];
        var reference = Assert.IsType<OpenApiSchemaReference>(root.Properties["ref"]);
        Assert.Null(reference.Target);
    }

    [Fact]
    public async Task DynamicAnchorRegisteredAcrossAllSubschemaLocations()
    {
        // Exercises every subschema location the anchor walk descends into (oneOf, anyOf, not,
        // items, additionalProperties, patternProperties, contains, propertyNames, contentSchema,
        // if/then/else, dependentSchemas, unevaluatedPropertiesSchema). Each declares a distinct
        // $dynamicAnchor name; a $dynamicRef to each confirms the walk reached it.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Root:
              type: object
              oneOf:
                - $dynamicAnchor: one
                  type: object
              anyOf:
                - $dynamicAnchor: any
                  type: object
              allOf:
                - type: object
                  properties:
                    nested:
                      type: array
                      items:
                        $dynamicAnchor: itm
                        type: string
                  dependentSchemas:
                    dep:
                      $dynamicAnchor: depn
                      type: object
              not:
                $dynamicAnchor: notn
                type: object
              contains:
                $dynamicAnchor: cont
                type: object
              propertyNames:
                $dynamicAnchor: pn
                type: string
              contentSchema:
                $dynamicAnchor: cs
                type: string
              unevaluatedProperties:
                $dynamicAnchor: up
                type: object
              patternProperties:
                '^x':
                  $dynamicAnchor: pp
                  type: object
              properties:
                child:
                  $dynamicAnchor: ifn
                  type: object
              additionalProperties:
                $dynamicAnchor: ap
                type: object
              if:
                $dynamicAnchor: iftop
                type: object
              then:
                $dynamicAnchor: thentop
                type: object
              else:
                $dynamicAnchor: elsetop
                type: object
              $defs:
                consumer:
                  type: object
                  properties:
                    one:
                      $dynamicRef: '#one'
                    any:
                      $dynamicRef: '#any'
                    notn:
                      $dynamicRef: '#notn'
                    cont:
                      $dynamicRef: '#cont'
                    pn:
                      $dynamicRef: '#pn'
                    cs:
                      $dynamicRef: '#cs'
                    up:
                      $dynamicRef: '#up'
                    pp:
                      $dynamicRef: '#pp'
                    ifn:
                      $dynamicRef: '#ifn'
                    itm:
                      $dynamicRef: '#itm'
                    depn:
                      $dynamicRef: '#depn'
                    iftop:
                      $dynamicRef: '#iftop'
                    thentop:
                      $dynamicRef: '#thentop'
                    elsetop:
                      $dynamicRef: '#elsetop'
                    ap:
                      $dynamicRef: '#ap'
        """;

        var doc = await LoadDocumentAsync(yaml);
        var root = doc.Components.Schemas["Root"];
        var consumer = root.Definitions["consumer"].Properties;

        // Each anchor is unique within the document, so every resolution must succeed.
        foreach (var name in new[] { "one", "any", "notn", "cont", "pn", "cs", "up", "pp", "ifn", "itm", "depn", "iftop", "thentop", "elsetop", "ap" })
        {
            var reference = Assert.IsType<OpenApiSchemaReference>(consumer[name]);
            Assert.NotNull(reference.Target);
        }
    }

    [Fact]
    public async Task DynamicAnchorInRefSiblingApplicatorsIsRegistered()
    {
        // A $ref schema may carry applicator siblings (allOf/oneOf/anyOf/properties/items/...) whose
        // subschemas declare $dynamicAnchor. The anchor walk must descend into a reference holder's
        // own siblings (read from JsonSchemaReference) to register them.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Base:
              type: object
            Referencing:
              $ref: '#/components/schemas/Base'
              oneOf:
                - $dynamicAnchor: refOne
                  type: object
              anyOf:
                - $dynamicAnchor: refAny
                  type: object
              properties:
                child:
                  $dynamicAnchor: refChild
                  type: object
              patternProperties:
                '^x':
                  $dynamicAnchor: refPP
                  type: object
              items:
                $dynamicAnchor: refItem
                type: string
              dependentSchemas:
                dep:
                  $dynamicAnchor: refDep
                  type: object
              $defs:
                consumer:
                  type: object
                  properties:
                    a:
                      $dynamicRef: '#refOne'
                    b:
                      $dynamicRef: '#refAny'
                    c:
                      $dynamicRef: '#refChild'
                    d:
                      $dynamicRef: '#refItem'
                    e:
                      $dynamicRef: '#refPP'
                    f:
                      $dynamicRef: '#refDep'
        """;

        var doc = await LoadDocumentAsync(yaml);
        var referencing = doc.Components.Schemas["Referencing"];
        var consumer = referencing.Definitions["consumer"].Properties;

        // Each $dynamicRef targets an anchor declared in a sibling applicator of the $ref schema.
        foreach (var reference in consumer.Values.Select(v => Assert.IsType<OpenApiSchemaReference>(v)))
        {
            Assert.NotNull(reference.Target);
        }
    }

    [Fact]
    public async Task DynamicRefReturnsNullForUnknownAnchor()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Foo:
              type: object
              properties:
                bar:
                  $dynamicRef: '#nonexistent'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var foo = doc.Components.Schemas["Foo"];
        var barSchema = foo.Properties["bar"];

        var reference = Assert.IsType<OpenApiSchemaReference>(barSchema);
        Assert.True(reference.Reference.IsDynamicRefOnly);
        Assert.Null(reference.Target);
    }

    [Fact]
    public async Task DynamicRefRoundTripsThroughSerialization()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Tree:
              $dynamicAnchor: node
              type: object
              properties:
                children:
                  type: array
                  items:
                    $dynamicRef: '#node'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var sw = new StringWriter();
        var writer = new OpenApiYamlWriter(sw);
        doc.SerializeAsV31(writer);
        var serialized = sw.ToString();

        var doc2 = await LoadDocumentAsync(serialized);

        var tree2 = doc2.Components.Schemas["Tree"];
        var childrenItems2 = tree2.Properties["children"].Items;
        var reference2 = Assert.IsType<OpenApiSchemaReference>(childrenItems2);
        Assert.True(reference2.Reference.IsDynamicRefOnly);
        Assert.NotNull(reference2.Target);
    }

    [Fact]
    public async Task ExistingRefWithPathStillWorks()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Foo:
              type: string
            Bar:
              $ref: '#/components/schemas/Foo'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var bar = doc.Components.Schemas["Bar"];
        var reference = Assert.IsType<OpenApiSchemaReference>(bar);
        Assert.False(reference.Reference.IsDynamicRefOnly);
        Assert.NotNull(reference.Target);
        Assert.Same(doc.Components.Schemas["Foo"], reference.Target);
    }

    [Fact]
    public async Task DynamicRefWithSiblingsPreservesSiblings()
    {
        // A $dynamicRef alongside structural schema keywords must not drop the siblings. The object
        // is parsed as an OpenApiSchemaReference with siblings preserved via ApplySchemaMetadata,
        // surfaced through the Reference-first property getters.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Foo:
              type: object
              properties:
                bar:
                  $dynamicRef: '#node'
                  maxProperties: 3
                  description: a constrained dynamic ref
        """;

        var doc = await LoadDocumentAsync(yaml);

        var foo = doc.Components.Schemas["Foo"];
        var bar = foo.Properties["bar"];

        // Siblings preserved
        Assert.Equal(3, bar.MaxProperties);
        Assert.Equal("a constrained dynamic ref", bar.Description);
        Assert.Equal("#node", bar.DynamicRef);
    }

    [Fact]
    public async Task AbsoluteDynamicRefResolvesToLocalAnchorWhenExternalNotLoaded()
    {
        // A URI-based $dynamicRef (https://example.com/external#node) targets an external resource.
        // When the external document is not loaded in the workspace, resolution returns null
        // rather than falling back to a local same-named anchor.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Tree:
              $dynamicAnchor: node
              type: object
              properties:
                next:
                  $dynamicRef: 'https://example.com/external#node'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var tree = doc.Components.Schemas["Tree"];
        var next = tree.Properties["next"];
        var reference = Assert.IsType<OpenApiSchemaReference>(next);
        Assert.True(reference.Reference.IsDynamicRefOnly);
        Assert.Null(reference.Target);
    }

    [Fact]
    public async Task AbsoluteDynamicRefResolvesAcrossDocuments()
    {
        // Document A references an anchor in Document B via a URI-based $dynamicRef.
        // Both documents are in the same workspace.
        var yamlA =
        """
        openapi: 3.1.0
        info:
          title: Doc A
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Tree:
              type: object
              properties:
                next:
                  $dynamicRef: 'https://example.com/external#node'
        """;

        var yamlB =
        """
        openapi: 3.1.0
        info:
          title: Doc B
          version: 1.0.0
        paths: {}
        components:
          schemas:
            ExternalNode:
              $dynamicAnchor: node
              type: object
              properties:
                value:
                  type: string
        """;

        using var streamA = new MemoryStream(Encoding.UTF8.GetBytes(yamlA));
        using var streamB = new MemoryStream(Encoding.UTF8.GetBytes(yamlB));
        var resultA = await OpenApiDocument.LoadAsync(streamA, "yaml", SettingsFixture.ReaderSettings);
        var resultB = await OpenApiDocument.LoadAsync(streamB, "yaml", SettingsFixture.ReaderSettings);

        var docA = resultA.Document;
        var docB = resultB.Document;

        docA.BaseUri = new("https://example.com/main");
        docB.BaseUri = new("https://example.com/external");

        var workspace = docA.Workspace ?? new OpenApiWorkspace();
        workspace.RegisterComponents(docA);
        workspace.RegisterComponents(docB);

        var tree = docA.Components.Schemas["Tree"];
        var next = tree.Properties["next"];
        var reference = Assert.IsType<OpenApiSchemaReference>(next);
        Assert.True(reference.Reference.IsDynamicRefOnly);
        Assert.NotNull(reference.Target);
        Assert.Same(docB.Components.Schemas["ExternalNode"], reference.Target);
    }

    [Fact]
    public async Task DynamicAnchorResolvesPerDocumentInSharedWorkspace()
    {
        // Two documents in the same workspace each declare $dynamicAnchor: node (the conventional
        // name for recursive tree schemas). Per JSON Schema 2020-12, dynamic scope is per-document,
        // so each $dynamicRef must resolve to its own document's anchor, not return null as ambiguous.
        var yaml = """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Tree:
              $dynamicAnchor: node
              type: object
              properties:
                next:
                  $dynamicRef: '#node'
        """;

        var docA = await LoadDocumentAsync(yaml);
        var docB = await LoadDocumentAsync(yaml);

        var workspace = new OpenApiWorkspace();
        docA.Workspace = workspace;
        docB.Workspace = workspace;
        workspace.RegisterComponents(docA);
        workspace.RegisterComponents(docB);

        var treeA = docA.Components.Schemas["Tree"];
        var treeB = docB.Components.Schemas["Tree"];

        var refA = Assert.IsType<OpenApiSchemaReference>(treeA.Properties["next"]);
        Assert.NotNull(refA.Target);
        Assert.Same(treeA, refA.Target);

        var refB = Assert.IsType<OpenApiSchemaReference>(treeB.Properties["next"]);
        Assert.NotNull(refB.Target);
        Assert.Same(treeB, refB.Target);
    }

    [Fact]
    public async Task DynamicAnchorInRefSiblingDefsIsRegistered()
    {
        // A $dynamicAnchor declared inside a $ref schema's $defs sibling must be registered, so a
        // $dynamicRef elsewhere in the document resolves to it. (Main's model carries authored
        // siblings on JsonSchemaReference, so the anchor walk must descend into them without
        // following the $ref target.)
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Base:
              type: object
            Referencing:
              $ref: '#/components/schemas/Base'
              $defs:
                node:
                  $dynamicAnchor: node
                  type: object
                  properties:
                    next:
                      $dynamicRef: '#node'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var referencing = doc.Components.Schemas["Referencing"];
        var nodeDef = referencing.Definitions["node"];
        var next = nodeDef.Properties["next"];

        var reference = Assert.IsType<OpenApiSchemaReference>(next);
        Assert.NotNull(reference.Target);
        Assert.Same(nodeDef, reference.Target);
    }

    [Fact]
    public async Task CreateShallowCopyPreservesValidationSiblings()
    {
        // CreateShallowCopy routes through the JsonSchemaReference copy constructor. Validation
        // keyword siblings carried on a $ref schema (type, minProperties, pattern, allOf, etc.)
        // must survive the copy, not just the JSON-Schema metadata siblings.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Target:
              type: object
            Referencing:
              $ref: '#/components/schemas/Target'
              type: object
              minProperties: 2
              pattern: '^a'
              allOf:
                - type: object
        """;

        var doc = await LoadDocumentAsync(yaml);

        var referencing = doc.Components.Schemas["Referencing"];
        var copy = referencing.CreateShallowCopy();

        Assert.IsType<OpenApiSchemaReference>(copy);
        Assert.Equal(JsonSchemaType.Object, copy.Type);
        Assert.Equal(2, copy.MinProperties);
        Assert.Equal("^a", copy.Pattern);
        Assert.NotNull(copy.AllOf);
        Assert.Single(copy.AllOf);
    }

    [Fact]
    public async Task InlineDynamicAnchorInResponseIsRegistered()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths:
          /endpoint-a:
            get:
              responses:
                '200':
                  description: A
                  content:
                    application/json:
                      schema:
                        $defs:
                          itemType:
                            $dynamicAnchor: itemType
                            $ref: '#/components/schemas/TypeA'
                        $ref: '#/components/schemas/Paged'
          /endpoint-b:
            get:
              responses:
                '200':
                  description: B
                  content:
                    application/json:
                      schema:
                        $defs:
                          itemType:
                            $dynamicAnchor: itemType
                            $ref: '#/components/schemas/TypeB'
                        $ref: '#/components/schemas/Paged'
        components:
          schemas:
            Paged:
              type: object
              properties:
                content:
                  type: array
                  items:
                    $dynamicRef: '#itemType'
            TypeA:
              type: object
              properties:
                name:
                  type: string
            TypeB:
              type: object
              properties:
                title:
                  type: string
        """;

        var doc = await LoadDocumentAsync(yaml);

        var candidates = doc.Workspace.GetDynamicAnchorCandidates(doc, "itemType");
        Assert.Equal(2, candidates.Count);

        var paged = doc.Components.Schemas["Paged"];
        var itemsSchema = paged.Properties["content"].Items;
        var itemsRef = Assert.IsType<OpenApiSchemaReference>(itemsSchema);
        Assert.True(itemsRef.Reference.IsDynamicRefOnly);
        Assert.Null(itemsRef.Target);

        var endpointASchema = doc.Paths["/endpoint-a"].Operations[HttpMethod.Get]
            .Responses["200"].Content["application/json"].Schema;
        var resolvedA = OpenApiWorkspace.ResolveDynamicAnchorInContext(endpointASchema, "itemType");
        Assert.NotNull(resolvedA);
        var resolvedARef = Assert.IsType<OpenApiSchemaReference>(resolvedA);
        Assert.NotNull(resolvedARef.Target);
        Assert.Same(doc.Components.Schemas["TypeA"], resolvedARef.Target);

        var endpointBSchema = doc.Paths["/endpoint-b"].Operations[HttpMethod.Get]
            .Responses["200"].Content["application/json"].Schema;
        var resolvedB = OpenApiWorkspace.ResolveDynamicAnchorInContext(endpointBSchema, "itemType");
        Assert.NotNull(resolvedB);
        var resolvedBRef = Assert.IsType<OpenApiSchemaReference>(resolvedB);
        Assert.NotNull(resolvedBRef.Target);
        Assert.Same(doc.Components.Schemas["TypeB"], resolvedBRef.Target);
    }

    [Fact]
    public async Task DynamicAnchorInReusableResponseIsRegistered()
    {
        // A $dynamicAnchor declared inside a schema nested under components/responses must be
        // registered just like one declared inline under paths, so a $dynamicRef resolves to it.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Paged:
              type: object
              properties:
                content:
                  type: array
                  items:
                    $dynamicRef: '#itemType'
            ItemType:
              type: object
              properties:
                name:
                  type: string
          responses:
            ListResponse:
              description: A paged list
              content:
                application/json:
                  schema:
                    $defs:
                      itemType:
                        $dynamicAnchor: itemType
                        $ref: '#/components/schemas/ItemType'
                    $ref: '#/components/schemas/Paged'
        """;

        var doc = await LoadDocumentAsync(yaml);

        // The anchor is reachable only via components/responses; before the fix it was never indexed.
        Assert.Single(doc.Workspace.GetDynamicAnchorCandidates(doc, "itemType"));

        var paged = doc.Components.Schemas["Paged"];
        var itemsRef = Assert.IsType<OpenApiSchemaReference>(paged.Properties["content"].Items);
        Assert.True(itemsRef.Reference.IsDynamicRefOnly);
        Assert.NotNull(itemsRef.Target);

        var resolved = Assert.IsType<OpenApiSchemaReference>(itemsRef.Target);
        Assert.Same(doc.Components.Schemas["ItemType"], resolved.Target);
    }

    [Fact]
    public async Task DynamicAnchorInReusableParameterIsRegistered()
    {
        // A $dynamicAnchor declared inside a reusable parameter's schema must be registered.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Filter:
              type: object
              properties:
                child:
                  $dynamicRef: '#node'
            Node:
              type: object
              properties:
                name:
                  type: string
          parameters:
            filterParam:
              name: filter
              in: query
              schema:
                $defs:
                  node:
                    $dynamicAnchor: node
                    $ref: '#/components/schemas/Node'
                $ref: '#/components/schemas/Filter'
        """;

        var doc = await LoadDocumentAsync(yaml);

        Assert.Single(doc.Workspace.GetDynamicAnchorCandidates(doc, "node"));

        var filter = doc.Components.Schemas["Filter"];
        var childRef = Assert.IsType<OpenApiSchemaReference>(filter.Properties["child"]);
        Assert.True(childRef.Reference.IsDynamicRefOnly);
        Assert.NotNull(childRef.Target);

        var resolved = Assert.IsType<OpenApiSchemaReference>(childRef.Target);
        Assert.Same(doc.Components.Schemas["Node"], resolved.Target);
    }

    [Fact]
    public async Task DynamicAnchorInReusableHeaderIsRegistered()
    {
        // A $dynamicAnchor declared inside a reusable header's schema (components/headers) must
        // be registered.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Cursor:
              type: object
              properties:
                next:
                  $dynamicRef: '#page'
            Page:
              type: object
              properties:
                index:
                  type: integer
          headers:
            XCursor:
              description: Cursor header
              schema:
                $defs:
                  page:
                    $dynamicAnchor: page
                    $ref: '#/components/schemas/Page'
                $ref: '#/components/schemas/Cursor'
        """;

        var doc = await LoadDocumentAsync(yaml);

        Assert.Single(doc.Workspace.GetDynamicAnchorCandidates(doc, "page"));

        var cursor = doc.Components.Schemas["Cursor"];
        var nextRef = Assert.IsType<OpenApiSchemaReference>(cursor.Properties["next"]);
        Assert.True(nextRef.Reference.IsDynamicRefOnly);
        Assert.NotNull(nextRef.Target);

        var resolved = Assert.IsType<OpenApiSchemaReference>(nextRef.Target);
        Assert.Same(doc.Components.Schemas["Page"], resolved.Target);
    }

    [Fact]
    public async Task DynamicAnchorInResponseHeaderSchemaIsRegistered()
    {
        // A $dynamicAnchor declared inside a response header's schema must be registered. This
        // exercises the response.Headers leaf, which (like parameter.Content, header.Content and
        // mediaType.ItemSchema) is a schema-bearing location that the anchor walk must cover.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths:
          /items:
            get:
              responses:
                '200':
                  description: OK
                  headers:
                    X-Page:
                      description: Paging header
                      schema:
                        $defs:
                          meta:
                            $dynamicAnchor: meta
                            type: object
                            properties:
                              total:
                                type: integer
        components:
          schemas:
            Body:
              type: object
              properties:
                paging:
                  $dynamicRef: '#meta'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var candidate = Assert.Single(doc.Workspace.GetDynamicAnchorCandidates(doc, "meta"));

        var body = doc.Components.Schemas["Body"];
        var pagingRef = Assert.IsType<OpenApiSchemaReference>(body.Properties["paging"]);
        Assert.True(pagingRef.Reference.IsDynamicRefOnly);
        Assert.NotNull(pagingRef.Target);
        Assert.Same(candidate, pagingRef.Target);
    }

    [Fact]
    public async Task ResolveDynamicAnchorInContextReturnsNullForNoMatch()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Foo:
              type: string
        """;

        var doc = await LoadDocumentAsync(yaml);
        var foo = doc.Components.Schemas["Foo"];

        var result = OpenApiWorkspace.ResolveDynamicAnchorInContext(foo, "nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveDynamicAnchorInContextDoesNotReadFromTargetForReferenceHolders()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Target:
              $defs:
                node:
                  $dynamicAnchor: node
                  type: object
                  properties:
                    name:
                      type: string
            Referrer:
              $ref: '#/components/schemas/Target'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var referrer = doc.Components.Schemas["Referrer"];
        var refSchema = Assert.IsType<OpenApiSchemaReference>(referrer);

        var result = OpenApiWorkspace.ResolveDynamicAnchorInContext(refSchema, "node");
        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveDynamicAnchorInContextDoesNotReadFromTargetForReferenceDefsEntries()
    {
        // A $defs entry that is itself a $ref must contribute its own authored $dynamicAnchor,
        // not an anchor declared on the referenced target. The "aliased" def points at a target
        // that declares $dynamicAnchor: node, but the def itself does not author it, so the
        // context-bound lookup must return null.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Target:
              $dynamicAnchor: node
              type: object
              properties:
                name:
                  type: string
            Container:
              type: object
              $defs:
                aliased:
                  $ref: '#/components/schemas/Target'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var container = doc.Components.Schemas["Container"];
        var aliased = Assert.IsType<OpenApiSchemaReference>(container.Definitions!["aliased"]);
        // Sanity: the fall-through property would surface the target's anchor...
        Assert.Equal("node", aliased.DynamicAnchor);
        // ...but the authored sibling on the def entry is empty.
        Assert.Null(aliased.Reference.DynamicAnchor);

        // Both the non-reference context branch (Container) and the reference-holder branch
        // (a ref whose Reference.Definitions carries the entry) must ignore the target's anchor.
        Assert.Null(OpenApiWorkspace.ResolveDynamicAnchorInContext(container, "node"));

        var refWithDefs = new OpenApiSchemaReference("Container", doc)
        {
            Reference =
            {
                Definitions = new Dictionary<string, IOpenApiSchema>
                {
                    ["aliased"] = aliased
                }
            }
        };
        Assert.Null(OpenApiWorkspace.ResolveDynamicAnchorInContext(refWithDefs, "node"));
    }

    [Fact]
    public async Task DynamicRefFallsBackToPlainAnchor()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Target:
              $anchor: meta
              type: object
              properties:
                value:
                  type: string
            Referencer:
              type: object
              properties:
                ref:
                  $dynamicRef: '#meta'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var target = doc.Components.Schemas["Target"];
        var referencer = doc.Components.Schemas["Referencer"];
        var refSchema = referencer.Properties["ref"];

        var reference = Assert.IsType<OpenApiSchemaReference>(refSchema);
        Assert.True(reference.Reference.IsDynamicRefOnly);
        Assert.NotNull(reference.Target);
        Assert.Same(target, reference.Target);
    }

    [Fact]
    public async Task DynamicAnchorTakesPrecedenceOverPlainAnchor()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            DynamicTarget:
              $dynamicAnchor: meta
              type: object
              properties:
                dynamic:
                  type: boolean
            PlainTarget:
              $anchor: meta
              type: object
              properties:
                plain:
                  type: boolean
            Referencer:
              type: object
              properties:
                ref:
                  $dynamicRef: '#meta'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var dynamicTarget = doc.Components.Schemas["DynamicTarget"];
        var referencer = doc.Components.Schemas["Referencer"];
        var refSchema = referencer.Properties["ref"];

        var reference = Assert.IsType<OpenApiSchemaReference>(refSchema);
        Assert.NotNull(reference.Target);
        Assert.Same(dynamicTarget, reference.Target);
    }

    [Fact]
    public async Task DynamicRefDoesNotFallBackToPlainAnchorWhenDynamicAnchorIsAmbiguous()
    {
        // Two $dynamicAnchor: meta declarations make the dynamic anchor ambiguous. Per §8.2.3.2,
        // the spec requires the outermost $dynamicAnchor, which this library cannot compute, so
        // Target must be null. The presence of a $anchor: meta must NOT trigger the plain-anchor
        // fallback, since that would silently resolve to a different, incorrect target.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            DynamicA:
              $dynamicAnchor: meta
              type: object
              properties:
                a:
                  type: boolean
            DynamicB:
              $dynamicAnchor: meta
              type: object
              properties:
                b:
                  type: boolean
            PlainTarget:
              $anchor: meta
              type: object
              properties:
                plain:
                  type: boolean
            Referencer:
              type: object
              properties:
                ref:
                  $dynamicRef: '#meta'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var referencer = doc.Components.Schemas["Referencer"];
        var refSchema = referencer.Properties["ref"];

        var reference = Assert.IsType<OpenApiSchemaReference>(refSchema);
        Assert.True(reference.Reference.IsDynamicRefOnly);
        Assert.Null(reference.Target);
    }

    [Fact]
    public async Task DynamicAnchorInInlineCallbackIsRegistered()
    {
        // A $dynamicAnchor declared inside a schema nested in an inline operation callback must be
        // registered, matching the treatment of reusable (components/callbacks) callbacks.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths:
          /register:
            post:
              requestBody:
                content:
                  application/json:
                    schema:
                      type: object
              responses:
                '200':
                  description: OK
              callbacks:
                onUpdate:
                  '$request.body#/url':
                    post:
                      requestBody:
                        content:
                          application/json:
                            schema:
                              $defs:
                                node:
                                  $dynamicAnchor: node
                                  $ref: '#/components/schemas/Node'
                              $ref: '#/components/schemas/Event'
                      responses:
                        '200':
                          description: OK
        components:
          schemas:
            Event:
              type: object
              properties:
                child:
                  $dynamicRef: '#node'
            Node:
              type: object
              properties:
                name:
                  type: string
        """;

        var doc = await LoadDocumentAsync(yaml);

        Assert.Single(doc.Workspace.GetDynamicAnchorCandidates(doc, "node"));

        var evt = doc.Components.Schemas["Event"];
        var childRef = Assert.IsType<OpenApiSchemaReference>(evt.Properties["child"]);
        Assert.True(childRef.Reference.IsDynamicRefOnly);
        Assert.NotNull(childRef.Target);

        var resolved = Assert.IsType<OpenApiSchemaReference>(childRef.Target);
        Assert.Same(doc.Components.Schemas["Node"], resolved.Target);
    }

    [Fact]
    public async Task CyclicCallbackDoesNotStackOverflowAndStillRegistersAnchors()
    {
        // A callback that references itself (via $ref) creates a structural cycle
        // pathItem -> operation -> callback -> pathItem. The anchor walk must terminate and still
        // register anchors declared inside the cycle.
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Event:
              type: object
              properties:
                child:
                  $dynamicRef: '#node'
            Node:
              type: object
              properties:
                name:
                  type: string
          callbacks:
            Loop:
              '$request.body#/url':
                post:
                  requestBody:
                    content:
                      application/json:
                        schema:
                          $defs:
                            node:
                              $dynamicAnchor: node
                              $ref: '#/components/schemas/Node'
                          $ref: '#/components/schemas/Event'
                  responses:
                    '200':
                      description: OK
                  callbacks:
                    self:
                      $ref: '#/components/callbacks/Loop'
        """;

        var doc = await LoadDocumentAsync(yaml);

        Assert.Single(doc.Workspace.GetDynamicAnchorCandidates(doc, "node"));

        var evt = doc.Components.Schemas["Event"];
        var childRef = Assert.IsType<OpenApiSchemaReference>(evt.Properties["child"]);
        Assert.True(childRef.Reference.IsDynamicRefOnly);
        Assert.NotNull(childRef.Target);

        var resolved = Assert.IsType<OpenApiSchemaReference>(childRef.Target);
        Assert.Same(doc.Components.Schemas["Node"], resolved.Target);
    }

    [Fact]
    public void DynamicRefResolvesInObjectModelBuiltDocument()
    {
        var doc = new OpenApiDocument();
        var tree = new OpenApiSchema
        {
            DynamicAnchor = "node",
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["value"] = new OpenApiSchema { Type = JsonSchemaType.String },
                ["next"] = new OpenApiSchemaReference("node", doc)
                {
                    Reference = { DynamicRef = "#node" }
                }
            }
        };
        doc.AddComponent("Tree", tree);
        doc.Workspace = new OpenApiWorkspace();
        doc.Workspace.RegisterComponents(doc);

        var nextRef = Assert.IsType<OpenApiSchemaReference>(tree.Properties["next"]);
        nextRef.Reference.SetJsonPointerPath("#node", "#/components/schemas/Tree/properties/next");
        Assert.True(nextRef.Reference.IsDynamicRefOnly);
        Assert.NotNull(nextRef.Target);
        Assert.Same(tree, nextRef.Target);
    }

    [Fact]
    public async Task ExternalDynamicRefResolvesToPlainAnchorInExternalDocument()
    {
        // Document B has only $anchor (no $dynamicAnchor). FindDocumentByBaseUri must search
        // both registries so the $anchor fallback works for URI-based $dynamicRef.
        var yamlA =
        """
        openapi: 3.1.0
        info:
          title: Doc A
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Tree:
              type: object
              properties:
                next:
                  $dynamicRef: 'https://example.com/external#node'
        """;

        var yamlB =
        """
        openapi: 3.1.0
        info:
          title: Doc B
          version: 1.0.0
        paths: {}
        components:
          schemas:
            ExternalNode:
              $anchor: node
              type: object
              properties:
                value:
                  type: string
        """;

        using var streamA = new MemoryStream(Encoding.UTF8.GetBytes(yamlA));
        using var streamB = new MemoryStream(Encoding.UTF8.GetBytes(yamlB));
        var resultA = await OpenApiDocument.LoadAsync(streamA, "yaml", SettingsFixture.ReaderSettings);
        var resultB = await OpenApiDocument.LoadAsync(streamB, "yaml", SettingsFixture.ReaderSettings);

        var docA = resultA.Document;
        var docB = resultB.Document;

        docA.BaseUri = new("https://example.com/main");
        docB.BaseUri = new("https://example.com/external");

        var workspace = docA.Workspace ?? new OpenApiWorkspace();
        workspace.RegisterComponents(docA);
        workspace.RegisterComponents(docB);

        var tree = docA.Components.Schemas["Tree"];
        var next = tree.Properties["next"];
        var reference = Assert.IsType<OpenApiSchemaReference>(next);
        Assert.True(reference.Reference.IsDynamicRefOnly);
        Assert.NotNull(reference.Target);
        Assert.Same(docB.Components.Schemas["ExternalNode"], reference.Target);
    }

    [Fact]
    public async Task DynamicRefWithStructuralSiblingsRoundTripsThroughSerialization()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Tree:
              $dynamicAnchor: node
              type: object
              properties:
                children:
                  type: array
                  items:
                    $dynamicRef: '#node'
                    minProperties: 2
                    description: constrained child
        """;

        var doc = await LoadDocumentAsync(yaml);

        var sw = new StringWriter();
        var writer = new OpenApiYamlWriter(sw);
        doc.SerializeAsV31(writer);
        var serialized = sw.ToString();

        var doc2 = await LoadDocumentAsync(serialized);

        var tree2 = doc2.Components.Schemas["Tree"];
        var items2 = tree2.Properties["children"].Items;
        var ref2 = Assert.IsType<OpenApiSchemaReference>(items2);
        Assert.True(ref2.Reference.IsDynamicRefOnly);
        Assert.NotNull(ref2.Target);
        Assert.Same(tree2, ref2.Target);
        Assert.Equal(2, ref2.MinProperties);
        Assert.Equal("constrained child", ref2.Description);
    }

    [Fact]
    public async Task DynamicAnchorInReusableCallbackIsRegistered()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Target:
              type: object
              properties:
                value:
                  type: string
            Paged:
              type: object
              properties:
                content:
                  type: array
                  items:
                    $dynamicRef: '#itemType'
          callbacks:
            Webhook:
              '{$request.body#/url}':
                post:
                  requestBody:
                    content:
                      application/json:
                        schema:
                          $defs:
                            itemType:
                              $dynamicAnchor: itemType
                              $ref: '#/components/schemas/Target'
                          $ref: '#/components/schemas/Paged'
                  responses:
                    '200':
                      description: ok
        """;

        var doc = await LoadDocumentAsync(yaml);

        Assert.Single(doc.Workspace.GetDynamicAnchorCandidates(doc, "itemType"));
        var paged = doc.Components.Schemas["Paged"];
        var itemsRef = Assert.IsType<OpenApiSchemaReference>(paged.Properties["content"].Items);
        Assert.NotNull(itemsRef.Target);
    }

    [Fact]
    public async Task DynamicAnchorInReusablePathItemIsRegistered()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Target:
              type: object
              properties:
                value:
                  type: string
            Paged:
              type: object
              properties:
                content:
                  type: array
                  items:
                    $dynamicRef: '#itemType'
          pathItems:
            SharedPath:
              get:
                responses:
                  '200':
                    description: ok
                    content:
                      application/json:
                        schema:
                          $defs:
                            itemType:
                              $dynamicAnchor: itemType
                              $ref: '#/components/schemas/Target'
                          $ref: '#/components/schemas/Paged'
        """;

        var doc = await LoadDocumentAsync(yaml);

        Assert.Single(doc.Workspace.GetDynamicAnchorCandidates(doc, "itemType"));
        var paged = doc.Components.Schemas["Paged"];
        var itemsRef = Assert.IsType<OpenApiSchemaReference>(paged.Properties["content"].Items);
        Assert.NotNull(itemsRef.Target);
    }

    [Fact]
    public async Task InliningDynamicRefProducesResolvedTargetSchema()
    {
        var yaml =
        """
        openapi: 3.1.0
        info:
          title: Test
          version: 1.0.0
        paths: {}
        components:
          schemas:
            Tree:
              $dynamicAnchor: node
              type: object
              properties:
                value:
                  type: string
                next:
                  $dynamicRef: '#node'
        """;

        var doc = await LoadDocumentAsync(yaml);

        var tree = doc.Components.Schemas["Tree"];
        var next = tree.Properties["next"];
        var reference = Assert.IsType<OpenApiSchemaReference>(next);

        var inlined = reference.CopyReferenceAsTargetElementWithOverrides(next);
        Assert.NotNull(inlined);
        Assert.Equal(JsonSchemaType.Object, inlined.Type);
        Assert.NotNull(inlined.Properties);
        Assert.True(inlined.Properties.ContainsKey("value"));
    }
}
