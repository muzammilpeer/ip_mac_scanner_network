using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using Microsoft.Win32;

namespace ComputerNetworkProject
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private ArrayList arpusers = new ArrayList();
        private ArrayList arpip = new ArrayList();
        private ArrayList arpmac = new ArrayList();
        private ArrayList onlineip = new ArrayList();
        private ArrayList onlinemac = new ArrayList();
        private String[] onlinelist = new String[255];
        private string arr = null;
        private int pingsSent;
       // private string online = "Online:";
        private int timeout = 120;
        AutoResetEvent resetEvent = new AutoResetEvent(false);
       
        public Window1()
        {
            InitializeComponent();
        }
        private String[] run_arp_cmd()
        {
            int ExitCode;
            ProcessStartInfo ProcessInfo;
            Process Process;
            string Command = "arp -a";
            ProcessInfo = new ProcessStartInfo("cmd.exe", "/C " + Command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = false;
            ProcessInfo.RedirectStandardOutput = (true);
            Process = Process.Start(ProcessInfo);

            int Timeout = 500;
            Process.WaitForExit(Timeout);
            using (StreamReader reader = Process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                //////////////////Tokenizing the String////////////////////////////
                int len = result.Length;
                //listpc.Items.Add(result.Length);
                int cou = 0;
                int outer = 0;
                int major = 0;
                int cout = 0;
                string tmpstr = String.Empty;
                string innerstr = String.Empty;
                //listpc.Items.Add("Chech here");
                foreach (char c in result)
                {
                    tmpstr += c;
                    // tmpstr = tmpstr.Trim();
                    if (c == '\n')
                    {
                        //  tmpstr = tmpstr.Replace('\t', ' ');
                        tmpstr = tmpstr.Replace("Interface:", " ");
                        tmpstr = tmpstr.Replace("Physical", " ");
                        tmpstr = tmpstr.Replace("Internet", " ");
                        tmpstr = tmpstr.Replace("Type", " ");
                        tmpstr = tmpstr.Replace("static", " ");
                        tmpstr = tmpstr.Replace("dynamic", " ");
                        tmpstr = tmpstr.Replace("Address", " ");
                        if (tmpstr != String.Empty)
                        {
                            foreach (char t in tmpstr)
                            {
                                innerstr += t;
                                innerstr = innerstr.TrimEnd(' ');
                                innerstr = innerstr.Trim();
                                // innerstr = innerstr.Replace('-', ':');
                                if (t == ' ' && innerstr != String.Empty)
                                {
                                    if (innerstr != String.Empty)
                                    {
                                        major++;
                                        if (major > 3)
                                        {
                                            arpusers.Add(innerstr);
                                            onlinelist[cout++] = innerstr;
                                        }
                                        innerstr = String.Empty;
                                        outer = 0;
                                    }
                                }
                                else
                                {
                                    outer++;
                                }

                            }

                        }
                        tmpstr = String.Empty;
                        cou = 0;
                    }
                    else
                    {
                        cou++;
                    }
                }
            }
            ExitCode = Process.ExitCode;
            Process.Close();
            return onlinelist;
        }
        private void test_btn_Click(object sender, RoutedEventArgs e)
        {
            listpc.Items.Clear();
            arpusers.Clear();  // array list
            arpip.Clear(); // aray list
            arpmac.Clear(); // array list
            onlineip.Clear();
            onlinemac.Clear();
            pinging();
            arpscan();
           // button1.IsEnabled = true;
            checkBox1.Visibility = Visibility.Visible;

        }
        private void pinging()
        {
            if (toip.Text != String.Empty && fromip.Text != String.Empty)
            {
                pingsSent = 0;
                IPAddress check;
                if (IPAddress.TryParse(fromip.Text.ToString(), out check) == false || IPAddress.TryParse(toip.Text.ToString(), out check) == false)
                {
                    MessageBox.Show("Invalid Format or Invalid IP Address");
                    return;
                }
                int from = IpToInt32(fromip.Text.ToString());
                int to = IpToInt32(toip.Text.ToString());
                if (from < to)
                {
                    // Send the ping
                    for (int i = from; i <= to; i++)
                    {
                        SendPing(Int32ToIp(i));
                        string tmp = Int32ToIp(i);
                        arr = tmp;
                        BitmapImage loadedImage = new BitmapImage();
                        loadedImage.BeginInit();
                        loadedImage.UriSource = (new Uri("/icon/offline.png", UriKind.Relative));
                        loadedImage.EndInit();
                        Image bmp = new Image();
                        bmp.Width = 30;
                        bmp.Height = 30;
                        bmp.Source = loadedImage;

                        StackPanel st = new StackPanel();
                        st.Orientation = Orientation.Horizontal;
                        TextBlock txt = new TextBlock();
                        // We got a response, let's see the statistics

                        txt.Text = tmp;
                        //                        arpusers.Add(tmp);
                        st.Children.Add(bmp);
                        st.Children.Add(txt);
                        // listpc.Items.Add(st);

                    }
                }
                else if (from > to)
                {
                    MessageBox.Show("From Range Value is Greator than To Range Value,IP Addresses !");
                }
            }
            else
            {
                MessageBox.Show("Invalid IP Range ! or Missing Range Field(s).");
            }


        }
        private void arpscan()
        {
            arpip.Clear();
            arpmac.Clear();
            String[] onlines = run_arp_cmd();
            for (int i = 0; i < (arpusers.Count - 1); i++)
            {
                string ips = arpusers[i].ToString();
                string mac = arpusers[i + 1].ToString();
                arpip.Add(ips);
                arpmac.Add(mac);
                i++;
            }
            int from = IpToInt32(fromip.Text.ToString());
            int to = IpToInt32(toip.Text.ToString());
            for (int i = 0; i < (arpip.Count); i++)
            {
                int cur = IpToInt32(arpip[i].ToString());
                if (cur >= from && cur <= to)
                {
                    onlineip.Add(arpip[i]);
                    onlinemac.Add(arpmac[i]);
                    // listBox1.Items.Add(arpip[i] + "+" + arpmac[i]);
                }


            }
            usercount.Text = onlineip.Count.ToString();
            int l = 0;
            int k = 0;
            //   listpc.Items.Add(onlineip.Count);
            int maindiff = to - from;
            for (int i = from; i < to; i++)
            {
                int cur = IpToInt32(onlineip[k].ToString());
                if (checkBox1.IsChecked == true)
                {
                    if (cur == i)
                    {
                        l++;
                        BitmapImage loadedImage = new BitmapImage();
                        TextBlock update = new TextBlock();
                        update.Text = l.ToString();
                        loadedImage.BeginInit();
                        loadedImage.UriSource = (new Uri("/icon/online.png", UriKind.Relative));
                        loadedImage.EndInit();
                        Image bmp = new Image();
                        bmp.Width = 30;
                        bmp.Height = 30;
                        bmp.Source = loadedImage;

                        StackPanel st = new StackPanel();
                        st.Orientation = Orientation.Horizontal;
                        TextBlock txt = new TextBlock();
                        string ipshow = Int32ToIp(i);
                        txt.Text = ipshow.ToString() + "\t\t\t\t" + onlinemac[k].ToString();
                        st.Children.Add(update);
                        st.Children.Add(bmp);
                        st.Children.Add(txt);
                        listpc.Items.Add(st);
                        k++;
                        if (k >= Int32.Parse(onlineip.Count.ToString()))
                            k = 0;
                    }
                    else
                    {
                        BitmapImage loadedImage = new BitmapImage();
                        loadedImage.BeginInit();
                        loadedImage.UriSource = (new Uri("/icon/offline.png", UriKind.Relative));
                        loadedImage.EndInit();
                        Image bmp = new Image();
                        bmp.Width = 30;
                        bmp.Height = 30;
                        bmp.Source = loadedImage;
                        StackPanel st = new StackPanel();
                        st.Orientation = Orientation.Horizontal;
                        TextBlock txt = new TextBlock();
                        string ipshow = Int32ToIp(i);
                        txt.Text = ipshow.ToString();
                        st.Children.Add(bmp);
                        st.Children.Add(txt);
                        listpc.Items.Add(st);
                    }
                }
                else
                {
                    if (cur == i)
                    {
                        l++;
                        BitmapImage loadedImage = new BitmapImage();
                        TextBlock update = new TextBlock();
                        update.Text = l.ToString();
                        loadedImage.BeginInit();
                        loadedImage.UriSource = (new Uri("/icon/online.png", UriKind.Relative));
                        loadedImage.EndInit();
                        Image bmp = new Image();
                        bmp.Width = 30;
                        bmp.Height = 30;
                        bmp.Source = loadedImage;

                        StackPanel st = new StackPanel();
                        st.Orientation = Orientation.Horizontal;
                        TextBlock txt = new TextBlock();
                        string ipshow = Int32ToIp(i);
                        txt.Text = ipshow.ToString() + "\t\t\t\t" + onlinemac[k].ToString();
                        st.Children.Add(update);
                        st.Children.Add(bmp);
                        st.Children.Add(txt);
                        listpc.Items.Add(st);
                        k++;
                        if (k >= Int32.Parse(onlineip.Count.ToString()))
                            k = 0;
                    }
                }
            }
        }

        private int IpToInt32(string ipAddress)
        {
            return BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes().Reverse().ToArray(), 0);
        }

        private string Int32ToIp(int ipAddress)
        {
            return new IPAddress(BitConverter.GetBytes(ipAddress).Reverse().ToArray()).ToString();
        }


        private void SendPing(string ip)
        {
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();

            // Create an event handler for ping complete
            pingSender.PingCompleted += new PingCompletedEventHandler(pingSender_Complete);

            // Create a buffer of 32 bytes of data to be transmitted.
            byte[] packetData = Encoding.ASCII.GetBytes("................................");

            // Jump though 50 routing nodes tops, and don't fragment the packet
            PingOptions packetOptions = new PingOptions(50, true);

            // Send the ping asynchronously
            pingSender.SendAsync(ip, timeout, packetData, packetOptions, resetEvent);
            //              pingSender.SendAsync(txtIP.Text, 5000, packetData, packetOptions, resetEvent);

        }

        private void pingSender_Complete(object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                listpc.Items.Add("Ping was canceled...\r\n");

                // The main thread can resume
                ((AutoResetEvent)e.UserState).Set();
            }
            else if (e.Error != null)
            {
                listpc.Items.Add("An error occured: " + e.Error + "\r\n");

                // The main thread can resume
                ((AutoResetEvent)e.UserState).Set();
            }
            else
            {
                PingReply pingResponse = e.Reply;
                // Call the method that displays the ping results, and pass the information with it
                ShowPingResults(pingResponse);
            }
        }

        public void ShowPingResults(PingReply pingResponse)
        {
            if (pingResponse == null)
            {
                // We got no response
                listpc.Items.Add("There was no response.\r\n\r\n");
                return;
            }
            else if (pingResponse.Status == IPStatus.Success)
            {
            }
            else
            {
            }
            // Increase the counter so that we can keep track of the pings sent
            pingsSent++;
            // Send 1 pings
            if (pingsSent < 1)
            {
                SendPing(arr);
            }

        }

    
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            TcpClient tc = new TcpClient("127.0.0.1", 8090);
            NetworkStream ns = tc.GetStream();
            StreamWriter sw = new StreamWriter(ns);
            listpc.Items.Add("Sending Result to Server");
            for (int i = 0; i < onlineip.Count; i++)
            {
                sw.WriteLine(onlineip[i]);
                sw.Flush();
                sw.WriteLine(onlinemac[i]);
                sw.Flush();
                listpc.Items.Add(onlineip[i]);
            }
            sw.WriteLine("exit");
            sw.Close();
            ns.Close();
            button1.IsEnabled = false;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show("IP and Mac Address Scanner By \n \t Sharjeel Riaz \n \t & Muzammil Peer \n \n \t NU-FAST Karachi ");
            AboutBox1 abt = new AboutBox1();
            abt.ShowDialog();
           
            
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text File|*.txt|Log File|*.log|XML File|*.xml";
            saveFileDialog1.Title = "Save Report Log File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        {
                            System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog1.OpenFile());
                            if (onlineip.Count != 0)
                            {
                                for (int i = 0; i < onlineip.Count; i++)
                                {
                                    sw.WriteLine(onlineip[i] + " - " + onlinemac[i]);
                                }
                            }
                            else
                            {
                                MessageBox.Show("List Box is Empty");
                            }
                            sw.Close();
                        } break;
                    case 2:
                        {
                            System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog1.OpenFile());
                            if (onlineip.Count != 0)
                            {
                                for (int i = 0; i < onlineip.Count; i++)
                                {
                                    sw.Write(onlineip[i] + " - " + onlinemac[i] + "   ");
                                }
                            }
                            else
                            {
                                MessageBox.Show("List Box is Empty");
                            }
                            sw.Close();
                        } break;
                    case 3:
                        {
                            try
                            {
                                //pick whatever filename with .xml extension
                                string filepath = String.Empty; 
                                filepath = saveFileDialog1.FileName;
                                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                                try
                                {
                                    xmlDoc.Load(filepath);
                                }
                                catch (System.IO.FileNotFoundException)
                                {
                                    //if file is not found, create a new xml file
                                    System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(filepath, System.Text.Encoding.UTF8);
                                    xmlWriter.Formatting = System.Xml.Formatting.Indented;
                                    xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                                    xmlWriter.WriteStartElement("IpandMacAddressScanner");
                                    xmlWriter.Close();
                                    xmlDoc.Load(filepath);
                                }
                                string category = "IpAndMacAddresses";
                                string datastr = "IpAddress";
                                string datastr1 = "MacAddress";
                                //foreach (System.Object strObject in listpc.Items)
                               // {

                                for (int i = 0; i < onlineip.Count; i++)
                                {
                                    System.Xml.XmlNode root = xmlDoc.DocumentElement;
                                    System.Xml.XmlElement childNode = xmlDoc.CreateElement(category);
                                    System.Xml.XmlElement childNode2 = xmlDoc.CreateElement(datastr);
                                    System.Xml.XmlElement childNode3 = xmlDoc.CreateElement(datastr1);
                                    System.Xml.XmlText textNode = xmlDoc.CreateTextNode("IPAddress");
                                    textNode.Value = "First";
                                    System.Xml.XmlText macNode = xmlDoc.CreateTextNode("MacAddress");
                                    macNode.Value = "Second";

                                    root.AppendChild(childNode);
                                    childNode.AppendChild(childNode2);
                                    childNode.AppendChild(childNode3);
                                  
                                    childNode2.SetAttribute("Name", "IPAddress");
                                    childNode2.AppendChild(textNode);
                                    childNode3.AppendChild(macNode);
                                    childNode3.SetAttribute("Name", "MacAddress");

                                    textNode.Value = onlineip[i].ToString();
                                    macNode.Value =  onlinemac[i].ToString();
                                 }
                                    xmlDoc.Save(filepath);
                               
                            }
                            catch (System.Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }

                        } break;
                }
            }
        }

     
        private void MenuItem_Click_New(object sender, RoutedEventArgs e)
        {
            listpc.Items.Clear();
            fromip.Text = String.Empty;
            toip.Text = String.Empty;
            onlineip.Clear();
            onlinemac.Clear();
            arpip.Clear();
            arpmac.Clear();
            arpusers.Clear();

        }
    }
}
