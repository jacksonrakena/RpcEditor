using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RpcEditor.ViewModels;

namespace RpcEditor.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        ((MainViewModel)DataContext).Connect();
    }

    private void Update(object? sender, RoutedEventArgs e)
    {
        ((MainViewModel)DataContext).Update();
    }
}