using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SignalrMauiClient.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public string? UserId { get; set; }
        public string? Message { get; set; }
        public ObservableCollection<UserData> MsgCollection { get; set; } = new ObservableCollection<UserData>();


        private bool isWorking;

        public bool IsWorking
        {
            get => isWorking; 
            set
            {
                isWorking = value;
                NotifyPropertyChanged();
            }
        }
        
        private bool isConnected;

        public bool IsConnected
        {
            get => isConnected; 
            set
            {
                isConnected = value;
                NotifyPropertyChanged();
            }
        }


        public ICommand SendCommand { get; set; }
        private HubConnection hubConnection { get; set; }

        public MainViewModel()
        {
            SendCommand = new Command(async () => await SendMsg(), () => IsConnected );
            hubConnection = new HubConnectionBuilder()                
                .WithUrl("http://58.181.61.21:8080/mychat")                
                .Build();
            //.WithUrl("https://localhost:7291/mychat")
            //.WithAutomaticReconnect() // 자동으로 재연결

            hubConnection.On<string, string>("receive", (msg, user) =>
            {                
                AddMsg(user, msg);
            });

            hubConnection.On<string>("notify", (msg) =>
            {
                //Dispatcher.Invoke(() =>
                //{
                //    string message = $"{msg}";
                //    chatLog.Items.Add(message);
                //});
                AddMsg("", msg);
            });
            hubConnection.Closed += async (error) =>
            {
                IsConnected = false;
                await Task.Delay(4000);
                await Connect();
            };
        }

        public async Task SendMsg()
        {
            try
            {                
                IsWorking = true;
                await hubConnection.InvokeAsync("Send", Message, UserId);
            }
            catch(Exception)
            {
                throw;
            }            
            IsWorking = false;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async Task Connect()
        {
            try
            {                
                await hubConnection.StartAsync();

                //chatLog.Items.Add("안녕하세요. 무엇을 도와 드릴까요?");
                AddMsg("", "안녕하세요. 무엇을 도와 드릴까요?");
                IsConnected = true;
            }
            catch (Exception)
            {
                //chatLog.Items.Add("서버와의 연결실패.");
            }
        }

        public void AddMsg(string userId, string msg)
        {
            MsgCollection.Add(new UserData(userId, msg));
        }

        public async Task Disconnect()
        {
            await hubConnection.InvokeAsync("Send", Message + "/" + UserId, "");
            await hubConnection.StopAsync();
            IsConnected = false;
        }

        public record UserData(string ListViewUserId, string ListViewMsg);        

    }
}
