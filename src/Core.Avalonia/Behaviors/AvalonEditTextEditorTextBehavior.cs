using Avalonia;
using Avalonia.Xaml.Interactivity;
using AvaloniaEdit;

namespace Core.Avalonia.Behaviors;

public class AvalonEditTextEditorTextBehavior : Behavior<TextEditor>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        var editor = this.AssociatedObject as TextEditor;
        if (editor == null) return;

        editor.TextChanged += this.TextChanged;
    }

    protected override void OnDetaching()
    {
        var editor = this.AssociatedObject as TextEditor;
        if (editor == null) return;

        editor.TextChanged -= this.TextChanged;

        base.OnDetaching();
    }

    public static readonly DirectProperty<AvalonEditTextEditorTextBehavior, string> TextProperty =
        AvaloniaProperty.RegisterDirect<AvalonEditTextEditorTextBehavior, string>(
            nameof(Text),
            o => o.Text,
            (o, v) => o.Text = v);

    private string _text = string.Empty;

    public string Text
    {
        get { return _text; }
        set
        {
            SetAndRaise(TextProperty, ref _text, value);

            var editor = this.AssociatedObject as TextEditor;
            if (editor == null) return;
            if (editor.Text == value) return;

            editor.Text = value;
        }
    }

    private void TextChanged(object? sender, EventArgs e)
    {
        var editor = this.AssociatedObject as TextEditor;
        if (editor == null) return;
        if (editor.Text == this.Text) return;

        this.Text = editor.Text;
    }
}
