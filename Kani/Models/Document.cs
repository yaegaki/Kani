using System;
using dnSpy.Contracts.Decompiler;

namespace Kani.Models
{
    public class Document : IDocument
    {
        public object Source { get; }
        private Action<IDecompiler, IDecompilerOutput, DecompilationContext> decompile;

        public Document(object source, Action<IDecompiler, IDecompilerOutput, DecompilationContext> decompile)
            => (this.Source, this.decompile) = (source, decompile);

        public void Decompile(IDecompiler decompiler, IDecompilerOutput output, DecompilationContext ctx)
            => this.decompile(decompiler, output, ctx);
    }
}
