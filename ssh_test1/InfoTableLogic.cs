using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssh_test1
{
    internal class InfoTableLogic
    {
        SendData send = new SendData();
        private List<PortData> portDatas = new List<PortData>();
        private string selectedPort;
        string commandOutput1;
        string commandOutput2;
        string commandOutput3;
        public InfoTableLogic(string selectedPort, string commandOutput1, string commandOutput2, string commandOutput3) 
        {
            this.selectedPort = selectedPort;
            this.commandOutput1 = commandOutput1;
            this.commandOutput2 = commandOutput2;
            this.commandOutput3 = commandOutput3;
            setPortData();
        }
        public List<PortData> getPortData()
        {
            return portDatas;
        }
        private void setPortData()
        {
            //send.progressionInfo = 3;
            string infoListData = commandOutput1 + commandOutput2 + commandOutput3;
            string[] output = infoListData.Replace(" : ", ":").Split(new String[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
            List<PortData> outputList = new List<PortData>();
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
                                    outputList.Add(new PortData { portInfo = infoListInput.Replace(":", " : ") });
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
                                outputList.Add(new PortData { advancedPortInfo = trimS });
                                break;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
            outputList.Add(new PortData { portInfo = noiseMarginInfo });
            outputList.Add(new PortData { portInfo = "output-power down/up: " + outputPowerInfoDown + "/" + outputPowerInfoUp });
            //send.progressionInfo = 0;
            portDatas = outputList;
        }
    }
}
