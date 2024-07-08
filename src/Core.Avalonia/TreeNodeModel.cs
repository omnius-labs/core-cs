using Avalonia.Collections;
using Avalonia.Threading;
using Omnius.Core.Pipelines;

// https://github.com/kekekeks/example-avalonia-huge-tree/blob/c77f1c32721dfa2ef8da1c65c0cce909b3b33eb2/AvaloniaHugeTree/TreeNodeModel.cs#L10

namespace Omnius.Core.Avalonia;

public interface IRootTreeNode
{
    void EnqueueUpdate();
}

public class RootTreeNodeModel : TreeNodeModel, IRootTreeNode
{
    private AvaloniaList<TreeNodeModel> _visibleChildren = new();
    private bool _updateEnqueued;

    public RootTreeNodeModel(Action<TreeNodeModel> isExpandedChangedCallback) : base(isExpandedChangedCallback)
    {
        _root = this;
    }

    public IAvaloniaReadOnlyList<TreeNodeModel> VisibleChildren => _visibleChildren;

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

        var newList = new List<TreeNodeModel>();
        AppendItems(newList, this);

        // 以下、ItemRepeaterの更新時のちらつきを抑えるため、複雑な差分更新処理となっている

        // 同一なら無視
        if (newList.SequenceEqual(_visibleChildren)) return;

        // 追加された要素を末尾に挿入
        var oldItems = _visibleChildren.ToHashSet();
        var addedItems = newList.Where(n => !oldItems.Contains(n)).ToArray();
        _visibleChildren.AddRange(addedItems);

        // ソート (破棄された項目は一番後ろに寄せられる)
        for (int i = 0; i < newList.Count; i++)
        {
            if (newList[i] == _visibleChildren[i]) continue;
            int oldIndex = _visibleChildren.IndexOf(newList[i]);
            _visibleChildren.Move(oldIndex, i);
        }

        // 末尾の不要な要素を削除
        int removeCount = _visibleChildren.Count - newList.Count;
        _visibleChildren.RemoveRange(_visibleChildren.Count - removeCount, removeCount);
    }

    private static void AppendItems(List<TreeNodeModel> list, TreeNodeModel node)
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
    private readonly Action<TreeNodeModel> _isExpandedChangedCallback;

    private string? _name;
    private object? _tag;
    private bool _isSelected;
    private bool _isExpanded;
    private List<TreeNodeModel> _children = new List<TreeNodeModel>();
    protected IRootTreeNode? _root;

    public TreeNodeModel(Action<TreeNodeModel> isExpandedChangedCallback)
    {
        _isExpandedChangedCallback = isExpandedChangedCallback;
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

    public bool IsSelected
    {
        get => _isSelected;
        set => this.SetProperty(ref _isSelected, value);
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            this.SetProperty(ref _isExpanded, value);
            _isExpandedChangedCallback(this);
            _root?.EnqueueUpdate();
        }
    }

    public IReadOnlyList<TreeNodeModel> Children => _children;

    public void AddChildren(IEnumerable<TreeNodeModel> children)
    {
        _children.AddRange(children);

        foreach (var child in _children)
        {
            child.SetRoot(_root, this.Level + 1);
        }

        _root?.EnqueueUpdate();
    }

    public void AddChild(TreeNodeModel child)
    {
        _children.Add(child);
        child.SetRoot(_root, this.Level + 1);
        _root?.EnqueueUpdate();
    }

    public void InsertChild(int index, TreeNodeModel child)
    {
        _children.Insert(index, child);
        child.SetRoot(_root, this.Level + 1);
        _root?.EnqueueUpdate();
    }

    public void RemoveChildAt(int index)
    {
        this.RemoveChild(_children[index]);
    }

    public void RemoveChild(TreeNodeModel child)
    {
        _children.Remove(child);
        child.SetRoot(null, 0);
        _root?.EnqueueUpdate();
    }

    public void ClearChildren()
    {
        foreach (var child in _children)
        {
            child.SetRoot(null, 0);
        }

        _children.Clear();
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
