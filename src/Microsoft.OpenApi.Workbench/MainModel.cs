using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Writers;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Readers.YamlReaders;
using Microsoft.OpenApi.Models;

namespace OpenApiWorkbench
{
    public class MainModel : INotifyPropertyChanged
    {
        public string input;
        public string Input { get { return input; } set { input = value; OnPropertyChanged("Input"); } }

        public string output;
        public string Output { get { return errors; } set { errors = value; OnPropertyChanged("Output"); } }

        private string errors;
        public string Errors { get { return errors; } set { errors = value; OnPropertyChanged("Errors"); } }

        public string parseTime;
        public string ParseTime { get { return parseTime; } set { parseTime = value; OnPropertyChanged("ParseTime"); } }

        public string renderTime;
        public string RenderTime { get { return renderTime; } set { renderTime = value; OnPropertyChanged("RenderTime"); } }

        private string format = "Yaml";
        private string version = "V3";

        public string Format {
         get {
                return format;
            }
        set
            {
                format = value;
                OnPropertyChanged("IsYaml");
                OnPropertyChanged("IsJson");
            }
        }

        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
                OnPropertyChanged("IsV2");
                OnPropertyChanged("IsV3");
            }
        }

        public bool IsYaml { get { return Format == "Yaml"; } set { Format = "Yaml"; }  }
        public bool IsJson { get { return Format == "JSON"; } set { Format ="JSON"; } }

        public bool IsV2{ get { return Version == "V2"; } set { Version = "V2"; } }
        public bool IsV3 { get { return Version == "V3"; } set { Version = "V3"; } }


        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        internal void Validate()
        {
            try
            { 

                MemoryStream stream = CreateStream(input);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var openApiDocument = new OpenApiStreamReader().Read(stream, out var context);
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
                Output = WriteContents(openApiDocument);
                stopwatch.Stop();

                RenderTime = $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch (Exception ex)
            {
                Errors = "Failed to parse input: " + ex.Message;
            }

            // Verify output is valid JSON or YAML
            //var dummy = YamlHelper.ParseYaml(Output);
        }
        
        private string WriteContents(OpenApiDocument doc)
        {
            var outputstream = new MemoryStream();
            OpenApiSerializer seriazlier = new OpenApiSerializer
            {
                Format = this.format == "Yaml" ? OpenApiFormat.Yaml : OpenApiFormat.Json,
                SpecVersion = IsV3 ? OpenApiSpecVersion.OpenApi3_0 : OpenApiSpecVersion.OpenApi2_0,
            };
            seriazlier.Serialize(outputstream, doc);
            outputstream.Position = 0;

            return new StreamReader(outputstream).ReadToEnd();
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


    public event PropertyChangedEventHandler PropertyChanged;
    }
}
