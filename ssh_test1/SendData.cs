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
                                    Thread.Sleep(10);
                                }
                                SplitDataForGraph(line, index);
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

        public List<PortData> FillInfoTable(string selectedPort, string commandOutput1, string commandOutput2, string commandOutput3)
        {
            progressionInfo = 3;
            string infoListData = commandOutput1 + commandOutput2 + commandOutput3;
            string[] output = infoListData.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            List<PortData> output1 = new List<PortData>();
            string noiseMarginInfo = "noise-margin down/up: ";
            string outputPowerInfoDown = "";
            string outputPowerInfoUp = "";
            int count = 0;
            foreach (string s in output)
            {
                string trimS = s.Trim();
                try
                {
                    if (char.IsLetter(trimS[0]) && trimS.Contains(":") && !trimS.Contains("ISAM"))
                    {
                        switch (trimS)
                        {
                            case string infoListInput when infoListInput.Contains("if-index"):
                                if (count == 0)
                                    output1.Add(new PortData { portInfo = infoListInput.Replace(":", " : ") });
                                count++;
                                break;

                            case string infoListInput when infoListInput.Contains("noise-margin"):
                                string[] noise = infoListInput.Split(':');
                                if (infoListInput.Contains("down"))
                                {
                                    noiseMarginInfo = noiseMarginInfo + noise[1].Trim();
                                }
                                if (infoListInput.Contains("up"))
                                {
                                    noiseMarginInfo = noiseMarginInfo + "/" + noise[1].Trim();
                                }
                                break;

                            case string infoListInput when infoListInput.Contains("output-power"):
                                string[] power = infoListInput.Split(':');
                                if (infoListInput.Contains("down"))
                                {
                                    outputPowerInfoDown = power[1].Trim();
                                }
                                if (infoListInput.Contains("up"))
                                {
                                    outputPowerInfoUp = power[1].Trim();
                                }
                                break;

                            default:
                                output1.Add(new PortData { advancedPortInfo = trimS });
                                break;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
            output1.Add(new PortData { portInfo = noiseMarginInfo });
            output1.Add(new PortData { portInfo = "output-power down/up: " + outputPowerInfoDown + "/" + outputPowerInfoUp });
            progressionInfo = 0;
            return output1;
        }

        public List<PortData> FillComboBox(string commandOnput)
        {
            progressionInfo = 3;
            List<PortData> output = new List<PortData>();
            using var sr = new StringReader(commandOnput);
            {
                string line = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        if (char.IsNumber(line[0]))
                        {
                            string[] info = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            if (info[2].Equals("down"))
                                output.Add(new PortData { portName = info[0], portState = @"Images\down.png" });
                            else
                                output.Add(new PortData { portName = info[0], portState = @"Images\up.png" });
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                progressionInfo = 0;
                return output;
            }
        }
        public string setConsoleText()
        {
            string output = "";
            switch (progressionInfo)
            {
                case 0:
                    output = "";
                    break;
                case 1:
                    output = "Sending request to DSLAM";
                    break;
                case 2:
                    output = "Reading response (" + lengthNow.ToString() + " symbols)";
                    break;
                case 3:
                    output = "Processing data";
                    break;

            }
            return output;
        }

        public void SplitDataForGraph(string line, int index)
        {
            if (graphRequired == true)
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
        public string SelectData(int startIndex, int stopIndex, string outputOfDSLAM)
        {
            String[] outputSplit = outputOfDSLAM.Split('\n');
            string output = "";
            for (int i = startIndex; i < stopIndex; i++)
            {
                output += outputSplit[i] + '\n';
            }
            return output;
        }
        public List<int> getGraph(string input) 
        {
            List<int> listOfDecValues = new List<int>();
            string[] outputSPlit = input.Split(':');
            int decValueBefore = -1;
            string outp = "";
            foreach (string j in outputSPlit)
            {
                try
                {
                    int decValue = Int32.Parse(j, System.Globalization.NumberStyles.HexNumber);
                    if(decValue != decValueBefore)
                    listOfDecValues.Add(decValue);
                }
                catch 
                {
                    continue;
                }
            }
            foreach(int k in listOfDecValues) 
            {
                outp = outp + k.ToString() + ",";
            }
            return listOfDecValues;
        }
    }
}
