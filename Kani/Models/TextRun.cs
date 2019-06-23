namespace Kani.Models
{
    public struct TextRun
    {
        public readonly string Text;
        public readonly string CssClass;

        public TextRun(string text) : this(text, string.Empty)
        {
        }

        public TextRun(string text, string cssClass)
            => (this.Text, this.CssClass) = (text, cssClass);
    }
}
