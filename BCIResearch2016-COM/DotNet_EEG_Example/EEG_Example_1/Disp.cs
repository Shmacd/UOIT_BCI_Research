using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace EEG 
{
    public partial class Disp : Form 
    {
        //private System.Timers.Timer tm;
        SerialPort sPort = new SerialPort("COM10", 9600, Parity.None, 8, StopBits.One);

        //CHANGE FOR DISPLAY TIME
        int time = 10;

        int selector;
        int passCTR;

        private Timer tm;

        List<String> passwords = new List<String>();

        // generate a number between min and max, not currently used
        public int rng(int min, int max) 
        {
            Random rnd = new Random();
            int c = rnd.Next(min, max);
            return c;
        }

        //Function to read passwords from file
        public List<String> read(String filename, int a) 
        {
            List<String> pws = new List<String>();
            string line;
            int counter = 0;
            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null) 
            {
                pws.Add(line);
                counter++;
            }
            file.Close();
            return pws;
        }

        //Function to write used passwords to new file
        public void write(String line, String filename)
        {
            string path = filename + ".passwords";
            // This text is added only once to the file.
            if (!File.Exists(path)) 
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path)) {}
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path)) 
            {
                sw.WriteLine(line);
            }

            // Open the file to read from.
            using (StreamReader sr = File.OpenText(path)) 
            {
                string s = "";
                while ((s = sr.ReadLine()) != null) 
                {
                    Console.WriteLine(s);
                }
            }
        }

  
        private void writee(byte f)
        {
            byte[] data = new byte[1] { f };

            if (sPort.IsOpen) 
            {
                sPort.Write(data, 0, 1);
            }

        }

        public Disp(int ezorhard, string fn, int counter) 
        {
            //Open the serial port
            sPort.Open();

            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            passCTR = counter;
            selector = ezorhard;

            if (ezorhard == 0) 
            {
                passwords = read("easy_passwords.txt", 1);
                String s = passwords[counter];
                label1.Text = s;
                write(s, fn);
                writee(1);
                selector = 1;
            } 
            else if (ezorhard == 1) 
            {
                passwords = read("hard_spell_passwords.txt", 2);
                String s = passwords[counter];
                label1.Text = s;
                write(s, fn);
                writee(2);
                selector = 0;
                passCTR++;
            } 
        }

        /*
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
        }
        */

        private void Disp_Load(object sender, EventArgs e) 
        {
            tm = new Timer();
            tm.Interval = time * 1000;
            tm.Tick += new EventHandler(tm_Tick);
            tm.Start();         
        }

        private void tm_Tick(object sender, EventArgs e) 
        {
            tm.Stop(); // so that we only fire the timer message once

            //Form2 frm = new Form2();
            //frm.Show();
            //Application.Exit();
            writee(99);

            sPort.Close();
            Test t = new Test(passCTR, selector);
            t.Show();
            this.Hide();
        }
    }
}
