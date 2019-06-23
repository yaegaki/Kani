using dnlib.DotNet;

namespace Kani.Decompile
{
    public class MethodOrFieldDef
    {
        public bool IsMethodDef => MethodDef != null;
        public bool IsFieldDef => FieldDef != null;
        public MethodDef MethodDef { get; }
        public FieldDef FieldDef { get; }

        public MethodOrFieldDef(MethodDef methodDef)
            => this.MethodDef = methodDef;

        public MethodOrFieldDef(FieldDef fieldDef)
            => this.FieldDef = fieldDef;
    }
}
