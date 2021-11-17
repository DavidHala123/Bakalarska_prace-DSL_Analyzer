using Renci.SshNet;

namespace ssh_test1
{
    class Connect
    {
        string ipv4;
        string name;
        string password;
        MainWindow main;
        public bool connected = false;
        public Connect(string ipv4, string name, string password)
        {
            this.ipv4 = ipv4;
            this.name = name;
            this.password = password;
            main = new MainWindow();
        }
        public bool con()
        {
            AuthenticationMethod method = new PasswordAuthenticationMethod(name, password);
            ConnectionInfo connection = new ConnectionInfo(ipv4, name, method);
            var client = new SshClient(connection);
            try
            {
                client.Connect();
            }
            catch
            {
                main.ErrorMessage("An error has occured please check your connection data");
                return false;
            }
            if (client.IsConnected)
            {
                main.ErrorMessage("Connection has been successful");
                client.Disconnect();
                return true;
            }
            else
                return false;
        }
    }
}
