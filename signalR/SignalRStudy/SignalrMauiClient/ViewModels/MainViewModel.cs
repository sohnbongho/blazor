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
        public ObservableCollection<UserData> userDatas { get; set; } = new ObservableCollection<UserData>();


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


        public ICommand SendCommand { get; set; } = null!;

        public MainViewModel()
        {
            SendCommand = new Command(async () => await SendMsg(), () => IsConnected );
        }

        public async Task SendMsg()
        {

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

        public record UserData(string ListViewUserId, string ListViewMsg);        

    }
}
