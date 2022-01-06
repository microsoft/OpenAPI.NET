namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Configuration settings to control how OpenAPI Json documents are written
    /// </summary>
    public class OpenApiJsonWriterSettings : OpenApiWriterSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiJsonWriterSettings"/> class.
        /// </summary>
        public OpenApiJsonWriterSettings()
        { }

        /// <summary>
        /// Indicates whether or not the produced document will be written in a compact or pretty fashion.
        /// </summary>
        public bool Terse { get; set; } = false;
    }
}
