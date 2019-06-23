using System;
using System.Collections.Generic;
using Kani.Models;

namespace Kani.Services
{
    public class DocumentHistoryService : IDocumentHistoryService
    {
        public bool CanBack => this.index > 0;

        public bool CanForward => this.index + 1 < count;

        public event Action OnStateChange;

        private IDocumentService documentService;
        private List<IDocument> histroy = new List<IDocument>();
        private int index = -1;
        private int count = 0;
        private bool ignore;

        public DocumentHistoryService(IDocumentService documentService)
        {
            this.documentService = documentService;
            documentService.OnShown += this.OnDocumentShown;
        }

        public void Back()
        {
            if (!this.CanBack) return;
            try
            {
                this.ignore = true;
                this.index--;
                var doc = this.histroy[this.index];
                this.documentService.Show(doc);
            }
            finally
            {
                this.ignore = false;
            }
            this.OnStateChange?.Invoke();
        }

        public void Forward()
        {
            if (!this.CanForward) return;
            try
            {
                this.ignore = true;
                this.index++;
                var doc = this.histroy[this.index];
                this.documentService.Show(doc);
            }
            finally
            {
                this.ignore = false;
            }
            this.OnStateChange?.Invoke();
        }

        private void OnDocumentShown(IDocument document)
        {
            if (this.ignore) return;

            var nextIndex = this.index + 1;
            var nextCount = nextIndex + 1;

            if (nextCount > this.histroy.Count)
            {
                this.histroy.Add(document);
            }
            else
            {
                this.histroy[nextIndex] = document;
            }

            this.index = nextIndex;
            this.count = nextCount;
            this.OnStateChange?.Invoke();
        }
    }
}
