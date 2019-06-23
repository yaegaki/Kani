using dnSpy.Contracts.Decompiler;

namespace Kani.Decompile
{
    public interface IDecompilerEventReceiver
    {
        void ShowReference(object reference, DecompilerReferenceFlags flags);
    }
}
