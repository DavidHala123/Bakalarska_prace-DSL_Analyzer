using Renci.SshNet;
using System.Windows;

namespace ssh_test1
{
    class Connect
    {
        public Connect()
        {
        }

        public bool con(bool isCheck)
        {
            AuthenticationMethod method = new PasswordAuthenticationMethod(ConData.name, ConData.password);
            ConnectionInfo connection = new ConnectionInfo(ConData.ipv4, ConData.name, method);
            var client = new SshClient(connection);
            try
            {
                client.Connect();
            }
            catch
            {
                if (!isCheck)
                    MessageBox.Show("An error has occured please check your connection data");
                return false;
            }
            if (client.IsConnected)
            {
                if (!isCheck)
                    MessageBox.Show("Connection has been successful");
                client.Disconnect();
                return true;
            }
            else
                return false;
        }
    }
}
