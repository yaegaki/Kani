using Kani.Services;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Kani.Models.TreeView
{
    public class AssemblyTreeViewItem : ITreeViewItem
    {
        public AssemblyDef Assembly { get; }
        public IReadOnlyList<TextRun> Texts { get; }
        public bool IsLeaf => false;

        private IDocumentService documentService;
        private ITreeViewItem[] children;

        public AssemblyTreeViewItem(IDocumentService documentService, AssemblyDef assembly)
        {
            this.documentService = documentService;
            this.Assembly = assembly;
            this.Texts = new[]
            {
                new TextRun(this.Assembly.Name.String),
            };
        }

        public void Select()
        {
        }

        public IEnumerable<ITreeViewItem> EnumerateChildren()
        {
            if (this.children == null)
            {
                this.children = this.Assembly.Modules
                    .Select(m => new ModuleTreeViewItem(this.documentService, m))
                    .ToArray();
            }

            return this.children;
        }
    }
}
