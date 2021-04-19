// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// A directory structure representing the paths of an OpenAPI document.
    /// </summary>
    public class OpenApiUrlTreeNode
    {
        /// <summary>
        /// All the subdirectories of a node.
        /// </summary>
        public IDictionary<string, OpenApiUrlTreeNode> Children { get; } = new Dictionary<string, OpenApiUrlTreeNode>();

        /// <summary>
        /// The relative directory path of the current node from the root node.
        /// </summary>
        public string Path { get; private set; } = "";

        /// <summary>
        /// Dictionary of labels and Path Item objects that describe the operations available on a node.
        /// </summary>
        public IDictionary<string, OpenApiPathItem> PathItems { get; private set; }

        /// <summary>
        /// A container to hold key value pairs of additional data describing a node.
        /// </summary>
        public IDictionary<string, string> AdditionalData { get; set; }

        /// <summary>
        /// Flag indicating whether a node segment is a path parameter.
        /// </summary>
        public bool IsParameter => Segment.StartsWith("{");

        /// <summary>
        /// The subdirectory of a relative path.
        /// </summary>
        public string Segment { get; private set; }

        /// <summary>
        /// Flag indicating whether the node's PathItems has operations.
        /// </summary>
        /// <returns>true or false.</returns>
        public bool HasOperations(string label)
        {
            if ((bool)!PathItems?.ContainsKey(label))
            {
                return false;
            }

            return PathItems[label].Operations != null && PathItems[label].Operations.Count > 0;
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
            return new OpenApiUrlTreeNode(string.Empty);
        }

        /// <summary>
        /// Creates a structured directory of <see cref="OpenApiUrlTreeNode"/> nodes from the paths of an OpenAPI document.
        /// </summary>
        /// <param name="doc">The OpenAPI document.</param>
        /// <param name="label">Name tag for labelling the <see cref="OpenApiUrlTreeNode"/> nodes in the directory structure.</param>
        /// <returns>The root node of the created <see cref="OpenApiUrlTreeNode"/> directory structure.</returns>
        public static OpenApiUrlTreeNode Create(OpenApiDocument doc, string label)
        {
            Utils.CheckArgumentNull(doc, nameof(doc));
            Utils.CheckArgumentNullOrEmpty(label, nameof(label));

            OpenApiUrlTreeNode root = new OpenApiUrlTreeNode(string.Empty);

            var paths = doc.Paths;
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    root.Attach(path.Key, path.Value, label);
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
            Utils.CheckArgumentNull(doc, nameof(doc));
            Utils.CheckArgumentNullOrEmpty(label, nameof(label));

            var paths = doc?.Paths;
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    Attach(path.Key, path.Value, label);
                }
            }
        }

        /// <summary>
        /// Appends an OpenAPI path and the PathItems to an <see cref="OpenApiUrlTreeNode"/> node.
        /// </summary>
        /// <param name="path">An OpenAPI path.</param>
        /// <param name="pathItem">Path Item object that describes the operations available on an OpenAPI path.</param>
        /// <param name="label">A name tag for labelling the <see cref="OpenApiUrlTreeNode"/> node.</param>
        /// <returns>An <see cref="OpenApiUrlTreeNode"/> node describing an OpenAPI path.</returns>
        public OpenApiUrlTreeNode Attach(string path, OpenApiPathItem pathItem, string label)
        {
            if (path.StartsWith("/"))
            {
                // Remove leading slash
                path = path.Substring(1);
            }
            var segments = path.Split('/');
            return Attach(segments, pathItem, label, "");
        }

        /// <summary>
        /// Assembles the constituent properties of an <see cref="OpenApiUrlTreeNode"/> node.
        /// </summary>
        /// <param name="segments">IEnumerable subdirectories of a relative path.</param>
        /// <param name="pathItem">Path Item object that describes the operations available on an OpenAPI path.</param>
        /// <param name="label">A name tag for labelling the <see cref="OpenApiUrlTreeNode"/> node.</param>
        /// <param name="currentPath">The relative path of a node.</param>
        /// <returns>An <see cref="OpenApiUrlTreeNode"/> node with all constituent properties assembled.</returns>
        private OpenApiUrlTreeNode Attach(IEnumerable<string> segments, OpenApiPathItem pathItem, string label, string currentPath)
        {
            if (PathItems.ContainsKey(label))
            {
                throw new ArgumentException("A duplicate label already exists for this node.", nameof(label));
            }

            var segment = segments.FirstOrDefault();
            if (string.IsNullOrEmpty(segment))
            {
                PathItems = new Dictionary<string, OpenApiPathItem>
                {
                    {
                        label, pathItem
                    }
                };
                Path = currentPath;

                return this;
            }

            // If the child segment has already been defined, then insert into it
            if (Children.ContainsKey(segment))
            {
                return Children[segment].Attach(segments.Skip(1), pathItem, label, currentPath + "\\" + segment);
            }
            else
            {
                var node = new OpenApiUrlTreeNode(segment)
                {
                    Path = currentPath + "\\" + segment
                };

                Children[segment] = node;
                return node.Attach(segments.Skip(1), pathItem, label, currentPath + "\\" + segment);
            }
        }
    }
}
