namespace StreamDeckWidgetApp.Abstractions;

/// <summary>
/// Calculates window dimensions based on grid configuration.
/// </summary>
public interface IWindowSizingService
{
    /// <summary>
    /// Calculates the required window width and height for the given grid configuration.
    /// </summary>
    /// <param name="rows">Number of rows in the grid.</param>
    /// <param name="columns">Number of columns in the grid.</param>
    /// <param name="buttonSize">Size of each button in pixels.</param>
    /// <returns>A tuple containing (Width, Height) in pixels.</returns>
    (double Width, double Height) CalculateWindowSize(int rows, int columns, int buttonSize);
}
