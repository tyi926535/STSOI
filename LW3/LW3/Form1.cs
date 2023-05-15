using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LW3
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Bitmap start;
        private Bitmap average;
        private Bitmap finish;
        private Graphics Gstart;
        private Graphics Gfinish;
        int ty = -1;
        int w = 0;
        int h = 0;
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
            w = NImg.Width;
            h = NImg.Height;
            start = new Bitmap(w, h);
            average = new Bitmap(w, h);
            finish = new Bitmap(w, h);
            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    var pix = NImg.GetPixel(j, i);
                    start.SetPixel(j, i, pix);
                    average.SetPixel(j, i, pix);

                }
            }
            comboBox1.Visible = true;
            pictureBox1.Visible = true;
            button1.Visible = false;
            checkedListBox1.Visible = true;
            comboBox1.SelectedIndex = 0;

        }

       

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int kr = -1; 
            if (checkedListBox1.CheckedItems.Count > 1)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    checkedListBox1.SetItemChecked(i, false);
                checkedListBox1.SetItemChecked(checkedListBox1.SelectedIndex, true);
            }
            if (checkedListBox1.SelectedIndex == 0)
            {
                ty = 0;
                kr = 0; greay(kr);
            }
            if (checkedListBox1.SelectedIndex == 1)
            {
                ty = 0;
                kr = 1; greay(kr);
            }
            if (checkedListBox1.SelectedIndex == 2)
            {
                ty = 0;
                kr = 2; greay(kr);
            }
            if (checkedListBox1.SelectedIndex == 3)
            {
                ty = 0;
                kr = 3; greay(kr);
            }
            //comboBox1_SelectedIndexChanged(sender, e);

        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value % 2 == 0) { numericUpDown1.Value += 1; }
            comboBox1_SelectedIndexChanged(sender, e);
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(sender, e);
        }

        void greay(int kr)
        {
            var NImg = start;
            var w1 = NImg.Width;
            var h1 = NImg.Height;
            Array.Clear(gis, 0, gis.Length);//очищаем
            for (int i = 0; i < h1; ++i)
            {
                for (int j = 0; j < w1; ++j)
                {
                    var pix = start.GetPixel(j, i);
                    int r = (int)(pix.R);
                    int g = (int)(pix.G);
                    int b = (int)(pix.B);
                    int gd = 0;
                    if (kr == 0)
                    { gd = (int)((r + g + b)) / 3; }
                    if (kr == 1)
                    {
                         gd = r;
                        if (g>gd) { gd = g; }
                        if (b>gd) { gd = b; }
                     }
                    if (kr == 2)
                    { gd = (int)((0.2125*r + 0.7154*g + 0.0721*b)) / 3; }
                    if (kr == 3)
                    {
                        gd = r;
                        if (g < gd) { gd = g; }
                        if (b < gd) { gd = b; }
                    }

                    gis[(int)(gd)]++;
                    average.SetPixel(j, i, Color.FromArgb(gd, gd, gd));

                }
            }
            pictureBox1.Image = average;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ng=comboBox1.SelectedIndex;
            int a = 0;
            float k = 0;
            if (ty>-1)
            {
                if(ng==1 || ng==2 || ng==0)
                {
                    label1.Visible = false;
                    label2.Visible = false;
                    numericUpDown1.Visible = false;
                    numericUpDown2.Visible = false;
                }
                if (ng == 3 || ng == 4 || ng == 5 || ng==6)
                {
                    label1.Visible = true;
                    label2.Visible = true;
                    numericUpDown1.Visible = true;
                    numericUpDown2.Visible = true;
                   // numericUpDown2_ValueChanged(sender, e);
                    a = (int)(numericUpDown1.Value); 
                    k = (float)(numericUpDown2.Value);
                }
                if (ng == 0) {checkedListBox1_SelectedIndexChanged(sender, e); }
                if (ng == 1){ gavr(); }
                if (ng == 2){ otsu(); }
                if (ng == 3){ nibl(a,k,3); label2.Text = "Sensitivity"; }
                if (ng == 4) { nibl(a, k, 4); label2.Text = "k"; }
                if (ng == 5) { vulf(a,k); label2.Text = "a"; }
                if (ng == 6) { Bredli(a, k); label2.Text = "Sensitivity"; }
            }

        }
       

        void gavr()
        {
            float sum = 0;
            for (var i = 0; i < gis.GetLength(0); i++)
            {
                sum += gis[i] * i;
            }
            float t= sum/(h * w);


            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    var pix = average.GetPixel(j, i);
                    int gd = (int)(pix.R);
                    if (gd <= t) { gd = 0; }
                    if (gd > t) { gd = 255; }
                    finish.SetPixel(j, i, Color.FromArgb(gd, gd, gd));
                }
            }
            pictureBox1.Image = finish;
        }

        void otsu()
        {
            float W1 = w; float H1 = h;
            float[] n = new float[256];
            float s2b = 0;
            float ts2b = 0;
            float w1 = 0;
            float w2 = 0;
            float u1 = 0, ut = 0, u2 = 0;
            float iN = 0;
            float t = 0;
            for (var i = 0; i < gis.GetLength(0); i++)
            {
                n[i] = (float)gis[i] / (float)(w * h);
                ut = (n[i] * i)+ut;
            }

            for (var i = 0; i < n.GetLength(0); i++)
            { 
                w2 = 1 - w1;
                u1 = iN / w1; u2 = (ut - u1 * w1) / w2;
                s2b = ((float)(w1 * w2 * Math.Pow((u1 - u2), 2)));
                if (s2b > ts2b) { t = i;ts2b = s2b; }
                w1 += n[i]; iN = (i * n[i])+iN;

            }
            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    var pix = average.GetPixel(j, i);
                    int gd = (int)(pix.R);
                    if (gd <= t) { gd = 0; }
                    if (gd > t) { gd = 255; }
                    finish.SetPixel(j, i, Color.FromArgb(gd, gd, gd));
                }
            }
            pictureBox1.Image = finish;


        }

        void nibl(int a, float k, int ir)
        {
            int i1, i2;
            int j1, j2;
            int f=a  / 2;
            int width = average.Width;
            int height = average.Height;

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    float s2 = 0, m2 = 0, t = 0;
                    float s = 0, m = 0;
                    float d = 0, q = 0;
                    float kol = 0; 
                    if ((i - f) <= 0) { i1 = 0; } else { i1 = i - f; };
                    if ((i + f) >= h) { i2 = h - 1; } else { i2 = i + f; };
                    if ((j - f) <= 0) { j1 = 0; } else { j1 = j - f; };
                    if ((j + f) >= w) { j2 = w - 1; } else { j2 = j + f; };
                    for (int i11=i1; i11 <= i2; ++i11)
                    {
                        for(int j11=j1; j11 <= j2; ++j11)
                        {
                            kol++;
                            var pix1 = average.GetPixel(j11, i11);
                            int gd1 = (int)(pix1.R);
                            s += gd1; s2 += (float)Math.Pow(gd1, 2);
                        }
                    }
                    m = s / kol; m2 = s2 / kol;
                    d=m2-(float)Math.Pow(m, 2);
                    q = (float)Math.Sqrt(d);
                    if (ir == 3) { t = (m + (k * q)); }
                   if (ir == 4) 
                    {
                        int r = 0;
                        int clb = checkedListBox1.SelectedIndex;
                        if (clb==1 || clb == 3) { r = 256; }
                        else { r = 128; }
                        t = m * (1+k*((q/r)-1)); 
                    }

                    var pix = average.GetPixel(j, i);
                    int gd = (int)(pix.R);
                    if (gd <= t) { gd = 0; }
                    if (gd > t) { gd = 255; }
                    finish.SetPixel(j, i, Color.FromArgb(gd, gd, gd));
                }
            }
            pictureBox1.Image = finish;

        }
        void vulf(int a,float k)
        {
            float kol = 0;
            int i1, i2;
            int j1, j2;
            float s2 = 0, m2 = 0, t = 0;
            float s = 0, m = 0;
            float d = 0, q = 0;
            int f = (a - 1) / 2;
            float[,] MO=new float[h,w];
            float[,] QO=new float[h,w];
            float maxq = 0;
            float min = 0;
            for (var i = 0; i < gis.GetLength(0); i++)
            { if (gis[i] > min) { min= i; break; } }

            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    kol = 0;
                    s2 = 0;
                    s = 0;
                    if ((i - f) <= 0) { i1 = 0; } else { i1 = i - f; };
                    if ((i + f) >= h) { i2 = h - 1; } else { i2 = i + f; };
                    if ((j - f) <= 0) { j1 = 0; } else { j1 = j - f; };
                    if ((j + f) >= w) { j2 = w - 1; } else { j2 = j + f; };
                    for (int i11 = i1; i11 <= i2; ++i11)
                    {
                        for (int j11 = j1; j11 <= j2; ++j11)
                        {
                            kol++;
                            var pix1 = average.GetPixel(j11, i11);
                            int gd1 = (int)(pix1.R);
                            s += gd1; s2 += (float)Math.Pow(gd1, 2);
                        }
                    }
                    m = s / kol; m2 = s2 / kol;
                    MO[i,j] = m;
                    d = m2 - (float)Math.Pow(m, 2);
                    q = (float)Math.Sqrt(d);
                    QO[i,j] = q;
                   if (q > maxq) { maxq = q; }
                }
            }
            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    t = (1 - k) * MO[i,j]+k*min+k*(QO[i,j]/maxq)*(MO[i,j]-min);
                    var pix = average.GetPixel(j, i);
                    int gd = (int)(pix.R);
                    if (gd <= t) { gd = 0; }
                    if (gd > t) { gd = 255; }
                    finish.SetPixel(j, i, Color.FromArgb(gd, gd, gd));
                }
            }
            pictureBox1.Image = finish;
        }

        void Bredli(int a, float k)
        {
            int width = average.Width; 
            int height= average.Height;
            int i1, i2;
            int j1, j2;
            int f = a/2;
            float[,] S = new float[height, width];

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    var pix = average.GetPixel(j, i).R;
                    float k1, k2, k3;
                    S[i, j] = 0;
                    if (j < 1) {k1 = 0; }
                    else {  k1 = S[i, j - 1];  }

                    if (i < 1) k2 = 0;
                    else k2 = S[i - 1, j];

                    if (j >= 1 && i >= 1) k3 = S[i - 1, j - 1];
                    else k3 = 0;

                    S[i, j] = pix + k1 + k2 - k3;
                }
            }
            for (int i = 0; i < height; ++i)
            {

                for (int j = 0; j < width; ++j)
                {
                    float sum = 0;
                    float k1, k2, k3;
                    i2 = i - (int)Math.Ceiling((decimal)a / 2);
                    i1 = i + (int)Math.Floor((decimal)a  / 2);
                    j2 = j - (int)Math.Ceiling((decimal)a / 2);
                    j1 = j + (int)Math.Floor((decimal)a / 2);
                    if (j2 < 0) { j2 = 0; }
                    if (j1 >= width) { j1 = width - 1; }
                    if (i2 < 0) { i2 = 0; }
                    if (i1 >= height) { i1 = height - 1; }
                    if (i2 >= 0 && j2 >= 0) k1 = S[i2, j2];
                    else k1 = 0;
                    if (i2 >= 0) k2 = S[i2, j1];
                    else k2 = 0;
                    if (j2 >= 0) k3 = S[i1, j2];
                    else k3 = 0;

                    sum = k1 + S[i1, j1] - k2 - k3;
                    int count = (i2 - i1) * (j2 - j1);

                    float ksum = sum * (1 - k);
                    var pix = average.GetPixel(j, i);
                    int gd = (int)((pix.R) * count);
                    if (gd < ksum) { gd = 0; }
                    if (gd >= ksum) { gd = 255; }
                    finish.SetPixel(j, i, Color.FromArgb(gd, gd, gd));
                }
            }
            pictureBox1.Image = finish;
        }




    }
}
