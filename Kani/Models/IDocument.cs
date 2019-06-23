using dnSpy.Contracts.Decompiler;

namespace Kani.Models
{
    public interface IDocument
    {
        object Source { get; }
        void Decompile(IDecompiler decompiler, IDecompilerOutput output, DecompilationContext ctx);
    }
}
