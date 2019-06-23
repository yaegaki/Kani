using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using Kani.Decompile;
using Kani.Services;

namespace Kani.Models.TreeView
{
    public class MethodTreeViewItem : ITreeViewItem
    {
        public MethodDef Method { get; } 

        public IReadOnlyList<TextRun> Texts { get; }
        public bool IsLeaf => true;

        private IDocumentService documentService;

        public MethodTreeViewItem(IDocumentService documentService, MethodDef method)
        {
            this.documentService = documentService;
            this.Method = method;

            this.Texts = BuildTexts();
        }

        public void Select()
        {
            this.documentService.Show(this.Method);
        }

        public IEnumerable<ITreeViewItem> EnumerateChildren()
            => Enumerable.Empty<ITreeViewItem>();

        private IReadOnlyList<TextRun> BuildTexts()
        {
            var texts = new List<TextRun>();
            texts.Add(new TextRun(this.Method.Name.String, this.Method.IsStatic ? "d-smethod" : "d-imethod"));
            texts.Add(new TextRun("("));
            var isFirst = true;
            foreach (var param in this.Method.Parameters)
            {
                if (!param.IsNormalMethodParameter) continue;

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    texts.Add(new TextRun(", "));
                }


                var type = param.Type;
                if (type.IsByRef)
                {
                    if (!param.ParamDef.IsIn && param.ParamDef.IsOut)
                    {
                        texts.Add(new TextRun("out ", "d-keyword"));
                    }
                    else
                    {
                        texts.Add(new TextRun("ref ", "d-keyword"));
                    }

                    type = type.Next;
                }

                DecompileFormatUtil.AddTexts(type, texts);
            }
            texts.Add(new TextRun(") : "));

            DecompileFormatUtil.AddTexts(Method.ReturnType, texts);

            return texts;
        }
    }
}
