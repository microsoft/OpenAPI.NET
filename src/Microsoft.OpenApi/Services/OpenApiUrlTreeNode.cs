// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// A directory structure representing the paths of an OpenAPI document.
    /// </summary>
    public class OpenApiUrlTreeNode
    {
        private const string RootPathSegment = "/";
        private const string PathSeparator = "\\";

        /// <summary>
        /// All the subdirectories of a node.
        /// </summary>
        public IDictionary<string, OpenApiUrlTreeNode> Children { get; } = new Dictionary<string, OpenApiUrlTreeNode>();

        /// <summary>
        /// The relative directory path of the current node from the root node.
        /// </summary>
        public string Path { get; set; } = "";

        /// <summary>
        /// Dictionary of labels and Path Item objects that describe the operations available on a node.
        /// </summary>
        public IDictionary<string, OpenApiPathItem> PathItems { get; } = new Dictionary<string, OpenApiPathItem>();

        /// <summary>
        /// A dictionary of key value pairs that contain information about a node.
        /// </summary>
        public IDictionary<string, List<string>> AdditionalData { get; set; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// Flag indicating whether a node segment is a path parameter.
        /// </summary>
        public bool IsParameter => Segment.StartsWith("{");

        /// <summary>
        /// The subdirectory of a relative path.
        /// </summary>
        public string Segment { get; private set; }

        /// <summary>
        /// Flag indicating whether the node's PathItems dictionary has operations
        /// under a given label.
        /// </summary>
        /// <param name="label">The name of the key for the target operations
        /// in the node's PathItems dictionary.</param>
        /// <returns>true or false.</returns>
        public bool HasOperations(string label)
        {
            Utils.CheckArgumentNullOrEmpty(label);

            return PathItems is not null && PathItems.TryGetValue(label, out var item) && item.Operations is not null && item.Operations.Any();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="segment">The subdirectory of a relative path.</param>
        private OpenApiUrlTreeNode(string segment)
        {
            Segment = segment;
        }

        /// <summary>
        /// Creates an empty structured directory of <see cref="OpenApiUrlTreeNode"/> node.
        /// </summary>
        /// <returns>The root node of the created <see cref="OpenApiUrlTreeNode"/> directory structure.</returns>
        public static OpenApiUrlTreeNode Create()
        {
            return new(RootPathSegment);
        }

        /// <summary>
        /// Creates a structured directory of <see cref="OpenApiUrlTreeNode"/> nodes from the paths of an OpenAPI document.
        /// </summary>
        /// <param name="doc">The OpenAPI document.</param>
        /// <param name="label">Name tag for labelling the <see cref="OpenApiUrlTreeNode"/> nodes in the directory structure.</param>
        /// <returns>The root node of the created <see cref="OpenApiUrlTreeNode"/> directory structure.</returns>
        public static OpenApiUrlTreeNode Create(OpenApiDocument doc, string label)
        {
            Utils.CheckArgumentNull(doc);
            Utils.CheckArgumentNullOrEmpty(label);

            var root = Create();

            var paths = doc.Paths;
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    root.Attach(path: path.Key,
                                pathItem: path.Value,
                                label: label);
                }
            }

            return root;
        }

        /// <summary>
        /// Retrieves the paths from an OpenAPI document and appends the items to an <see cref="OpenApiUrlTreeNode"/> node.
        /// </summary>
        /// <param name="doc">The OpenAPI document.</param>
        /// <param name="label">Name tag for labelling related <see cref="OpenApiUrlTreeNode"/> nodes in the directory structure.</param>
        public void Attach(OpenApiDocument doc, string label)
        {
            Utils.CheckArgumentNull(doc);
            Utils.CheckArgumentNullOrEmpty(label);

            var paths = doc.Paths;
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    Attach(path: path.Key,
                           pathItem: path.Value,
                           label: label);
                }
            }
        }

        /// <summary>
        /// Appends a path and the PathItem to an <see cref="OpenApiUrlTreeNode"/> node.
        /// </summary>
        /// <param name="path">An OpenAPI path.</param>
        /// <param name="pathItem">Path Item object that describes the operations available on an OpenAPI path.</param>
        /// <param name="label">A name tag for labelling the <see cref="OpenApiUrlTreeNode"/> node.</param>
        /// <returns>An <see cref="OpenApiUrlTreeNode"/> node describing an OpenAPI path.</returns>
        public OpenApiUrlTreeNode Attach(string path,
                                         OpenApiPathItem pathItem,
                                         string label)
        {
            Utils.CheckArgumentNullOrEmpty(label);
            Utils.CheckArgumentNullOrEmpty(path);
            Utils.CheckArgumentNull(pathItem);

            if (path.StartsWith(RootPathSegment))
            {
                // Remove leading slash
                path = path.Substring(1);
            }

            var segments = path.Split('/');
            if (path.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                // Remove the last element, which is empty, and append the trailing slash to the new last element
                // This is to support URLs with trailing slashes
                Array.Resize(ref segments, segments.Length - 1);
                segments[segments.Length - 1] += @"\";
            }

            return Attach(segments: segments,
                          pathItem: pathItem,
                          label: label,
                          currentPath: "");
        }

        /// <summary>
        /// Assembles the constituent properties of an <see cref="OpenApiUrlTreeNode"/> node.
        /// </summary>
        /// <param name="segments">IEnumerable subdirectories of a relative path.</param>
        /// <param name="pathItem">Path Item object that describes the operations available on an OpenAPI path.</param>
        /// <param name="label">A name tag for labelling the <see cref="OpenApiUrlTreeNode"/> node.</param>
        /// <param name="currentPath">The relative path of a node.</param>
        /// <returns>An <see cref="OpenApiUrlTreeNode"/> node with all constituent properties assembled.</returns>
        private OpenApiUrlTreeNode Attach(IEnumerable<string> segments,
                                          OpenApiPathItem pathItem,
                                          string label,
                                          string currentPath)
        {
            var segment = segments.FirstOrDefault();
            if (string.IsNullOrEmpty(segment))
            {
                if (PathItems.ContainsKey(label))
                {
                    throw new ArgumentException($"A duplicate label already exists for this node: {label}", nameof(label));
                }

                Path = currentPath;
                PathItems.Add(label, pathItem);
                return this;
            }

            // If the child segment has already been defined, then insert into it
            if (Children.TryGetValue(segment, out var child))
            {
                var newPath = currentPath + PathSeparator + segment;

                return child.Attach(
                    segments: segments.Skip(1),
                    pathItem: pathItem,
                    label: label,
                    currentPath: newPath);
            }
            else
            {
                var newPath = currentPath + PathSeparator + segment;

                var node = new OpenApiUrlTreeNode(segment)
                {
                    Path = newPath
                };

                Children[segment] = node;

                return node.Attach(segments: segments.Skip(1),
                                   pathItem: pathItem,
                                   label: label,
                                   currentPath: newPath);
            }
        }

        /// <summary>
        /// Adds additional data information to the AdditionalData property of the node.
        /// </summary>
        /// <param name="additionalData">A dictionary of key value pairs that contain information about a node.</param>
        public void AddAdditionalData(Dictionary<string, List<string>> additionalData)
        {
            Utils.CheckArgumentNull(additionalData);

            foreach (var item in additionalData)
            {
                AdditionalData[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// Write tree as Mermaid syntax
        /// </summary>
        /// <param name="writer">StreamWriter to write the Mermaid content to</param>
        public void WriteMermaid(TextWriter writer)
        {
            writer.WriteLine("graph LR");
            foreach (var style in MermaidNodeStyles)
            {
                writer.WriteLine($"classDef {style.Key} fill:{style.Value.Color},stroke:#333,stroke-width:2px");
            }

            ProcessNode(this, writer);
        }

        /// <summary>
        /// Dictionary that maps a set of HTTP methods to HTML color.  Keys are sorted, upper-cased, concatenated HTTP methods.
        /// </summary>
        public readonly static IReadOnlyDictionary<string, MermaidNodeStyle> MermaidNodeStyles = new Dictionary<string, MermaidNodeStyle>(StringComparer.OrdinalIgnoreCase)
        {
            { "GET", new MermaidNodeStyle("lightSteelBlue", MermaidNodeShape.SquareCornerRectangle) },
            { "POST", new MermaidNodeStyle("Lightcoral", MermaidNodeShape.OddShape) },
            { "GET_POST", new MermaidNodeStyle("forestGreen", MermaidNodeShape.RoundedCornerRectangle) },
            { "DELETE_GET_PATCH", new MermaidNodeStyle("yellowGreen", MermaidNodeShape.Circle) },
            { "DELETE_GET_PATCH_PUT", new MermaidNodeStyle("oliveDrab", MermaidNodeShape.Circle) },
            { "DELETE_GET_PUT", new MermaidNodeStyle("olive", MermaidNodeShape.Circle) },
            { "DELETE_GET", new MermaidNodeStyle("DarkSeaGreen", MermaidNodeShape.Circle) },
            { "DELETE", new MermaidNodeStyle("Tomato", MermaidNodeShape.Rhombus) },
            { "OTHER", new MermaidNodeStyle("White", MermaidNodeShape.SquareCornerRectangle) },
        };

        private static void ProcessNode(OpenApiUrlTreeNode node, TextWriter writer)
        {
            var path = string.IsNullOrEmpty(node.Path) ? "/" : SanitizeMermaidNode(node.Path);
            var methods = GetMethods(node);
            var (startChar, endChar) = GetShapeDelimiters(methods);
            foreach (var child in node.Children)
            {
                var childMethods = GetMethods(child.Value);
                var (childStartChar, childEndChar) = GetShapeDelimiters(childMethods);
                writer.WriteLine($"{path}{startChar}\"{node.Segment}\"{endChar} --> {SanitizeMermaidNode(child.Value.Path)}{childStartChar}\"{child.Key}\"{childEndChar}");
                ProcessNode(child.Value, writer);
            }
            if (String.IsNullOrEmpty(methods)) methods = "OTHER";
            writer.WriteLine($"class {path} {methods}");
        }

        private static string GetMethods(OpenApiUrlTreeNode node)
        {
            return String.Join("_", node.PathItems.SelectMany(p => p.Value.Operations.Select(o => o.Key))
                .Distinct()
                .Select(o => o.ToString().ToUpper())
                .OrderBy(o => o)
                .ToList());
        }

        private static (string, string) GetShapeDelimiters(string methods)
        {
            if (MermaidNodeStyles.TryGetValue(methods, out var style))
            {
                //switch on shape
                switch (style.Shape)
                {
                    case MermaidNodeShape.Circle:
                        return ("((", "))");
                    case MermaidNodeShape.RoundedCornerRectangle:
                        return ("(", ")");
                    case MermaidNodeShape.Rhombus:
                        return ("{", "}");
                    case MermaidNodeShape.SquareCornerRectangle:
                        return ("[", "]");
                    case MermaidNodeShape.OddShape:
                        return (">", "]");
                    default:
                        return ("[", "]");
                }
            }
            else
            {
                return ("[", "]");
            }
        }
        private static string SanitizeMermaidNode(string token)
        {
            return token.Replace("\\", "/")
                    .Replace("{", ":")
                    .Replace("}", "")
                    .Replace(".", "_")
                    .Replace("(", "_")
                    .Replace(")", "_")
                    .Replace(";", "_")
                    .Replace("-", "_")
                    .Replace("graph", "gra_ph")  // graph is a reserved word
                    .Replace("default", "def_ault");  // default is a reserved word for classes
        }
    }
    /// <summary>
    /// Defines the color and shape of a node in a Mermaid graph diagram
    /// </summary>
    public class MermaidNodeStyle
    {
        /// <summary>
        /// Create a style that defines the color and shape of a diagram element
        /// </summary>
        /// <param name="color"></param>
        /// <param name="shape"></param>
        internal MermaidNodeStyle(string color, MermaidNodeShape shape)
        {
            Color = color;
            Shape = shape;
        }

        /// <summary>
        /// The CSS color name of the diagram element
        /// </summary>
        public string Color { get; }

        /// <summary>
        /// The shape of the diagram element
        /// </summary>
        public MermaidNodeShape Shape { get; }
    }

    /// <summary>
    /// Shapes supported by Mermaid diagrams
    /// </summary>
    public enum MermaidNodeShape
    {
        /// <summary>
        /// Rectangle with square corners
        /// </summary>
        SquareCornerRectangle,
        /// <summary>
        /// Rectangle with rounded corners
        /// </summary>
        RoundedCornerRectangle,
        /// <summary>
        /// Circle
        /// </summary>
        Circle,
        /// <summary>
        /// Rhombus
        /// </summary>
        Rhombus,
        /// <summary>
        /// Odd shape
        /// </summary>
        OddShape
    }
}
