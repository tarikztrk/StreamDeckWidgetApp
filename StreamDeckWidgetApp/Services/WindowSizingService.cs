using StreamDeckWidgetApp.Abstractions;

namespace StreamDeckWidgetApp.Services;

/// <summary>
/// Calculates window dimensions based on grid configuration.
/// Implements Single Responsibility Principle by extracting sizing logic from View.
/// </summary>
public class WindowSizingService : IWindowSizingService
{
    private const int ButtonMargin = 2; // 1px margin on each side
    private const int TitleBarHeight = 32;
    private const int WindowPadding = 2; // Grid margin in MainWindow

    public (double Width, double Height) CalculateWindowSize(int rows, int columns, int buttonSize)
    {
        // Calculate total button area including margins
        double totalButtonWidth = (buttonSize + ButtonMargin) * columns;
        double totalButtonHeight = (buttonSize + ButtonMargin) * rows;

        // Add window padding and title bar
        double width = totalButtonWidth + (WindowPadding * 2);
        double height = totalButtonHeight + TitleBarHeight + (WindowPadding * 2);

        return (width, height);
    }
}
