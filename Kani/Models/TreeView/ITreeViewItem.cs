using System.Collections.Generic;

namespace Kani.Models.TreeView
{
    public interface ITreeViewItem
    {
        IReadOnlyList<TextRun> Texts { get; }
        bool IsLeaf { get; }

        void Select();
        IEnumerable<ITreeViewItem> EnumerateChildren();
    }
}
