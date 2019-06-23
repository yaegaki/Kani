using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using Kani.Decompile;
using Kani.Services;

namespace Kani.Models.TreeView
{
    public class TypeTreeViewItem : ITreeViewItem
    {
        public TypeDef Type { get; } 

        public IReadOnlyList<TextRun> Texts { get; }
        public bool IsLeaf => false;

        private IDocumentService documentService;
        private ITreeViewItem[] children;

        public TypeTreeViewItem(IDocumentService documentService, TypeDef type)
        {
            this.documentService = documentService;
            this.Type = type;

            var texts = new List<TextRun>();
            DecompileFormatUtil.AddTexts(this.Type, texts);
            this.Texts = texts;
        }

        public void Select()
        {
            this.documentService.Show(this.Type);
        }

        public IEnumerable<ITreeViewItem> EnumerateChildren()
        {
            if (this.children == null)
            {
                var fields = this.Type.Fields
                    .Select(f => new FieldTreeViewItem(this.documentService, f))
                    .Cast<ITreeViewItem>();

                var properties = this.Type.Properties
                    .Select(p => new PropertyTreeViewItem(this.documentService, p))
                    .Cast<ITreeViewItem>();

                var methods = this.Type.Methods
                    .Select(m => new MethodTreeViewItem(this.documentService, m))
                    .Cast<ITreeViewItem>();
                
                var nestedTypes = this.Type.NestedTypes
                    .Select(t => new TypeTreeViewItem(this.documentService, t))
                    .Cast<ITreeViewItem>();
                
                this.children = fields
                    .Concat(properties)
                    .Concat(methods)
                    .Concat(nestedTypes)
                    .ToArray();
            }

            return this.children;
        }
    }
}
