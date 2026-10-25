using System;
using System.Collections.Generic;
using System.Text;

namespace BasicMvvm.Tests.Examples
{
    public class LoginVm : BaseVm
    {
        public LoginVm()
        {
            SubscribePropertyChanged(nameof(Password), () => OnPropertyChanged(nameof(PasswordMask)));
        }

        private string _username = "";
        public string Username { get => _username; set => SetProperty(ref _username, value); }

        private string _password = "";
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public string PasswordMask => new string('*', Password.Length);
    }
}
