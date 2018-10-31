using System;
using System.Windows;

namespace Omnius.Wpf
{
    public class DragAcceptEventArgs : EventArgs
    {
        public IDragable Source { get; private set; }
        public IDropable Destination { get; private set; }

        public DragAcceptEventArgs(IDragable source, IDropable destination)
        {
            this.Source = source;
            this.Destination = destination;
        }
    }

    // http://b.starwing.net/?p=131
    public sealed class DragAcceptDescription
    {
        public DragDropEffects Effects { get; set; }
        public string Format { get; set; }

        public event Action<DragAcceptEventArgs> DragDrop;

        public void OnDrop(DragAcceptEventArgs dragAcceptEventArgs)
        {
            this.DragDrop?.Invoke(dragAcceptEventArgs);
        }
    }
}
