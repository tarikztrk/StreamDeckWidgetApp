using System.Windows;
using System.Windows.Input;

namespace StreamDeckWidgetApp.Core.Behaviors;

/// <summary>
/// Attached behavior that enables window dragging when clicking on the window
/// (excluding interactive controls like buttons, textboxes, etc.)
/// </summary>
public static class WindowDragBehavior
{
    public static readonly DependencyProperty EnableDragProperty =
        DependencyProperty.RegisterAttached(
            "EnableDrag",
            typeof(bool),
            typeof(WindowDragBehavior),
            new PropertyMetadata(false, OnEnableDragChanged));

    public static bool GetEnableDrag(DependencyObject obj)
    {
        return (bool)obj.GetValue(EnableDragProperty);
    }

    public static void SetEnableDrag(DependencyObject obj, bool value)
    {
        obj.SetValue(EnableDragProperty, value);
    }

    private static void OnEnableDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Window window)
        {
            if ((bool)e.NewValue)
            {
                window.MouseDown += Window_MouseDown;
            }
            else
            {
                window.MouseDown -= Window_MouseDown;
            }
        }
    }

    private static void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || sender is not Window window)
            return;

        // Don't start dragging when clicking interactive controls
        var src = e.OriginalSource as DependencyObject;
        while (src != null)
        {
            // Check all interactive control types
            if (src is System.Windows.Controls.Button ||
                src is System.Windows.Controls.ComboBox ||
                src is System.Windows.Controls.TextBox ||
                src is System.Windows.Controls.Primitives.ToggleButton ||
                src is System.Windows.Controls.Primitives.Popup)
                return;

            // Also check if we're in a Border that's part of a Button template
            if (src is System.Windows.Controls.Border border)
            {
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(border);
                if (parent is System.Windows.Controls.Button)
                    return;
            }

            src = System.Windows.Media.VisualTreeHelper.GetParent(src);
        }

        try
        {
            window.DragMove();
        }
        catch
        {
            // Ignore drag errors when clicking rapidly
        }
    }
}
