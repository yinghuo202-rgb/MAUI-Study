using CrossNGram.MAUI.ViewModels;
using Microsoft.Maui.Controls;

namespace CrossNGram.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }
}
