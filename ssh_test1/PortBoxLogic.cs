using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ssh_test1
{
    internal class PortBoxLogic
    {
        SendData send = new SendData();
        private List<PortData> portDatasCombo = new List<PortData>();
        private string commandOutput;
        public PortBoxLogic(string commandOutput) 
        {
            this.commandOutput = commandOutput;
            setPortDataCombo();
        }
        public List<PortData> getPortDataCombo() 
        {
            return portDatasCombo;
        }
        private void setPortDataCombo() 
        {
            send.progressionInfo = 3;
            List<PortData> output = new List<PortData>();
            using var sr = new StringReader(commandOutput);
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
                send.progressionInfo = 0;
                portDatasCombo = output;
            }
        }
        //public List<PortData> FillComboBox(string commandOnput)
        //{
        //    send.progressionInfo = 3;
        //    List<PortData> output = new List<PortData>();
        //    using var sr = new StringReader(commandOnput);
        //    {
        //        string line = sr.ReadLine();
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            try
        //            {
        //                if (char.IsNumber(line[0]))
        //                {
        //                    string[] info = line.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        //                    if (info[2].Equals("down"))
        //                        output.Add(new PortData { portName = info[0], portState = @"Images\down.png" });
        //                    else
        //                        output.Add(new PortData { portName = info[0], portState = @"Images\up.png" });
        //                }
        //            }
        //            catch
        //            {
        //                continue;
        //            }
        //        }
        //        send.progressionInfo = 0;
        //        return output;
        //    }
        //}
    }
}
