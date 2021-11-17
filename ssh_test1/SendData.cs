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
        public int progressionInfo = 0;
        public int lengthNow = 0;
        public List<int> indexes = new List<int>();

        public SendData()
        {
        }

        public string sendCommandGetResponse(string command)
        {
            try
            {
                progressionInfo = 1;
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
                            progressionInfo = 2;
                            while (true)
                            {
                                string line = reader.ReadLine();
                                if (line != null)
                                {
                                    output = output + line + '\n';
                                    if (graphRequired == true)
                                    {
                                        getIndexes(line, index);
                                    }
                                    Thread.Sleep(10);
                                }
                                lengthNow = output.Length;
                                if (lengthNow == lengthBefore && line == null)
                                {
                                    if (graphRequired == true) 
                                    {
                                        indexes.Add(index);
                                    }
                                    break;
                                }
                                lengthBefore = lengthNow;
                                index++;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("exception: " + ex.ToString());
                            progressionInfo = 0;
                            return null;
                        }
                        shell.Close();
                    }
                    vclient.Disconnect();
                    progressionInfo = 0;
                    return output;
                }
            }
            catch
            {
                MessageBox.Show("An Error has occured, please check your connection");
                progressionInfo = 0;
                return null;
            }
        }

        private void getIndexes(string line, int index)
        {
            switch (line)
            {
                case string lineNow when line.Contains("load-distribution"):
                    indexes.Add(index);
                    break;
                case string lineNow when line.Contains("gain-allocation"):
                    indexes.Add(index);
                    break;
                case string lineNow when line.Contains("snr"):
                    indexes.Add(index);
                    break;
                case string lineNow when line.Contains("qln"):
                    indexes.Add(index);
                    break;
                case string lineNow when line.Contains("char-func-complex"):
                    indexes.Add(index);
                    break;
                case string lineNow when line.Contains("char-func-real"):
                    indexes.Add(index);
                    break;
                case string lineNow when line.Contains("tx-psd"):
                    indexes.Add(index);
                    break;
                default:
                    break;
            }
        }
    }
}
