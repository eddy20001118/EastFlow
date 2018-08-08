using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EastFlow.Properties;

namespace WindowsFormsApp1
{
    public partial class Form1 : MaterialForm
    {
        public static int N = 0;
        public static int[] colorTag = new int[64];
        public static int HowManyzero;
        string path3 = System.IO.Directory.GetCurrentDirectory() + @"\solver.exe";
        FileStream fs;
        FileStream res;
        StreamWriter sw;
        StreamReader sr;
        Process p;
        
        public Form1()
        {
            InitializeComponent();
            InitColorTag();
            AddPictureBox();
            Generate();
        }

        private void InitColorTag()
        {
            for (int i = 0; i < 64; i++)
            {
                colorTag[i] = 0;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue800, Primary.Blue800, Primary.Blue800, Accent.LightBlue200, TextShade.WHITE);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormClosing += Form1_FormClosing;
    }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (sr != null)
            {
                sr.Close();
                File.Delete("testdata.out");
            }
            sw.Close();
            fs.Close();
            File.Delete("solver.exe");
            File.Delete("testdata.in");
        }

        Button[] Btn = new Button[64];
        private void AddPictureBox()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int N = i * 8 + j;
                    Btn[N] = new Button()
                    {
                        Height = 55,//每个格子高度 宽度均为70
                        Width = 55,//
                        Top =i * 60,//起始坐标
                        Left =j * 60
                    };
                    Btn[N].BackColor = Color.FromArgb(255,245,245,245);
                    Btn[N].FlatStyle = FlatStyle.Flat;
                    Btn[N].FlatAppearance.BorderSize = 0;
                    this.panel1.Controls.Add(Btn[N]);
                    Btn[N].Click += delegate (object sender, EventArgs e)
                    {
                        Button btn = (Button)sender;
                        int M = N;
                        btn.BackgroundImage = Image.FromFile(@"C:\Users\yuhao\source\repos\WindowsFormsApp1\WindowsFormsApp1\Res\castle.png");
                        btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                        switch (colorTag[M])
                        {
                            case 0: btn.BackColor = Color.FromArgb(255,205,92,92); colorTag[M] = 1; break; //red
                            case 1: btn.BackColor = Color.FromArgb(255,46,139,87); colorTag[M] = 2; break; //green
                            case 2: btn.BackColor = Color.FromArgb(255,238,221,130); colorTag[M] = 3; break;  //yellow
                            case 3: btn.BackColor = Color.FromArgb(255,65,105,225); colorTag[M] = 4; break;
                            case 4:
                                btn.BackgroundImage = null; 
                                btn.BackColor = Color.FromArgb(255, 245, 245, 245);
                                btn.FlatStyle = FlatStyle.Flat;
                                btn.FlatAppearance.BorderSize = 0;
                                colorTag[M] = 0;
                                break;
                        }
                    };
                }
            }
        }

        private void Generate()
        {
            fs = new FileStream("testdata.in", FileMode.OpenOrCreate);
            sw = new StreamWriter(fs);
            Button generate = this.Gen;
            Button Reset = this.Res;
            generate.Click += delegate (object sender, EventArgs e)
            {
                fs.SetLength(0);
                HowManyzero = 0;
                for(int q = 0; q < 64; q++)
                {
                    if(colorTag[q] == 0)
                    {
                        HowManyzero++;
                    }
                }
                if (HowManyzero != 48)
                {
                    MessageBox.Show("输入错误");
                }
                else
                {
                    for (int n = 0; n < 8; n++)
                    {
                        for (int m = 0; m < 8; m++)
                        {
                            sw.Write(colorTag[n * 8 + m]);
                            sw.Write(" ");
                        }
                        sw.WriteLine();
                    }
                    sw.Flush();
                    res = new FileStream(path3, FileMode.Create);
                    res.Write(Resources.solver, 0, Resources.solver.Length);
                    res.Close();
                    p = new Process();
                    p.StartInfo.FileName = "solver.exe";
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    p.WaitForExit(15*1000);
                    if(p.HasExited == true)
                    {
                        Thread.Sleep(100);
                        ReadColor();
                        Stop_Process();
                    }
                    else
                    {
                        MessageBox.Show("无解");
                        p.CloseMainWindow();
                        Stop_Process();
                    }
                }
                
            };
            Reset.Click += delegate (object sender, EventArgs e)
            {
                fs.SetLength(0);
                InitColorTag();
                for(int N = 0; N < 64; N++)
                {
                    Btn[N].BackgroundImage = null;
                    Btn[N].BackColor = Color.FromArgb(255, 245, 245, 245);
                    Btn[N].FlatStyle = FlatStyle.Flat;
                    Btn[N].FlatAppearance.BorderSize = 0;
                }
            };
        }
        private void ReadColor()
        {
            sr = new StreamReader("testdata.out");
            String str_read = sr.ReadToEnd();
            int[] ReadColorTag = new int[64];
            sr.Close();
            str_read = str_read.Replace(" ", "");
            str_read = str_read.Replace("\r\n", "");
            if (str_read.Length != 64)
            {
                MessageBox.Show("无解");
            }
            else
            {
                for (int m = 0; m < 64; m++)
                {
                    int n = m;
                    int swit = Convert.ToInt32(str_read[n].ToString());
                    switch (swit)
                    {
                        default: Btn[n].BackColor = Color.FromArgb(255, 245, 245, 245); break; //default color
                        case 1: Btn[n].BackColor = Color.FromArgb(255, 205, 92, 92); break; //red
                        case 2: Btn[n].BackColor = Color.FromArgb(255, 46, 139, 87); break; //green
                        case 3: Btn[n].BackColor = Color.FromArgb(255, 238, 221, 130); break;  //yellow
                        case 4: Btn[n].BackColor = Color.FromArgb(255, 65, 105, 225); break;
                    }

                }
            }
            
        }
        private void Stop_Process()
        {
            Process[] ps = Process.GetProcesses();//获取计算机上所有进程
            foreach (Process p in ps)
            {
                if (p.ProcessName == "solver")//判断进程名称
                {
                    p.Kill();//停止进程
                }
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        
    }
}