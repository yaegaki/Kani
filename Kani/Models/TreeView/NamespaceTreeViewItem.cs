using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using Kani.Services;

namespace Kani.Models.TreeView
{
    public class NamespaceTreeViewItem : ITreeViewItem
    {
        public UTF8String Name { get; } 
        public IReadOnlyList<TypeDef> Types { get; }

        public IReadOnlyList<TextRun> Texts { get; }
        public bool IsLeaf => false;

        private IDocumentService documentService;
        private ITreeViewItem[] children;

        public NamespaceTreeViewItem(IDocumentService documentService, UTF8String name, IEnumerable<TypeDef> types)
        {
            this.documentService = documentService;
            this.Name = name;
            this.Types = types.ToArray();
            this.Texts = new[]
            {
                new TextRun(UTF8String.IsNullOrEmpty(this.Name) ? "-" : this.Name.String, "d-namespace"),
            };
        }

        public void Select()
        {
        }

        public IEnumerable<ITreeViewItem> EnumerateChildren()
        {
            if (this.children == null)
            {
                this.children = this.Types
                    .Select(t => new TypeTreeViewItem(this.documentService, t))
                    .ToArray();
            }

            return this.children;
        }
    }
}
