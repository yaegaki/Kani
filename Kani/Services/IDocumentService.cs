using System;
using dnlib.DotNet;
using Kani.Decompile;
using Kani.Models;

namespace Kani.Services
{
    public interface IDocumentService
    {
        event Action<IDocument> OnShown;

        void Show(IDocument document);
    }

    public static class DocumentServiceExtensions
    {
        public static void Show(this IDocumentService s, TypeDef type)
            => s.Show(new Document(type, (decompiler, output, ctx) => decompiler.Decompile(type, output, ctx)));

        public static void Show(this IDocumentService s, PropertyDef property)
            => s.Show(new Document(property, (decompiler, output, ctx) => decompiler.Decompile(property, output, ctx)));

        public static void Show(this IDocumentService s, MethodDef method)
            => s.Show(new Document(method, (decompiler, output, ctx) => decompiler.Decompile(method, output, ctx)));

        public static void Show(this IDocumentService s, FieldDef field)
            => s.Show(new Document(field, (decompiler, output, ctx) => decompiler.Decompile(field, output, ctx)));

        public static void Show(this IDocumentService s, MethodOrFieldDef methodOrFieldDef)
        {
            if (methodOrFieldDef.IsFieldDef)
            {
                s.Show(methodOrFieldDef.FieldDef);
            }
            else
            {
                s.Show(methodOrFieldDef.MethodDef);
            }
        }
    }
}
