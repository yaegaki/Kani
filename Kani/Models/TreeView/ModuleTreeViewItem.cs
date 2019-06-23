using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using Kani.Services;

namespace Kani.Models.TreeView
{
    public class ModuleTreeViewItem : ITreeViewItem
    {
        public ModuleDef Module { get; }

        public IReadOnlyList<TextRun> Texts { get; }
        public bool IsLeaf => false;

        private IDocumentService documentService;
        private ITreeViewItem[] children;

        public ModuleTreeViewItem(IDocumentService documentService, ModuleDef module)
        {
            this.documentService = documentService;
            this.Module = module;
            this.Texts = new[]
            {
                new TextRun(module.Name.String, "d-module"),
            };
        }

        public void Select()
        {
        }

        public IEnumerable<ITreeViewItem> EnumerateChildren()
        {
            if (this.children == null)
            {
                this.children = this.Module.Types
                    .GroupBy(t => t.Namespace)
                    .Select(g => new NamespaceTreeViewItem(this.documentService, g.Key, g))
                    .ToArray();
            }

            return this.children;
        }
    }
}
