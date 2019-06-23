using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using Kani.Decompile;
using Kani.Services;

namespace Kani.Models.TreeView
{
    public class PropertyTreeViewItem : ITreeViewItem
    {
        public PropertyDef Property { get; } 

        public IReadOnlyList<TextRun> Texts { get; }
        public bool IsLeaf => true;

        private IDocumentService documentService;

        public PropertyTreeViewItem(IDocumentService documentService, PropertyDef property)
        {
            this.documentService = documentService;
            this.Property = property;

            this.Texts = BuildTexts();
        }

        public void Select()
        {
            this.documentService.Show(this.Property);
        }

        public IEnumerable<ITreeViewItem> EnumerateChildren()
            => Enumerable.Empty<ITreeViewItem>();

        private IReadOnlyList<TextRun> BuildTexts()
        {
            var texts = new List<TextRun>();
            
            texts.Add(new TextRun(this.Property.Name.String, this.Property.PropertySig.HasThis ? "d-iproperty" : "d-sproperty"));
            texts.Add(new TextRun(" : "));

            DecompileFormatUtil.AddTexts(this.Property.PropertySig.RetType, texts);

            return texts;
        }
    }
}
