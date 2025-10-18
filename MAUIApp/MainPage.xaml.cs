using System;
using Microsoft.Maui.Controls;

namespace MAUIStudy;

/// <summary>
/// Code-behind for <see cref="MainPage"/> that handles the counter logic.
/// </summary>
public partial class MainPage : ContentPage
{
    // Backing field that stores the number of times the button has been clicked.
    private int _counter;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();

        // Ensure the counter label reflects the initial value when the page loads.
        UpdateCounterLabel();
    }

    /// <summary>
    /// Handles the click event from the button, increments the counter, and updates the UI.
    /// </summary>
    /// <param name="sender">The source of the click event.</param>
    /// <param name="e">Event data describing the click.</param>
    private void OnCounterClicked(object sender, EventArgs e)
    {
        _counter++;
        UpdateCounterLabel();
    }

    /// <summary>
    /// Updates the label that shows the current counter value.
    /// </summary>
    private void UpdateCounterLabel()
    {
        CounterLabel.Text = _counter.ToString();
    }
}
