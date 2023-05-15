using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;






namespace LW4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button3.FlatAppearance.BorderSize = 0;
            button4.FlatAppearance.BorderSize = 0;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value % 2 == 0) { numericUpDown1.Value += 1; }
            if (numericUpDown1.Value < 3) { numericUpDown1.Value = 3; }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value % 2 == 0) { numericUpDown2.Value += 1; }
            if (numericUpDown2.Value < 3 ) { numericUpDown2.Value = 3; }
        }

        private Bitmap start;
        private Bitmap finish;
        int ty = -1;
        //int w = 0;
        //int h = 0;
        float[] gis = new float[256];

        private void button1_Click(object sender, EventArgs e)
        {
            var opd = new OpenFileDialog();// Создает окно открытия файла
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//Размещает по всей ширине объекта
            if (opd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opd.FileName);
                pictureBox1.BackColor = Color.Transparent;
            }
            var NImg = (Bitmap)pictureBox1.Image;
            int w = NImg.Width;
            int h = NImg.Height;
            start = new Bitmap(w, h);
            finish = new Bitmap(w, h);
            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    var pix = NImg.GetPixel(j, i);
                    start.SetPixel(j, i, pix);

                }
            }
            dataGridView1.Visible = true;
            pictureBox1.Visible = true;
            button1.Visible = false;
            button2.Visible = true;
            numericUpDown3.Visible = true;
            numericUpDown4.Visible = true;
            numericUpDown5.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            label7.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            button4.Visible = false;
            numericUpDown1.Visible = true;
            numericUpDown2.Visible = true;
            numericUpDown1.Value=3;
            numericUpDown2.Value = 3;
            label1.Visible = true;
            label2.Visible = true;
            button3.Visible = true;
            numericUpDown3.Visible = false;
            numericUpDown4.Visible = false;
            numericUpDown5.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ty = 1;
            button2.Visible = true;
            button4.Visible = true;
            numericUpDown3.Visible = true;
            numericUpDown4.Visible = true;
            numericUpDown5.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            label7.Visible = true;
            label6.Text = "";
            numericUpDown1.Visible = false;
            numericUpDown2.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            button3.Visible = false;
            int height = (int)numericUpDown2.Value;
            int width = (int)numericUpDown1.Value;
            createDataGridView(height, width);
            

        }
        private void button4_Click(object sender, EventArgs e)
        {
            if(numericUpDown3.Value!=0 && numericUpDown4.Value!=0)
            {
                int r=(int)numericUpDown3.Value;
                int sig=(int)numericUpDown4.Value;
                gaus(r, sig);

                numericUpDown3.Value = 0;
                numericUpDown4.Value = 0;


            }
            else
            {
                if (numericUpDown5.Value!=1)
                {
                    if (numericUpDown5.Value<3) { numericUpDown5.Value = 3; }
                    int a=(int)(numericUpDown5.Value);
                    mediana(a);
                    numericUpDown5.Value = 1;
                }
                else
                {
                    int height = (int)numericUpDown2.Value;
                    int width = (int)numericUpDown1.Value;
                    createTable(height, width);
                    label6.Text = "";
                }
                
            }

        }

        void createDataGridView(int height,int width)
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowCount = height;
            dataGridView1.ColumnCount = width;
            int i, j;

            for(i = 0; i < width; i++)
            {
                dataGridView1.Columns[i].Width = 30;
                for (j = 0; j < height; j++)
                {
                    dataGridView1.Rows[j].Cells[i].Value = 0;
                }
            }
        }
        void createTable(int height, int width)
        {
            double[,] mx = new double[height,width];
            int i, j;
            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    string rt;
                    rt= Convert.ToString(dataGridView1.Rows[i].Cells[j].Value);
                    DataTable dt = new DataTable();
                    mx[i,j] = Convert.ToDouble(dt.Compute(rt,""));

                }
            }
            prohodmatrix(height,width,mx);
        }

        void prohodmatrix(int height,int width,double[,] mx)
        {
            int w = start.Width;
            int h = start.Height;
            int fh = (height - 1) / 2;
            int fw= (width - 1) / 2;
            for (int i = 0;i<h;i++)
            {
                for(int j = 0;j<w;j++)
                {
                    double sumr = 0;
                    double sumb = 0;
                    double sumg = 0;
                    int i3,j3,i4,j4;
                    int ih1, jw1,ih2,jw2;
                    i3=i-fh;
                    j3=j-fw;
                    i4=i+fh;
                    j4=j+fw;
                    ih1 = i3; ih2 = i4;
                    for(int i2 = 0;i2<height;i2++)
                    {
                        jw1 = j3; jw2 = j4;
                        for(int j2 = 0;j2<width;j2++)
                        {
                            if (jw1 >= w) { jw1 = jw2; }
                            if (jw1 < 0) { jw1 = jw2; }
                            if (ih1 >= h) { ih1 = ih2; }
                            if (ih1 < 0) { ih1 = ih2; }
                            int r = start.GetPixel(jw1, ih1).R;
                            int g = start.GetPixel(jw1, ih1).G;
                            int b = start.GetPixel(jw1, ih1).B;
                            sumr += r * mx[i2, j2];
                            sumg += g * mx[i2, j2];
                            sumb += b * mx[i2, j2];

                            jw1++;jw2--;
                           
                        } 
                        ih1++; ih2--;
                    }
                    if (sumr < 0) { sumr = 0; } if (sumb < 0) { sumb = 0; }  if (sumg < 0) { sumg = 0; }
                    if (sumr > 255) { sumr = 255; }  if (sumb > 255) { sumb = 255; } if (sumg > 255) { sumg = 255; }
                    finish.SetPixel(j, i, Color.FromArgb((int)(sumr), (int)(sumg), (int)(sumb)));
                }
            }
            pictureBox1.Image = finish;


        }

        void mediana(int a1)
        {
            int f = (a1 - 1) / 2;
            int w = start.Width;
            int h = start.Height; 
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    int k = 0;
                    List<int> medr = new List<int>();
                    List<int> medg = new List<int>();
                    List<int> medb = new List<int>();
                    int i3, j3, i4, j4,j5;
                    i3 = i - f;
                    j3 = j - f;
                    i4 = i + f;
                    j4 = j + f;
                    if (i3 < 0) { i3 = 0; }
                    if (j3 < 0) { j3 = 0; }
                    if(j4>=w) { j4 = w-1; }
                    if(i4>=h) { i4 = h-1; }
                    for (; i3 <= i4; i3++)
                    {
                        j5 = j3;
                        for (; j5 <=j4 ; j5++)
                        {
                            int r = start.GetPixel(j5, i3).R;
                            int g = start.GetPixel(j5, i3).G;
                            int b = start.GetPixel(j5, i3).B;
                            medr.Add(r); medg.Add(g); medb.Add(b);
                           
                            k++;

                        }
                    }
                        k = (k + 1) / 2;
                    medr.Sort(); medg.Sort(); medb.Sort();
                    
                    finish.SetPixel(j, i, Color.FromArgb((int)(medr[k]), (int)(medg[k]), (int)(medb[k])));
                }
            }
            pictureBox1.Image = finish;



        }
        










        void gaus(int r,int sig)
        {
            int f = r * 2 + 1;
            double[,] mx= new double[f,f];
            double s = 0;
            double g;

            string ss="";
            string g_str = "";
            //ss.precision(5);

            var sig_sqr = 2.0 * sig * sig;
            var pi_siq_sqr = sig_sqr * Math.PI;

            for (int i = -r; i <= r; ++i)
            {
                for (int j = -r; j <= r; ++j)
                {
                    g = 1.0 / pi_siq_sqr * Math.Exp(-1.0 * (i * i + j * j) / (sig_sqr));
                    s += g;
                    mx[r+i,r+j] = g;
                }
                
            }
            ss = string.Format("{0:0.######}", s);
            label6.Text = ss;
            prohodmatrix(f, f, mx);
        }
    }

    
}
