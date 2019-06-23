using System;
using Kani.Models;

namespace Kani.Services
{
    public class DocumentService : IDocumentService
    {
        private IDocument currentDocument;
        public event Action<IDocument> OnShown;

        public void Show(IDocument doc)
        {
            if (this.currentDocument?.Source == doc.Source)
            {
                return;
            }

            this.currentDocument = doc;
            this.OnShown?.Invoke(doc);
        }
    }
}
