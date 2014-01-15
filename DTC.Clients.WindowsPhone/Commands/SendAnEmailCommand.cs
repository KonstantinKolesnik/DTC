using Microsoft.Phone.Tasks;
using System;
using System.Windows.Input;

namespace DTC.Clients.WindowsPhone.Commands
{
    public class SendAnEmailCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            EmailComposeTask emailTask = new EmailComposeTask();
            emailTask.To = "info@company.com";
            emailTask.Show();
        }
    }
}
