using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.JSInterop;
using System;

namespace prototipoperguntasMaui.MauiPages;

public partial class MapPage : ContentPage
{
    public MapPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // No direct native->JS call here to avoid platform-specific WebView plumbing.
        // mapInterop has an internal render-check interval (set in init) to invalidate size when the container becomes visible.
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        try
        {
            // Navigate back in the native Shell stack
            await Microsoft.Maui.Controls.Shell.Current.GoToAsync("..", true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MapPage.OnBackClicked: {ex}");
        }
    }
}