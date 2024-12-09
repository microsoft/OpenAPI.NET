// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;

namespace Microsoft.OpenApi.Workbench
{
    /// <summary>
    /// Main model of the Workbench tool.
    /// </summary>
    public class MainModel : INotifyPropertyChanged
    {
        private string _input;

        private bool _inlineLocal;
        private bool _inlineExternal;

        private bool _resolveExternal;

        private string _inputFile;

        private string _output;

        private string _errors;

        private string _parseTime;

        private string _renderTime;

        /// <summary>
        /// Default format.
        /// </summary>
        private OpenApiFormat _format = OpenApiFormat.Yaml;

        /// <summary>
        /// Default version.
        /// </summary>
        private OpenApiSpecVersion _version = OpenApiSpecVersion.OpenApi3_0;

        private static readonly HttpClient _httpClient = new();

        public string Input
        {
            get => _input;
            set
            {
                _input = value;
                OnPropertyChanged(nameof(Input));
            }
        }

        public bool ResolveExternal
        {
            get => _resolveExternal;
            set
            {
                _resolveExternal = value;
                OnPropertyChanged(nameof(ResolveExternal));
            }
        }

        public string InputFile
        {
            get => _inputFile;
            set
            {
                _inputFile = value;
                OnPropertyChanged(nameof(InputFile));
            }
        }
        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                OnPropertyChanged(nameof(Output));
            }
        }

        public string Errors
        {
            get => _errors;
            set
            {
                _errors = value;
                OnPropertyChanged(nameof(Errors));
            }
        }

        public string ParseTime
        {
            get => _parseTime;
            set
            {
                _parseTime = value;
                OnPropertyChanged(nameof(ParseTime));
            }
        }

        public string RenderTime
        {
            get => _renderTime;
            set
            {
                _renderTime = value;
                OnPropertyChanged(nameof(RenderTime));
            }
        }

        public OpenApiFormat Format
        {
            get => _format;
            set
            {
                _format = value;
                OnPropertyChanged(nameof(IsYaml));
                OnPropertyChanged(nameof(IsJson));
            }
        }

        public bool InlineLocal
        {
            get => _inlineLocal;
            set
            {
                _inlineLocal = value;
                OnPropertyChanged(nameof(InlineLocal));
            }
        }

        public bool InlineExternal
        {
            get => _inlineExternal;
            set
            {
                _inlineExternal = value;
                OnPropertyChanged(nameof(InlineExternal));
            }
        }

        public OpenApiSpecVersion Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(IsV2_0));
                OnPropertyChanged(nameof(IsV3_0));
                OnPropertyChanged(nameof(IsV3_1));
            }
        }

        public bool IsYaml
        {
            get => Format == OpenApiFormat.Yaml;
            set => Format = value ? OpenApiFormat.Yaml : Format;
        }

        public bool IsJson
        {
            get => Format == OpenApiFormat.Json;
            set => Format = value ? OpenApiFormat.Json : Format;
        }

        public bool IsV2_0
        {
            get => Version == OpenApiSpecVersion.OpenApi2_0;
            set => Version = value ? OpenApiSpecVersion.OpenApi2_0 : Version;
        }

        public bool IsV3_0
        {
            get => Version == OpenApiSpecVersion.OpenApi3_0;
            set => Version = value ? OpenApiSpecVersion.OpenApi3_0 : Version;
        }

        public bool IsV3_1
        {
            get => Version == OpenApiSpecVersion.OpenApi3_1;
            set => Version = value ? OpenApiSpecVersion.OpenApi3_1 : Version;
        }

        /// <summary>
        /// Handling method when the property with given name has changed.
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new(propertyName));
            }
        }

        /// <summary>
        /// The core method of the class.
        /// Runs the parsing and serializing.
        /// </summary>
        internal async Task ParseDocumentAsync()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yml, new OpenApiYamlReader());

            Stream stream = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(_inputFile))
                {
                    stream = _inputFile.StartsWith("http") ? await _httpClient.GetStreamAsync(_inputFile) 
                        : new FileStream(_inputFile, FileMode.Open);
                }
                else
                {
                    if (ResolveExternal)
                    {
                        throw new ArgumentException("Input file must be used to resolve external references");
                    }
                    stream = CreateStream(_input);
                }

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var settings = new OpenApiReaderSettings
                {
                    ReferenceResolution = ResolveExternal ? ReferenceResolutionSetting.ResolveAllReferences : ReferenceResolutionSetting.ResolveLocalReferences,
                    RuleSet = ValidationRuleSet.GetDefaultRuleSet()
                };
                if (ResolveExternal && !string.IsNullOrWhiteSpace(_inputFile))
                {
                    settings.BaseUrl = _inputFile.StartsWith("http") ? new(_inputFile) 
                        : new("file://" + Path.GetDirectoryName(_inputFile) + "/");
                }

                var readResult = await OpenApiDocument.LoadAsync(stream, Format.GetDisplayName());
                var document = readResult.Document;
                var context = readResult.Diagnostic;

                stopwatch.Stop();
                ParseTime = $"{stopwatch.ElapsedMilliseconds} ms";

                if (context.Errors.Count == 0)
                {
                    Errors = "OK";
                }
                else
                {
                    var errorReport = new StringBuilder();

                    foreach (var error in context.Errors)
                    {
                        errorReport.AppendLine(error.ToString());
                    }

                    Errors = errorReport.ToString();
                }

                stopwatch.Reset();
                stopwatch.Start();
                Output = WriteContents(document);
                stopwatch.Stop();

                RenderTime = $"{stopwatch.ElapsedMilliseconds} ms";

                var statsVisitor = new StatsVisitor();
                var walker = new OpenApiWalker(statsVisitor);
                walker.Walk(document);

                Errors += Environment.NewLine + "Statistics:" + Environment.NewLine + statsVisitor.GetStatisticsReport();
            }
            catch (Exception ex)
            {
                Output = string.Empty;
                Errors = "Failed to parse input: " + ex.Message;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    await stream.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// Write content from the given document based on the format and version set in this class.
        /// </summary>
        private string WriteContents(OpenApiDocument document)
        {
            var outputStream = new MemoryStream();

            document.Serialize(
                outputStream,
                Version,
                Format,
                new()
                {
                    InlineLocalReferences = InlineLocal,
                    InlineExternalReferences = InlineExternal
                });

            outputStream.Position = 0;

            return new StreamReader(outputStream).ReadToEnd();
        }

        private static MemoryStream CreateStream(string text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(text);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
