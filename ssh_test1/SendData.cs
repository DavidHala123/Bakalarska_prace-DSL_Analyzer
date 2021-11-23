using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace ssh_test1
{
    class SendData
    {
        public int lengthNow = 0;
        private string output;

        public SendData(string dslamInput)
        {
            sendCommandGetResponse(dslamInput);
        }

        public string getResponse()
        {
            return output;
        }

        private void sendCommandGetResponse(string command)
        {
            try
            {
                ConsoleLogic.ConsoleText = "1";
                using (var vclient = new SshClient(ConData.ipv4, ConData.name, ConData.password))
                {
                    string dslamResponse = "";
                    vclient.Connect();
                    using (ShellStream shell = vclient.CreateShellStream(ConData.name, 0, 0, 0, 0, 2048))
                    {
                        StreamWriter writer = new StreamWriter(shell);
                        writer.AutoFlush = true;
                        writer.WriteLine(command + " | no-more");
                        while (shell.Length == 0)
                        {
                            Thread.Sleep(500);
                        }
                        GetFullResponse(ref dslamResponse, shell);
                        shell.Close();
                    }
                    vclient.Disconnect();
                    ConsoleLogic.ConsoleText = "0";
                    output = dslamResponse;
                }
            }
            catch
            {
                MessageBox.Show("An Error has occured, please check your connection");
                ConsoleLogic.ConsoleText = "0";
                output = null;
            }
        }

        private void GetFullResponse(ref string output, ShellStream shell)
        {
            ConsoleLogic.ConsoleText = "2";
            StreamReader reader = new StreamReader(shell);
            int lengthBefore = -1;
            while (true)
            {
                string line = reader.ReadLine();
                if (line != null)
                {
                    output = output + line + '\n';
                    Thread.Sleep(10);
                }
                lengthNow = output.Length;
                if (lengthNow == lengthBefore && line == null)
                {
                    break;
                }
                lengthBefore = lengthNow;

            }
        }
    }
}
