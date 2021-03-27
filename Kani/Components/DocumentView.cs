using System;
using System.Threading;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnSpy.Contracts.Decompiler;
using dnSpy.Contracts.Text;
using Kani.Decompile;
using Kani.Models;
using Kani.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Kani.Components
{
    public class DocumentView : ComponentBase, IDecompilerEventReceiver, IDisposable
    {
        [Inject]
        private IDocumentService DocumentService { get; set; }
        [Inject]
        private IAssemblyRegistry AssemblyRegistry { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        private IDecompiler decompiler;
        private IDocument document;
        private object sync = new object();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private ElementReference elementRef;
        private bool documentChanged = true;

        protected override void OnInitialized()
        {
            DocumentService.OnShown += OnShown;

            var asm = typeof(dnSpy.Decompiler.ILSpy.Core.Properties.dnSpy_Decompiler_ILSpy_Core_Resources).Assembly;

            foreach (var type in asm.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract) continue;

                var b = typeof(dnSpy.Contracts.Decompiler.IDecompilerProvider).IsAssignableFrom(type);
                if (b)
                {
                    var instance = Activator.CreateInstance(type) as dnSpy.Contracts.Decompiler.IDecompilerProvider;
                    foreach (var d in instance.Create())
                    {
                        if (d.UniqueGuid != DecompilerConstants.LANGUAGE_CSHARP_ILSPY) continue;

                        this.decompiler = d;
                        return;
                    }
                }
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "dark-editor");
            builder.AddElementReferenceCapture(2, elementRef => {
                this.elementRef = elementRef;
            });

            if (this.document != null && this.decompiler != null) {
                using (var output = new DecompilerOutput(this, builder, 3, new Indenter(4, 4, false)))
                {
                    var decompilationContext = new DecompilationContext();
                    this.document.Decompile(this.decompiler, output, decompilationContext);
                }
            }
            builder.CloseElement();
        }

        protected override async Task OnAfterRenderAsync(bool isFirstRender)
        {
            if (this.documentChanged)
            {
                await this.JSRuntime.InvokeAsync<object>("scrollToTop", this.elementRef);
                this.documentChanged = false;
            }
        }

        private void OnShown(IDocument doc)
        {
            lock (this.sync)
            {
                this.cts.Cancel();
                this.cts.Dispose();
                this.cts = new CancellationTokenSource();
                this.document = doc;
                this.documentChanged = true;
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            DocumentService.OnShown -= OnShown;
        }

        public void ShowReference(object reference, DecompilerReferenceFlags flags)
        {
            // System.Console.WriteLine($"{reference}, {reference?.GetType()}, {flags}");
            var ct = this.cts.Token;
            switch (reference)
            {
                case TypeDef typeDef:
                    this.DocumentService.Show(typeDef);
                    break;
                case TypeRef typeRef:
                    Show(this.AssemblyRegistry.ResolveAsync(typeRef, ct), t => this.DocumentService.Show(t), ct);
                    break;
                case MethodDef methodDef:
                    this.DocumentService.Show(methodDef);
                    break;
                case MethodSpec methodSpec:
                    Show(this.AssemblyRegistry.ResolveAsync(methodSpec, ct), m => this.DocumentService.Show(m), ct);
                    break;
                case FieldDef fieldDef:
                    this.DocumentService.Show(fieldDef);
                    break;
                case MemberRef memberRef:
                    Show(this.AssemblyRegistry.ResolveAsync(memberRef, ct), m => this.DocumentService.Show(m), ct);
                    break;
                default:
                    break;
            }
        }

        private void Show<T>(Task<T> task, Action<T> show, CancellationToken cancellationToken)
        {
            _ = task.ContinueWith(t =>
                {
                    lock (this.sync)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var result = task.Result;
                        if (result != null)
                        {
                            show(result);
                        }
                    }
                });
        }
    }
}
