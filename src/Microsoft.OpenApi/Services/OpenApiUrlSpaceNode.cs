// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// A directory structure representing the paths of an OpenAPI document.
    /// </summary>
    public class OpenApiUrlSpaceNode
    {
        /// <summary>
        /// All the subdirectories of a node.
        /// </summary>
        public IDictionary<string, OpenApiUrlSpaceNode> Children { get; set; } = new Dictionary<string, OpenApiUrlSpaceNode>();

        /// <summary>
        /// The name tag for a group of nodes.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Path Item object that describes the operations available on a node.
        /// </summary>
        public OpenApiPathItem PathItem { get; private set; }

        /// <summary>
        /// The relative directory path of the current node from the root node.
        /// </summary>
        public string Path { get; set; } = "";

        /// <summary>
        /// Flag indicating whether a node segment is a path parameter.
        /// </summary>
        public bool IsParameter => Segment.StartsWith("{");

        /// <summary>
        /// Flag indicating whether a node segment is a path function.
        /// </summary>
        public bool IsFunction => Segment.Contains("(");

        /// <summary>
        /// The subdirectory of a relative path.
        /// </summary>
        public string Segment { get; private set; }

        /// <summary>
        /// The Pascal-cased alphabet name of a segment.
        /// </summary>
        public string Identifier
        {
            get
            {
                string identifier;
                if (IsParameter)
                {
                    identifier = Segment.Substring(1, Segment.Length - 2).ToPascalCase();
                }
                else
                {
                    identifier = Segment.ToPascalCase().Replace("()", "");
                    var openParen = identifier.IndexOf("(");
                    if (openParen >= 0)
                    {
                        identifier = identifier.Substring(0, openParen);
                    }
                }
                return identifier;
            }
        }

        /// <summary>
        /// Flag indicating whether a PathItem has operations.
        /// </summary>
        /// <returns>true or false.</returns>
        public bool HasOperations()
        {
            return PathItem != null && PathItem.Operations != null && PathItem.Operations.Count > 0;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="segment">The subdirectory of a relative path.</param>
        public OpenApiUrlSpaceNode(string segment)
        {
            Segment = segment;
        }

        /// <summary>
        /// Uses SHA256 hash algorithm to hash the Path value.
        /// </summary>
        /// <returns>The hashed value.</returns>
        public string Hash()
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return GetHash(sha256Hash, Path);
            }
        }

        /// <summary>
        /// Hashes a string value using a specified hash algorithm.
        /// </summary>
        /// <param name="hashAlgorithm">The hash algorithm to use for hashing.</param>
        /// <param name="input">The string to hash.</param>
        /// <returns>The hashed value.</returns>
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < 2; i++)  // data.Length  Limit to 4 chars
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Creates a structured directory of nodes from the paths of an OpenAPI document.
        /// </summary>
        /// <param name="doc">The OpenAPI document.</param>
        /// <param name="label">Name tag for labelling the nodes in the directory structure.</param>
        /// <returns>The root node of the created directory structure.</returns>
        public static OpenApiUrlSpaceNode Create(OpenApiDocument doc, string label = "")
        {
            OpenApiUrlSpaceNode root = null;

            var paths = doc?.Paths;
            if (paths != null)
            {
                root = new OpenApiUrlSpaceNode("");

                foreach (var path in paths)
                {
                    root.Attach(path.Key, path.Value, label);
                }
            }
            return root;
        }

        /// <summary>
        /// Retrieves the paths from an OpenAPI document and appends the items to a node.
        /// </summary>
        /// <param name="doc">The OpenAPI document.</param>
        /// <param name="label">Name tag for labelling the nodes in the directory structure.</param>
        public void Attach(OpenApiDocument doc, string label)
        {
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
        /// Appends an OpenAPI path and the PathItems to a node.
        /// </summary>
        /// <param name="path">An OpenAPI path.</param>
        /// <param name="pathItem">Path Item object that describes the operations available on an OpenAPI path.</param>
        /// <param name="label">A name tag for labelling the node.</param>
        /// <returns>A node describing an OpenAPI path.</returns>
        public OpenApiUrlSpaceNode Attach(string path, OpenApiPathItem pathItem, string label)
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
        /// Assembles the constituent properties of a node.
        /// </summary>
        /// <param name="segments">IEnumerable subdirectories of a relative path.</param>
        /// <param name="pathItem">Path Item object that describes the operations available on an OpenAPI path.</param>
        /// <param name="label">A name tag for labelling the node.</param>
        /// <param name="currentPath">The relative path of a node.</param>
        /// <returns>A node with all constituent properties assembled.</returns>
        private OpenApiUrlSpaceNode Attach(IEnumerable<string> segments, OpenApiPathItem pathItem, string label, string currentPath)
        {
            var segment = segments.FirstOrDefault();
            if (string.IsNullOrEmpty(segment))
            {
                if (PathItem == null)
                {
                    PathItem = pathItem;
                    Path = currentPath;
                    Label = label;
                }
                return this;
            }

            // If the child segment has already been defined, then insert into it
            if (Children.ContainsKey(segment))
            {
                return Children[segment].Attach(segments.Skip(1), pathItem, label, currentPath + "\\" + segment );
            }
            else
            {
                var node = new OpenApiUrlSpaceNode(segment)
                {
                    Path = currentPath + "\\" + segment
                };

                Children[segment] = node;
                return node.Attach(segments.Skip(1), pathItem, label, currentPath + "\\" + segment);
            }
        }
    }
}
