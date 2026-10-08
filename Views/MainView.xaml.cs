using HeatMapApp.ViewModels;

namespace HeatMapApp.Views;

/// <summary>Code-behind for the main heat-map page.</summary>
public partial class MainView : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <inheritdoc/>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await _viewModel.InitializeAsync();
        }
        catch (PermissionException ex)
        {
            await DisplayAlert("Permission required", ex.Message, "OK");
        }
    }
}