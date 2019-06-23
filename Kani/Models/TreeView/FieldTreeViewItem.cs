using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using Kani.Decompile;
using Kani.Services;

namespace Kani.Models.TreeView
{
    public class FieldTreeViewItem : ITreeViewItem
    {
        public FieldDef Field { get; } 

        public IReadOnlyList<TextRun> Texts { get; }
        public bool IsLeaf => true;

        private IDocumentService documentService;

        public FieldTreeViewItem(IDocumentService documentService, FieldDef property)
        {
            this.documentService = documentService;
            this.Field = property;

            this.Texts = BuildTexts();
        }

        public void Select()
        {
            this.documentService.Show(this.Field);
        }

        public IEnumerable<ITreeViewItem> EnumerateChildren()
            => Enumerable.Empty<ITreeViewItem>();

        private IReadOnlyList<TextRun> BuildTexts()
        {
            var texts = new List<TextRun>();
            
            string cssClass;
            if (this.Field.DeclaringType.IsEnum)
            {
                cssClass = "d-efield";
            }
            else
            {
                cssClass = this.Field.FieldSig.HasThis ? "d-ifield" : "d-sfield";
            }

            texts.Add(new TextRun(this.Field.Name.String, cssClass));
            texts.Add(new TextRun(" : "));

            DecompileFormatUtil.AddTexts(this.Field.FieldSig.Type, texts);

            return texts;
        }
    }
}

