using System;

namespace Kani.Services
{
    public interface IDocumentHistoryService
    {
        event Action OnStateChange;

        bool CanBack { get; }
        bool CanForward { get; }

        void Back();
        void Forward();
    }
}
