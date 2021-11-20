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
        public bool graphRequired = false;
        public int lengthNow = 0;

        public SendData()
        {
        }

        public string sendCommandGetResponse(string command)
        {
            try
            {
                ConsoleLogic.ConsoleText = "1";
                using (var vclient = new SshClient(ConData.ipv4, ConData.name, ConData.password))
                {
                    string output = "";
                    int lengthBefore = -1;
                    int index = 0;
                    vclient.Connect();
                    using (ShellStream shell = vclient.CreateShellStream(ConData.name, 0, 0, 0, 0, 2048))
                    {
                        StreamReader reader = null;
                        try
                        {
                            StreamWriter writer = new StreamWriter(shell);
                            reader = new StreamReader(shell);
                            writer.AutoFlush = true;
                            writer.WriteLine(command + " | no-more");
                            while (shell.Length == 0)
                            {
                                Thread.Sleep(500);
                            }
                            ConsoleLogic.ConsoleText = "2";
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
                                index++;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("exception: " + ex.ToString());
                            ConsoleLogic.ConsoleText = "0";
                            return null;
                        }
                        shell.Close();
                    }
                    vclient.Disconnect();
                    ConsoleLogic.ConsoleText = "0";
                    return output;
                }
            }
            catch
            {
                MessageBox.Show("An Error has occured, please check your connection");
                ConsoleLogic.ConsoleText = "0";
                return null;
            }
        }
    }
}
