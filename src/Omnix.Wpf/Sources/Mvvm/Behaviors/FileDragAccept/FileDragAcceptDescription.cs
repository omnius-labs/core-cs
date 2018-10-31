using System;
using System.Collections.Generic;
using System.Windows;

namespace Omnius.Wpf
{
    public class FileDragAcceptEventArgs : EventArgs
    {
        public IEnumerable<string> Paths { get; private set; }

        public FileDragAcceptEventArgs(IEnumerable<string> paths)
        {
            this.Paths = paths;
        }
    }

    public sealed class FileDragAcceptDescription
    {
        public DragDropEffects Effects { get; set; }

        public event Action<FileDragAcceptEventArgs> DragDrop;

        public void OnDrop(FileDragAcceptEventArgs fileDragAcceptEventArgs)
        {
            this.DragDrop?.Invoke(fileDragAcceptEventArgs);
        }
    }
}
