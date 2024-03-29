﻿using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace DSL_Analyzer
{
    //SENDS REQUEST AND GETS RESPONSE
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

        //OPENING NEW SSH CONNECTION AND SENDING REQUEST
        private void sendCommandGetResponse(string command)
        {
            try
            {
                ConsoleUC.ConsoleText = "1";
                using (var vclient = new SshClient(ConData.ipv4, ConData.name, ConData.password))
                {
                    string dslamResponse = "";
                    vclient.Connect();
                    using (ShellStream shell = vclient.CreateShellStream(ConData.name, 0, 0, 0, 0, 2048))
                    {
                        StreamWriter writer = new StreamWriter(shell);
                        writer.AutoFlush = true;
                        writer.WriteLine(command + " | count words | no-more");
                        while (shell.Length == 0)
                        {
                            Thread.Sleep(500);
                        }
                        GetFullResponse(ref dslamResponse, shell);
                        shell.Close();
                    }
                    vclient.Disconnect();
                    ConsoleUC.ConsoleText = "0";
                    output = dslamResponse;
                }
            }
            catch
            {
                MessageBox.Show("An Error has occured, please check your connection");
                ConsoleUC.ConsoleText = "0";
                output = null;
            }
        }

        //GETS RESPONSE, READING UNTIL WORDS COUNT FUNCTION PRIOVIDED BY DSLAM
        private void GetFullResponse(ref string output, ShellStream shell)
        {
            ConsoleUC.ConsoleText = "2";
            StreamReader reader = new StreamReader(shell);
            while (true)
            {
                string line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    if (line.Contains("words") && !line.Contains("|"))
                    {
                        break;
                    }
                    output = output + line + '\n';
                }
            }
        }
    }
}
