using System;
using System.Text;
using dnSpy.Contracts.Decompiler;
using dnSpy.Contracts.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Kani.Decompile
{
    public class DecompilerOutput : IDecompilerOutput, IDisposable
    {
        private StringBuilder stringBuilder = new StringBuilder();
        private IDecompilerEventReceiver eventReceiver;
        private RenderTreeBuilder builder;
        public int NextSequence;
        private Indenter indenter;
        private bool addIndent = true;
        private bool elementOpened = false;
        private int lineNumber = 1;

        public DecompilerOutput(IDecompilerEventReceiver eventReceiver, RenderTreeBuilder builder, int sequence, Indenter indenter)
        {
            this.eventReceiver = eventReceiver;
            this.builder = builder;
            this.NextSequence = sequence; 
            this.indenter = indenter;
        }

        public int Length { get; private set; }

        public int NextPosition => Length;

        public bool UsesCustomData => false;

        public void AddCustomData<TData>(string id, TData data)
        {
        }


        public void IncreaseIndent()
            => this.indenter.IncreaseIndent();

        public void DecreaseIndent()
            => this.indenter.DecreaseIndent();

        public void Write(string text, object color)
            => Write(text, null, DecompilerReferenceFlags.None, color);

        public void Write(string text, int index, int length, object color)
            => Write(text.Substring(index, length), null, DecompilerReferenceFlags.None, color);

        public void Write(string text, object reference, DecompilerReferenceFlags flags, object color)
        {
            if (!this.elementOpened)
            {
                LineStart();
            }

            if (this.addIndent)
            {
                var indent = indenter.String;
                if (indent.Length > 0)
                {
                    this.builder.AddContent(this.NextSequence++, this.indenter.String);
                }
                this.addIndent = false;
            }

            this.builder.OpenElement(this.NextSequence++, "span");
            var cssClass = ColorToCssClass(color);
            if (reference != null && this.eventReceiver != null)
            {
                var _eventReceiver = this.eventReceiver;
                var callback = EventCallback.Factory.Create<MouseEventArgs>(_eventReceiver, () =>
                {
                    _eventReceiver.ShowReference(reference, flags);
                });
                this.builder.AddAttribute<MouseEventArgs>(this.NextSequence++, "onclick", callback);
                if (string.IsNullOrEmpty(cssClass))
                {
                    cssClass = "reference";
                }
                else
                {
                    cssClass += " reference";
                }
            }
            if (!string.IsNullOrEmpty(cssClass))
            {
                this.builder.AddAttribute(this.NextSequence++, "class", cssClass);
            }
            this.builder.AddContent(this.NextSequence++, text);
            this.builder.CloseElement();

            Length += text.Length;
        }

        public void Write(string text, int index, int length, object reference, DecompilerReferenceFlags flags, object color)
        {
            Write(text.Substring(index, length), reference, flags, color);
        }

        public void WriteLine()
        {
            if (!this.elementOpened)
            {
                LineStart();
            }

            LineEnd();
            this.addIndent = true;

            Length += 1;
        }

        private string ColorToCssClass(object color)
        {
            if (color is TextColor c)
            {
                return ColorToCssClass(c);
            }

            return string.Empty;
        }

        private string ColorToCssClass(TextColor color)
        {
            switch (color)
            {
                case TextColor.Comment:
                    return "d-comment";
                case TextColor.Keyword:
                    return "d-keyword";
                case TextColor.String:
                case TextColor.VerbatimString:
                    return "d-string";
                case TextColor.Char:
                    return "d-char";
                case TextColor.Namespace:
                    return "d-namespace";
                case TextColor.Type:
                    return "d-type";
                case TextColor.SealedType:
                    return "d-sealedtype";
                case TextColor.StaticType:
                    return "d-statictype";
                case TextColor.Delegate:
                    return "d-delegate";
                case TextColor.Enum:
                    return "d-enum";
                case TextColor.Interface:
                    return "d-interface";
                case TextColor.ValueType:
                    return "d-valuetype";
                case TextColor.Module:
                    return "d-module";
                case TextColor.TypeGenericParameter:
                    return "d-typegenericparameter";
                case TextColor.MethodGenericParameter:
                    return "d-methodgenericparameter";
                case TextColor.InstanceMethod:
                    return "d-imethod";
                case TextColor.StaticMethod:
                    return "d-smethod";
                case TextColor.ExtensionMethod:
                    return "d-emethod";
                case TextColor.InstanceField:
                    return "d-ifield";
                case TextColor.EnumField:
                    return "d-efield";
                case TextColor.LiteralField:
                    return "d-lfield";
                case TextColor.StaticField:
                    return "d-sfield";
                case TextColor.InstanceEvent:
                    return "d-ievent";
                case TextColor.StaticEvent:
                    return "d-sevent";
                case TextColor.InstanceProperty:
                    return "d-iproperty";
                case TextColor.StaticProperty:
                    return "d-sproperty";
                case TextColor.Local:
                    return "d-local";
                case TextColor.Parameter:
                    return "d-parameter";
                case TextColor.Label:
                    return "d-label";
                case TextColor.OpCode:
                    return "d-opcode";
                case TextColor.Error:
                    return "d-error";
                default:
                    return string.Empty;
            }
        }

        private void LineStart()
        {
            this.builder.OpenElement(this.NextSequence++, "tr");
            this.builder.AddMarkupContent(this.NextSequence++, $"<td class=\"line-number\">{this.lineNumber++}</td>");
            this.builder.OpenElement(this.NextSequence++, "td");
            this.elementOpened = true;
        }

        private void LineEnd()
        {
            this.builder.AddContent(this.NextSequence++, "\n");
            // close td
            this.builder.CloseElement();
            // close tr
            this.builder.CloseElement();
            this.elementOpened = false;
        }

        public void Dispose()
        {
            if (this.elementOpened)
            {
                LineEnd();
            }
        }
    }
}
