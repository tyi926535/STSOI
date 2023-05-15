using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;


using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LW2
{
    
    public partial class Form1 : Form
    {
        int it = -1;
        Point[] po1 = new Point[129];
        int[] gis = new int[129];
        int[] gisF = new int[129];
        int[] gisR = new int[129];
        int[] gisB = new int[129];
        int[] gisG = new int[129];
        int tr1 = 0;
        int tr2 = 0;
        int tr3 = 0;

        Graphics grGis ;
        Graphics grGisR;
        Graphics grGisB;
        Graphics grGisG;
        Graphics grGisF;
        bool mouse = false;

        Point click;
        Point first;
        
        Point end;
        List<Graphics> GF=new List<Graphics>();
        List<Button> CG = new List<Button>();

        public Form1()
        {
            InitializeComponent();
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            ClientSize = new Size(700, 420);
            FormBorderStyle= FormBorderStyle.FixedSingle;
            this.Text = "2 Лаба по СЦОИ";
        }
        
        
        private void button1_Click(object sender, EventArgs e) //Стартовая линия
        {
            var opd = new OpenFileDialog();// Создает окно открытия файла
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;//Размещает по всей ширине объекта
            if (opd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(opd.FileName);
                pictureBox1.BackColor = Color.Transparent;
            }
            var NImg = (Bitmap)pictureBox1.Image;
            var w = NImg.Width;
            var h = NImg.Height;
            start = new Bitmap(w, h);
            finish = new Bitmap(w, h);
            Gstart= pictureBox1.CreateGraphics();
            Gfinish = pictureBox1.CreateGraphics();

            //попиксельно обрабатываем картинку 
            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    var pix = NImg.GetPixel(j, i);
                    start.SetPixel(j, i, pix);
                    finish.SetPixel(j, i, pix);
                    Gstart = Graphics.FromImage(start);
                    Gfinish = Graphics.FromImage(finish);

                }
            }
            
            button1.Visible = false;
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox7.Visible = true;

            button3.Visible = true;
            comboBox1.Visible = true;
            comboBox1.SelectedIndex = 1;
        }


        private void doubleClickPB2(object sender, MouseEventArgs e)//Добавление точек
        {
            if (comboBox1.SelectedIndex == 1)
            {
                tr1 = 1;
                button2.Visible = true;
                it++;
                click = e.Location;
                Button nCG = Clone(button2);
                nCG.Location = new Point(click.X, click.Y);
                CG.Add(nCG);
                CG[it].MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpCG);
                this.pictureBox2.Controls.Add(CG[it]);
                button2.Visible = false;
                Grafic();
            }
            if (comboBox1.SelectedIndex == 0)
            {
                pictureBox2.Invalidate();
                pictureBox2.Update();
                it = -1;
                picher();
                tr3 = 1;
            }




        }

        public static T Clone<T>(T controlToClone) where T : Control// Не знаю как это работает но не трогайте пж
        {
            T instance = Activator.CreateInstance<T>();

            PropertyInfo[] props = controlToClone.GetType().GetProperties();

            foreach (PropertyInfo pi in props)
            {
                if ((pi.CanWrite) && !(pi.Name == "WindowTarget") && !(pi.Name == "Capture"))
                {

                    pi.SetValue(instance, pi.GetValue(controlToClone, null), null);
                }
            }
            return instance;
        }

        private void MouseUpCG(object sender, MouseEventArgs e)//Перенос и удаление точек
        {
            if (comboBox1.SelectedIndex == 1)
            {
                if (it > -1)
                {
                    Point r;
                    Point v;

                    click = PointToClient(Cursor.Position); ;
                    int g = 0;
                    for (int i = 0; i <= it; i++)
                        if (CG[i] == sender) { g = i; }
                    r = CG[g].Location;
                    v = r;
                    CG[g].Location = new Point(click.X - 10, click.Y - 15);


                    for (int i = r.X - 20; i <= r.X + 20; i++)
                        for (int j = r.Y - 20; j <= r.Y + 20; j++)
                        {
                            v.X = i; v.Y = j;
                            if (click == v)
                            {
                                CG[g].Visible = false;
                                CG.RemoveAt(g);
                                it = it - 1;
                            }
                        }
                    g++;
                    Grafic();
                }
            }
        }

        void Grafic()//Рисуем график только для линейной
        {
            
            Graphics grap; 
            grap = pictureBox2.CreateGraphics();
            if(tr1>0)
            {
            pictureBox2.Invalidate();
            pictureBox2.Update();
            }
            if (comboBox1.SelectedIndex == 1)
            {
                if (it == -1)
                {
                    for (int i = 0; i < po1.Length; i++)
                    {
                        po1[i] = first;
                        po1[i] = new Point(i * 3, (128 - i) * 3);
                    }
                    grap.DrawLines(Pens.Red, po1);
                    picher();
                    GF.Add(grap);

                }
                else
                {
                    int[] yu = new int[it+1];
                    for (int j=0; j < yu.Length; j++)
                    {
                        yu[j] = j;
                    }
                   
                    for (int j = 0; j < it+1 ;  j++)
                    {
                        for (int i = j+1; i <= it; i++)
                        {
                            int r1 = CG[j].Location.X;
                            int r2 = CG[i].Location.X;
                            if (r1 > r2)
                            {
                                Point h = CG[j].Location;
                                CG[j].Location = CG[i].Location;
                                CG[i].Location = h;
                            }
                        }
                    }

                    for (int j = 0; j <= (it+1); j++)
                    {

                        double l1, l2, m1, m2;
                        int h=0; int r = 0;float k = 0;
                        Point p = new Point(0, 0);
                        if (j<=it)
                        {
                            p = CG[j].Location;
                            p = new Point((int)(p.X / 3) * 3, (int)(p.Y / 3) * 3);
                            CG[j].Location = p;
                        }
                        if(j == 0)
                        {
                            h = 0;
                            r = (int)(p.X / 3);
                            k = (p.Y) / (p.X);
                            l1 = (int)(first.X / 3);
                            m1 = (int)(first.Y / 3);
                            l2 = (int)(p.X / 3);
                            m2= (int)(p.Y / 3);
                        }
                        else
                        {
                            if (j > it)
                            {
                                h = (int)(CG[j-1].Location.X/3);
                                r = po1.Length;
                                l1 = (int)(CG[j - 1].Location.X / 3);
                                m1 = (int)(CG[j - 1].Location.Y / 3);
                                l2 = (int)(end.X / 3);
                                m2 = (int)(end.Y / 3);

                            }
                            else
                            {
                                h = (int)(CG[j-1].Location.X / 3);
                                r = (int)(CG[j].Location.X / 3);
                                l1 = (int)(CG[j - 1].Location.X / 3);
                                m1 = (int)(CG[j - 1].Location.Y / 3);
                                l2 = (int)(p.X / 3);
                                m2 = (int)(p.Y / 3);
                            }
                        }
                        if (h > 129 || r > 129)
                        {
                            if (h > 129) { h = 129; }
                            if (r > 129) { r = 129; }

                        }
                        if (h <0 || r <0)
                        {
                            if (h <0) { h = 0; }
                            if (r <0) { r = 0; }

                        }
                        for (; h < r; h++)
                            {
                                double rt;
                                po1[h] = first;
                                rt = (double)((h - l1) / (l2 - l1));
                                rt = (double)(rt * (m2 - m1));
                                rt = (double)(Math.Abs(rt + m1));
                                po1[h] = new Point(h * 3, (int)(rt * 3));
                            }
                       

                    }
                    grap.DrawLines(Pens.Red, po1);
                }

                picher();
                GF[0]=grap;
            }

        }
        private Bitmap start;
        private Bitmap finish;
        private Graphics Gstart;
        private Graphics Gfinish;
        void picher()//Здесь обрабатывается изображение и делается заготовка для гистограмм
        {
            if(it==-1)
            {
                pictureBox1.Image = start;

                var NImg = (Bitmap)pictureBox1.Image;
                    var w = NImg.Width;
                    var h = NImg.Height;
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            var pix = start.GetPixel(j, i);
                            int r = (int)(pix.R);
                            int g = (int)(pix.G);
                            int b = (int)(pix.B);
                            int gd = (int)((r + g + b)) / 3;
                            gisF[(int)(gd / 2)]++;
                            gis[(int)(gd / 2)]++;
                            gisR[(int)r / 2]++;
                            gisG[(int)g / 2]++;
                            gisB[(int)b / 2]++;
                        }
                    }
                    gistograms();

            }
            if (it > -1)
            {

                var NImg = (Bitmap)pictureBox1.Image;
                var w = NImg.Width;
                var h = NImg.Height;
                Gstart = pictureBox1.CreateGraphics();
                Gfinish = pictureBox1.CreateGraphics();

                for (int i = 0; i < h; ++i)
                {
                    for (int j = 0; j < w; ++j)
                    {
                        var pix = start.GetPixel(j, i);
                        int c = (int)((pix.R + pix.B + pix.G)) / 6;
                        double gr = 129 - (double)((int)po1[c].Y/3);
                        double k = gr / c;
                        int r = (int)(pix.R * k);
                        int g = (int)(pix.G * k);
                        int b = (int)(pix.B * k);
                        if (r > 255) { r = 255; }
                        if (g > 255) { g = 255; }
                        if (b > 255) { b = 255; }
                        if (r < 0) { r = 0; }
                        if (g < 0) { g = 0; }
                        if (b < 0) { b = 0; }
                        int gd = (int)((r + g + b)) / 3;
                        gis[(int)(gd/2)]++;
                        gisR[(int)r/2]++;
                        gisG[(int)g/2]++;
                        gisB[(int)b/2]++;
                        finish.SetPixel(j, i, Color.FromArgb(r, g, b));
                    }
                }
               pictureBox1.Image= finish;

                gistograms();
            }
            for (int i = 0; i < gis.Length; i++)
            {
                gis[i] = 0;
                gisR[i] = 0;
                gisG[i] = 0;
                gisB[i] = 0;
            }

        }

        

        void gistograms()//Здесь создается гистограмма
        {
           // if (it > -1)
            {
                double max=0;
                double maxR = 0;
                double maxB = 0;
                double maxG = 0;
                double maxF = 0;
                for (int i=0; i< gis.Length; i++)
                {
                    if (gis[i] > max) { max =gis[i]; }
                    if (gisR[i] > maxR) { maxR = gisR[i]; }
                    if (gisB[i] > maxB) { maxB = gisB[i]; }
                    if (gisG[i] > maxG) { maxG = gisG[i]; }
                    if (gisF[i] > maxF) { maxF = gisF[i]; }
                }
                double h =pictureBox3.Height-20;
                double k1;
                if (h > max){k1 = max/h; } else{k1 = h/max;}

                grGis = pictureBox3.CreateGraphics();
                pictureBox3.Invalidate();
                pictureBox3.Update();
               // pictureBox3.Clean();
                Point nk = first;
                Point kk = first;
                for (int i = 0; i < gis.Length; i++)
                {
                    nk.X = i * 2 + 5; nk.Y = 90;
                    kk.X = i * 2 + 5; kk.Y = 90 - (int)(k1 * gis[i]);
                    grGis.DrawLine(Pens.White, nk, kk);
                    
                }

                if (button3.TabIndex == 1)
                {
                    double kr;
                    double kb;
                    double kg;
                    double kf;
                    if (h > maxR){kr = maxR / h;}else{kr = h / maxR;}
                    if (h > maxB){kb = maxB / h;}else{kb = h / maxB;}
                    if (h > maxG) {kg = maxG / h;}else{kg = h / maxG;}
                    if (h > maxF) {kf = maxF / h;}else{ kf = h / maxF;}
                    grGisR = pictureBox4.CreateGraphics();
                    pictureBox4.Invalidate();
                    pictureBox4.Update();
                    Point nkr = first;
                    Point kkr = first;
                    grGisB = pictureBox5.CreateGraphics();
                    pictureBox5.Invalidate();
                    pictureBox5.Update();
                    Point nkb = first;
                    Point kkb = first;
                    grGisG = pictureBox6.CreateGraphics();
                    pictureBox5.Invalidate();
                    pictureBox5.Update();
                    Point nkg = first;
                    Point kkg = first;
                    
                        grGisF = pictureBox7.CreateGraphics();
                        Point nkf = first;
                        Point kkf = first;
                    
                    

                    for (int i = 0; i < gis.Length; i++)
                    {
                        nkr.X = i * 2 + 5; nkr.Y = 90;
                        kkr.X = i * 2 + 5; kkr.Y = 90 - (int)(kr * gisR[i]);
                        grGisR.DrawLine(Pens.Red, nkr, kkr);
                        nkb.X = i * 2 + 5; nkb.Y = 90;
                        kkb.X = i * 2 + 5; kkb.Y = 90 - (int)(kb * gisB[i]);
                        grGisB.DrawLine(Pens.Blue, nkb, kkb);
                        nkg.X = i * 2 + 5; nkg.Y = 90;
                        kkg.X = i * 2 + 5; kkg.Y = 90 - (int)(kg * gisG[i]);
                        grGisG.DrawLine(Pens.Green, nkg, kkg);
                        if (tr2 == 0)
                        {
                            nkf.X = i * 2 + 5; nkf.Y = 90;
                            kkf.X = i * 2 + 5; kkf.Y = 90 - (int)(kf * gisF[i]);
                            grGisF.DrawLine(Pens.White, nkf, kkf);
                        }


                    }
                    tr2++;
                }
            }
        }
        Button nCG1;
        Button nCG2;
        private void clickPB2(object sender, EventArgs e)//обязательно надо нажать на график чтобы начать работу
        {
            
            if (it < 0 && comboBox1.SelectedIndex == 1 && tr1==0)
            {
                click = PointToClient(Cursor.Position);
                first = click;
                end = click;
                first.X = 0;
                first.Y = 384;
                end.X = 384;
                end.Y = 0;
                button2.Visible = true;
                nCG1 = Clone(button2);
                nCG1.Location = new Point(first.X, first.Y);
                this.pictureBox2.Controls.Add(nCG1);
                nCG2 = Clone(button2);
                nCG2.Location = new Point(end.X, end.Y);
                this.pictureBox2.Controls.Add(nCG2);
                button2.Visible = false;
                Grafic();
                tr1 = 1;
            }

        }

        private void button3_Click(object sender, EventArgs e)//Открывает 2 пространство
        {
              if(button3.TabIndex==0)
              {
                button3.TabIndex = 1;
                button3.Text = "Свернуть";
                ClientSize = new Size(1000, 420);
                pictureBox4.Visible = true;
                pictureBox5.Visible = true;
                pictureBox6.Visible = true;
            }
            else
            {
                if (button3.TabIndex == 1)
                {
                    button3.TabIndex = 0;
                    button3.Text = "Развернуть";
                    tr2 = 0;
                    ClientSize = new Size(700, 420);
                    pictureBox4.Visible = false;
                    pictureBox5.Visible = false;
                    pictureBox6.Visible = false;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            pictureBox2.Invalidate();
            pictureBox2.Update();
            pictureBox3.Invalidate();
            pictureBox3.Update();
            int r = 0;
            for(int i = 0; i <= it; i++)
            {
                CG[r].Visible = false;
                CG.RemoveAt(r);
            }
            it = -1;
            pictureBox1.Image = start;
            picher();
            if(comboBox1.SelectedIndex==1 && tr1>0)
            {
                nCG1.Visible = true;
                nCG2.Visible = true;
            }
            if(comboBox1.SelectedIndex==0 && tr1>0)
            {
                nCG1.Visible = false;
                nCG2.Visible = false;

            }

        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            mouse = true;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (comboBox1.SelectedIndex == 0 )
            {
                mouse =false;
                it = 0;
                if(tr3==1)
                {
                    pictureBox2.Invalidate();
                    pictureBox2.Update();
                    it = -1;
                    tr3 = 0;

                }
                picher();
                mR1.X = 0;mR1.Y = 0;

            }
        }
        Rectangle mR1;
        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse==true && comboBox1.SelectedIndex == 0)
            {
                Graphics grap;
                grap = pictureBox2.CreateGraphics();
                
                int x=(int)(e.X/3);
                if(x>0 && x<128)
                {
                    po1[x] = first;
                    po1[x] = new Point(e.X, e.Y);
                    Pen mP = new Pen(System.Drawing.Color.Red, 3);
                    Rectangle mR2 = new Rectangle(po1[x].X, po1[x].Y, 1, 1);
                    grap.DrawEllipse(mP, mR2);
                    if(mR1.X!=0&& mR1.Y != 0)
                    {
                        grap.DrawLine(mP, mR2.X,mR2.Y,mR1.X,mR1.Y);

                    }
                    mR1 = mR2;
                    //GF.Add(grap);

                }

            }
        }
    }

    
}
