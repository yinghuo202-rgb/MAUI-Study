using Microsoft.Maui.Controls;
using CrossNGram.MAUI.ViewModels;

namespace CrossNGram.MAUI;
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }
}
