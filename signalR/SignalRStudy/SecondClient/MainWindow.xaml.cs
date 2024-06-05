using Microsoft.AspNetCore.SignalR.Client;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecondClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HubConnection hubConnection;

        public MainWindow()
        {
            InitializeComponent();

            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7291/mychat")
                .WithAutomaticReconnect() // 자동으로 재연결
                .Build();

            hubConnection.On<string, string>("receive", (msg, user) =>
            {
                Dispatcher.Invoke(() => 
                {
                    string message = $"{user}의 메시지: {msg}";
                    chatLog.Items.Add(message);
                });
            });
            
            hubConnection.On<string>("notify", (msg) =>
            {
                Dispatcher.Invoke(() => 
                {
                    string message = $"{msg}";
                    chatLog.Items.Add(message);
                });
            });
        }

        private async void btnSend_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {            
            await hubConnection.InvokeAsync("Send", textBoxMessage.Text, textBoxUserId.Text);
        }
                

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                btnSend.IsEnabled = false;
                await hubConnection.StartAsync();

                chatLog.Items.Add("안녕하세요. 무엇을 도와 드릴까요?");
                btnSend.IsEnabled = true;
            }
            catch (Exception ex)
            {
                chatLog.Items.Add("서버와의 연결실패.");                
            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await hubConnection.InvokeAsync("Send", textBoxMessage.Text + "/" + textBoxUserId.Text, "");
            await hubConnection.StopAsync();
        }
    }
}