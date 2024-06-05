using SignalrMauiClient.ViewModels;

namespace SignalrMauiClient.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
        
    }

}
