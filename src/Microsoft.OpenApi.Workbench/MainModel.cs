// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
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

        public string Input
        {
            get => _input;
            set
            {
                _input = value;
                OnPropertyChanged(nameof(Input));
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

        public OpenApiSpecVersion Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(IsV2_0));
                OnPropertyChanged(nameof(IsV3_0));
            }
        }

        public bool IsYaml
        {
            get => Format == OpenApiFormat.Yaml;
            set => Format = OpenApiFormat.Yaml;
        }

        public bool IsJson
        {
            get => Format == OpenApiFormat.Json;
            set => Format = OpenApiFormat.Json;
        }

        public bool IsV2_0
        {
            get => Version == OpenApiSpecVersion.OpenApi2_0;
            set => Version = OpenApiSpecVersion.OpenApi2_0;
        }

        public bool IsV3_0
        {
            get => Version == OpenApiSpecVersion.OpenApi3_0;
            set => Version = OpenApiSpecVersion.OpenApi3_0;
        }

        /// <summary>
        /// Handling method when the property with given name has changed.
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The core method of the class.
        /// Runs the parsing and serializing.
        /// </summary>
        internal void ParseDocument()
        {
            try
            {
                Stream stream;
                if (!String.IsNullOrWhiteSpace(_inputFile))
                {
                    stream = new FileStream(_inputFile, FileMode.Open);
                }
                else
                {
                    stream = CreateStream(_input);
                }
                

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var document = new OpenApiStreamReader(new OpenApiReaderSettings
                    {
                        ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences,
                        RuleSet = ValidationRuleSet.GetDefaultRuleSet()
                    }
                ).Read(stream, out var context);
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
                Format);
            
            outputStream.Position = 0;

            return new StreamReader(outputStream).ReadToEnd();
        }

        private MemoryStream CreateStream(string text)
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