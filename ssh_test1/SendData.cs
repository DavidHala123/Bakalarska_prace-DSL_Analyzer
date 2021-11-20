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
        public string midString = "";
        List<int> indexes = new List<int>();
        List<int> groupIndexes = new List<int>();
        List<string> messages = new List<string>();

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
                                    if(command.Contains("carrier-data"))
                                        FillInedex(line, index);
                                    Thread.Sleep(10);
                                }
                                lengthNow = output.Length;
                                if (lengthNow == lengthBefore && line == null)
                                {
                                    if (command.Contains("carrier-data"))
                                        indexes.Add(index);
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

        private void FillInedex(string line, int index)
        {
            switch (line)
            {
                case string lineNow when line.Contains("grp") || line.Contains("grop") || line.Contains("vect-qln")
                || line.Contains("rmc-carr") | line.Contains("gf"):
                    groupIndexes.Add(index);
                    break;
                case string lineNow when line.Contains("load-distribution") || line.Contains("gain-allocation")
                || line.Contains("snr") || line.Contains("qln") || line.Contains("char-func-complex")
                || line.Contains("char-func-real") || line.Contains("tx-psd"):
                    indexes.Add(index);
                    break;
                default:
                    break;
            }
        }

        public List<string> getLines(string DSLAMouput) 
        {
            indexes.Add(groupIndexes[0]);
            List<string> outLines = new List<string>();
            string[] splitOutput = DSLAMouput.Split('\n');
            string output = "";
            for (int i =0 ; i < indexes.Count-1; i++) 
            {
                for(int j = indexes[i]; j< indexes[i+1]; j++) 
                {
                    output += splitOutput[j].Trim();
                }
                outLines.Add(output);
                output = "";
            }
            indexes.Clear();
            return outLines;
        }
    }
}
