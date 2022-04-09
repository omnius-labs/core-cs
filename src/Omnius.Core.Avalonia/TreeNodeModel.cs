using Avalonia.Collections;
using Avalonia.Threading;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Avalonia;

// https://github.com/kekekeks/example-avalonia-huge-tree/blob/c77f1c32721dfa2ef8da1c65c0cce909b3b33eb2/AvaloniaHugeTree/TreeNodeModel.cs#L10

public interface IRootTreeNode
{
    void EnqueueUpdate();
}

public class RootTreeNodeModel : TreeNodeModel, IRootTreeNode
{
    private AvaloniaList<TreeNodeModel> _visibleChildren = new();
    private bool _updateEnqueued;

    public RootTreeNodeModel(IActionCaller<TreeNodeModel> isExpandedChangedActionCaller) : base(isExpandedChangedActionCaller)
    {
        _root = this;
    }

    public event Action<TreeNodeModel> IsExpandedChanged = (_) => { };

    public IAvaloniaReadOnlyList<TreeNodeModel> VisibleChildren => _visibleChildren;

    internal void OnIsExpandedChanged(TreeNodeModel node)
    {
        this.IsExpandedChanged?.Invoke(node);
    }

    public void EnqueueUpdate()
    {
        if (!_updateEnqueued)
        {
            _updateEnqueued = true;
            Dispatcher.UIThread.Post(this.Update, DispatcherPriority.Background);
        }
    }

    public void Update()
    {
        _updateEnqueued = false;
        var list = new AvaloniaList<TreeNodeModel>();
        AppendItems(list, this);

        _visibleChildren = new AvaloniaList<TreeNodeModel>(list);
        this.RaisePropertyChanged(nameof(VisibleChildren));
    }

    private static void AppendItems(AvaloniaList<TreeNodeModel> list, TreeNodeModel node)
    {
        list.Add(node);

        if (node.IsExpanded)
        {
            foreach (var child in node.Children)
            {
                AppendItems(list, child);
            }
        }
    }
}

public class TreeNodeModel : BindableBase
{
    private readonly IActionCaller<TreeNodeModel> _isExpandedChangedActionCaller;

    private string? _name;
    private object? _tag;
    private bool _isExpanded;
    private List<TreeNodeModel> _children = new List<TreeNodeModel>();
    protected IRootTreeNode? _root;

    public TreeNodeModel(IActionCaller<TreeNodeModel> isExpandedChangedActionCaller)
    {
        _isExpandedChangedActionCaller = isExpandedChangedActionCaller;
    }

    public int Level { get; private set; }

    public string? Name
    {
        get => _name;
        set => this.SetProperty(ref _name, value);
    }

    public object? Tag
    {
        get => _tag;
        set => this.SetProperty(ref _tag, value);
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            this.SetProperty(ref _isExpanded, value);
            _isExpandedChangedActionCaller.Call(this);
            _root?.EnqueueUpdate();
        }
    }

    public IReadOnlyList<TreeNodeModel> Children => _children;

    public void AddChild(TreeNodeModel child)
    {
        InsertChild(_children.Count, child);
    }

    public void InsertChild(int index, TreeNodeModel child)
    {
        if (child._root != null) throw new InvalidOperationException();
        _children.Insert(index, child);
        child.SetRoot(_root, this.Level + 1);
        _root?.EnqueueUpdate();
    }

    public void RemoveChild(TreeNodeModel child)
    {
        var index = _children.IndexOf(child);
        if (index != -1) RemoveChildAt(index);
    }

    public void RemoveChildAt(int index)
    {
        var child = _children[index];
        _children.RemoveAt(index);
        child.SetRoot(null, 0);
        _root?.EnqueueUpdate();
    }

    protected void SetRoot(IRootTreeNode? root, int level)
    {
        _root = root;
        this.Level = root == null ? -1 : level;

        foreach (var child in _children)
        {
            child.SetRoot(root, level + 1);
        }
    }
}
