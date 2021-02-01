using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedExtensionsBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Extension;

        private readonly Dictionary<string, IOpenApiExtension> _oldExtensions;
        private readonly Dictionary<string, IOpenApiExtension> _newExtensions;
        private readonly DiffContextBO _context;

        public Dictionary<string, ChangedBO> Increased { get; set; }
        public Dictionary<string, ChangedBO> Missing { get; set; }
        public Dictionary<string, ChangedBO> Changed { get; set; }

        public ChangedExtensionsBO(Dictionary<string, IOpenApiExtension> oldExtensions, Dictionary<string, IOpenApiExtension> newExtensions, DiffContextBO context)
        {
            _oldExtensions = oldExtensions;
            _newExtensions = newExtensions;
            _context = context;
            Increased = new Dictionary<string, ChangedBO>();
            Missing = new Dictionary<string, ChangedBO>();
            Changed = new Dictionary<string, ChangedBO>();
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>()
                .Concat(Increased.Select(x => (x.Key, (ChangedBO)x.Value)))
                .Concat(Missing.Select(x => (x.Key, (ChangedBO)x.Value)))
                .Concat(Changed.Select(x => (x.Key, (ChangedBO)x.Value)))
                .Where(x => x.Item2 != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            return new DiffResultBO(DiffResultEnum.NoChanges);
        }

        protected override List<ChangedInfoBO> GetCoreChanges()
        {
            return new List<ChangedInfoBO>();
        }
    }
}
