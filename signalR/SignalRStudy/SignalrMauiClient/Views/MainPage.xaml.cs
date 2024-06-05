using SignalrMauiClient.ViewModels;

namespace SignalrMauiClient.Views
{
    public partial class MainPage : ContentPage
    {
        MainViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();

            viewModel = new MainViewModel();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await viewModel.Connect();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

            await viewModel.Disconnect();
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {

        }
    }

}
