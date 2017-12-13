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

namespace Microsoft.OpenApi.Workbench
{
    /// <summary>
    /// Main model of the Workbench tool.
    /// </summary>
    public class MainModel : INotifyPropertyChanged
    {
        private string _input;

        private string _output;

        private string _errors;

        private string _parseTime;

        private string _renderTime;

        /// <summary>
        /// Default format.
        /// </summary>
        private string _format = "Yaml";

        /// <summary>
        /// Default version.
        /// </summary>
        private string _version = "V3";

        public string Input
        {
            get => _input;
            set
            {
                _input = value;
                OnPropertyChanged("Input");
            }
        }

        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                OnPropertyChanged("Output");
            }
        }

        public string Errors
        {
            get => _errors;
            set
            {
                _errors = value;
                OnPropertyChanged("Errors");
            }
        }

        public string ParseTime
        {
            get => _parseTime;
            set
            {
                _parseTime = value;
                OnPropertyChanged("ParseTime");
            }
        }

        public string RenderTime
        {
            get => _renderTime;
            set
            {
                _renderTime = value;
                OnPropertyChanged("RenderTime");
            }
        }

        public string Format
        {
            get => _format;
            set
            {
                _format = value;
                OnPropertyChanged("IsYaml");
                OnPropertyChanged("IsJson");
            }
        }

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged("IsV2");
                OnPropertyChanged("IsV3");
            }
        }

        public bool IsYaml
        {
            get => Format == "Yaml";
            set => Format = "Yaml";
        }

        public bool IsJson
        {
            get => Format == "JSON";
            set => Format = "JSON";
        }

        public bool IsV2
        {
            get => Version == "V2";
            set => Version = "V2";
        }

        public bool IsV3
        {
            get => Version == "V3";
            set => Version = "V3";
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
        internal void Validate()
        {
            try
            {
                var stream = CreateStream(_input);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var document = new OpenApiStreamReader().Read(stream, out var context);
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
                IsV3 ? OpenApiSpecVersion.OpenApi3_0_0 : OpenApiSpecVersion.OpenApi2_0,
                _format == "Yaml" ? OpenApiFormat.Yaml : OpenApiFormat.Json);
            
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