using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        Dictionary<float, decimal> table = new Dictionary<float, decimal>();
        Bitmap input, output;
        double size;
        int[] B1 = new int[256];
        public Form1()
        {
            InitializeComponent();
            buttonEQ.Enabled = false;
            buttonLocal.Enabled = false;
            buttonStats.Enabled = false;
            buttonReset.Enabled = false;
            buttonSave.Enabled = false;
        }
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            for (int i = 0; i < 255; i++)
            {
                B1[i] = 0;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;)|*.BMP;*.JPG;*.GIF;*.PNG;|All Files (*.*)|*.*";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                    input = new Bitmap(pictureBox1.Image);                    
                    size = input.Width * input.Height;
                    for (int j = 0; j < input.Height; j++)
                        for (int i = 0; i < input.Width; i++)
                        {
                            int R = input.GetPixel(i, j).R;
                            int G = input.GetPixel(i, j).G;
                            int B = input.GetPixel(i, j).B;

                            B = (int)(0.299 * R + 0.587 * G + 0.114 * B);
                            if (B > 255)
                                B = 255;
                            if (B < 0)
                                B = 0;
                            B1[B]++;
                            input.SetPixel(i, j, Color.FromArgb(B, B, B));
                        }
                    pictureBox1.Image = input;
                    int max = 0;
                    for (int i = 0; i < 256; i++)
                        if(B1[i] > max)
                            max = B1[i];
                    chart1.ChartAreas[0].AxisY.Maximum = max + 0.01;
                    for (int i = 0; i < 256; i++)
                        chart1.Series[0].Points.AddXY(i, B1[i]);

                    buttonEQ.Enabled = true;
                    buttonLocal.Enabled = true;
                    buttonStats.Enabled = true;
                    buttonReset.Enabled = true;
                    buttonSave.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("Формат файла не поддерживается", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }        

        private void buttonReset_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
            {
                pictureBox1.Image = input;
                chart1.Series[0].Points.Clear();
                int max = 0;
                for (int i = 0; i < 256; i++)
                {
                    if (B1[i] > max)
                        max = B1[i];
                }
                chart1.ChartAreas[0].AxisY.Maximum = max + 0.01;
                for (int i = 0; i < 256; i++)
                {
                    chart1.Series[0].Points.AddXY(i, B1[i]);
                }
            }
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                SaveFileDialog Save = new SaveFileDialog();
                Save.Title = "Сохранить как";
                Save.OverwritePrompt = true;
                Save.CheckPathExists = true;
                Save.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG";
                Save.ShowHelp = true;
                if(Save.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        pictureBox1.Image.Save(Save.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void buttonEQ_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
            {
                //chart1.Series[0].Points.Clear();
                Bitmap output = new Bitmap(input.Width, input.Height);
                int imageSize = input.Width * input.Height;
                Color pixel;
                double value;
                int[] brightnessesCount = new int[256];
                double[] brightnessesPossibility = new double[256];
                double[] brightnessesPossibilityNormalized = new double[256];

                for (var i = 0; i < input.Width; i++)
                {
                    for (var j = 0; j < input.Height; j++)
                    {
                        pixel = input.GetPixel(i, j);
                        value = Math.Round(pixel.GetBrightness() * 255, 0); //берем яркость пикселя
                        brightnessesCount[(int)value]++; //считаем, сколько пикселей имеет эту яркость
                    }
                }
                double possibility;
                for (var i = 0; i < brightnessesCount.Length; i++)
                {
                    brightnessesPossibility[i] = (double)brightnessesCount[i] / imageSize;
                    possibility = brightnessesPossibility[i] * 255;

                    if (i != 0)
                        possibility += brightnessesPossibilityNormalized[i - 1];

                    brightnessesPossibilityNormalized[i] = Math.Round(possibility, 0);//новая яркость
                }
                for (var i = 0; i < input.Width; i++)
                {
                    for (var j = 0; j < input.Height; j++)
                    {
                        if ((int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G] > 255)
                        {
                            brightnessesPossibilityNormalized[input.GetPixel(i, j).G] = 255;
                        }
                        output.SetPixel(i, j,
                            Color.FromArgb((int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G],
                            (int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G],
                            (int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G])
                        );
                    }
                }
                int[] histogram = new int[256];
                float max = 0;
                int histHeight = 200;
                Bitmap imga = new Bitmap(256, histHeight + 10);
                Graphics g = Graphics.FromImage(imga);
                for (int i = 0; i < output.Width; i++) //заполняем массив гистограммы
                {
                    for (int j = 0; j < output.Height; j++)
                    {
                        int redValue = output.GetPixel(i, j).R;
                        histogram[redValue]++;
                        if (max < histogram[redValue])
                            max = histogram[redValue];
                    }
                }
                for (int i = 0; i < histogram.Length; i++)
                {
                    float pct = histogram[i] / max;
                    g.DrawLine(Pens.BlueViolet, new Point(i, imga.Height - 10), new Point(i, imga.Height - 10 - (int)(pct * 256))
                        );
                }
                HistogramBox.Image = imga;
                pictureBox1.Image = output;   
            }            
        }
                
        private void buttonReduction_Click(object sender, EventArgs e)
        {

        }

        private void buttonLocal_Click(object sender, EventArgs e)
        {
            Bitmap input = new Bitmap(pictureBox1.Image);
            Bitmap output = new Bitmap(input.Width, input.Height);
            int imageSize = input.Width * input.Height;
            double value;
            double[] brightnessesPossibility = new double[256];
            double[] brightnessesPossibilityNormalized = new double[256];
            for (var i = 0; i < input.Width; i++)
            {
                for (var j = 0; j < input.Height; j++)
                {
                    List<int[]> x = getVisiblePixels(i, j, 1, input.Width, input.Height);
                    int[] brightnessesCount = new int[256];
                    for (var q = 0; q < x.Count; q++)
                    {
                        Color pixel = input.GetPixel(x[q][0], x[q][1]);
                        value = Math.Round(pixel.GetBrightness() * 255, 0);
                        brightnessesCount[(int)value]++;
                    }
                    double possibility;
                    for (var s = 0; s < brightnessesCount.Length; s++)
                    {
                        brightnessesPossibility[s] = (double)brightnessesCount[s] / x.Count;
                        possibility = brightnessesPossibility[s] * 255;

                        if (s != 0)
                        {
                            possibility += brightnessesPossibilityNormalized[s - 1];
                        }

                        brightnessesPossibilityNormalized[s] = Math.Round(possibility, 0);//новая яркость
                    }
                    if ((int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G] > 255)
                    {
                        brightnessesPossibilityNormalized[input.GetPixel(i, j).G] = 255;
                    }
                    output.SetPixel(i, j,
                        Color.FromArgb((int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G],
                        (int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G],
                        (int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G])
                    );
                }
            }
            pictureBox1.Image = output;
            int[] histogram = new int[256];
            float max = 0;
            int histHeight = 200;
            Bitmap imga = new Bitmap(256, histHeight + 10);
            Graphics g = Graphics.FromImage(imga);
            for (int i = 0; i < output.Width; i++) //заполняем массив гистограммы
            {
                for (int j = 0; j < output.Height; j++)
                {
                    int redValue = output.GetPixel(i, j).R;
                    histogram[redValue]++;
                    if (max < histogram[redValue])
                        max = histogram[redValue];
                }
            }
            for (int i = 0; i < histogram.Length; i++)
            {
                float pct = histogram[i] / max;
                g.DrawLine(Pens.BlueViolet,
                    new Point(i, imga.Height - 10),
                    new Point(i, imga.Height - 10 - (int)(pct * 256))
                    );
            }
            HistogramBox.Image = imga;
        }
        bool isOutOfRange(int x1, int y1, int width1, int height1)
        {
            return x1 < 0 || x1 >= width1 || y1 < 0 || y1 >= height1;
        }

        public List<int[]> getVisiblePixels(int x, int y, int n, int width, int height)
        {
            int xCounter = x - n;
            int xLocal;
            int yCounter = y - n;
            int yLocal;
            int side = (1 + n + n);
            int pixelsCount = side * side;

            List<int[]> pixels = new List<int[]>();

            for (int i = 0; i < side; i++)
            {
                xLocal = xCounter + i;

                for (int j = 0; j < side; j++)
                {
                    yLocal = yCounter + j;

                    if (isOutOfRange(xLocal, yLocal, width, height))
                    {
                        continue;
                    }

                    pixels.Add(new int[] { xLocal, yLocal });
                }
            }
            return pixels;
        }

        private void buttonStats_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap input = new Bitmap(pictureBox1.Image);
                Bitmap output = new Bitmap(input.Width, input.Height);
                int imageSize = input.Width * input.Height;
                double value;
                Color pixel;
                int[] brightnessesCount = new int[256];
                double[] brightnessesPossibility = new double[256]; //глобальная вероятность
                double[] brightnessesPossibility2 = new double[256]; //локальная вероятность
                double[] brightnessesPossibilityNormalized = new double[256];
                double Mg, MG = 0; //глобальное мат ожидание
                double Dg, DG = 0; //глобальная дисперсия
                double Ms, MS = 0; //локальное мат ожидание
                double Ds, DS = 0; //локальная дисперсия
                double k0 = 0.6; double E = 1.5; double k1 = 0.02; double k2 = 0.6; //заданные константы
                for (var i = 0; i < input.Width; i++)
                {
                    for (var j = 0; j < input.Height; j++)
                    {
                        pixel = input.GetPixel(i, j);
                        value = value = Math.Round(pixel.GetBrightness() * 255, 0); //берем яркость пикселя
                        brightnessesCount[(int)value]++; //считаем, сколько пикселей имеет эту яркость
                    }
                }
                for (var i = 0; i < brightnessesCount.Length; i++) //считаем глобальное мат ожидание
                {
                    brightnessesPossibility[i] = (double)brightnessesCount[i] / imageSize;
                    Mg = i * brightnessesPossibility[i];
                    if (i != 0)
                    {
                        MG += Mg;
                        MG = Math.Round(MG, 2);
                    }
                }
                for (var i = 0; i < brightnessesCount.Length; i++) //считаем глобальную дисперсию
                {
                    brightnessesPossibility[i] = (double)brightnessesCount[i] / imageSize;
                    Dg = Math.Pow((i - MG), 2) * brightnessesPossibility[i];
                    if (i != 0)
                    {
                        DG += Dg;
                        DG = Math.Round(DG, 2);
                    }
                }
                double possibility;
                double c;
                for (var i = 0; i < input.Width; i++)
                {
                    for (var j = 0; j < input.Height; j++)
                    {
                        List<int[]> x = getVisiblePixels(i, j, 1, input.Width, input.Height);
                        int[] brightnessesCount2 = new int[256];
                        for (var q = 0; q < x.Count; q++)
                        {
                            pixel = input.GetPixel(x[q][0], x[q][1]);
                            value = Math.Round(pixel.GetBrightness() * 255, 0);
                            brightnessesCount2[(int)value]++;
                        }

                        for (var s = 0; s < brightnessesCount2.Length; s++) //считаем локальное мат ожидание
                        {
                            brightnessesPossibility2[s] = (double)brightnessesCount2[s] / x.Count;
                            Ms = s * brightnessesPossibility2[s];
                            possibility = brightnessesPossibility[s] * 255;
                            MS += Ms;
                            MS = Math.Round(MS, 2);
                            if (s != 0)
                            {
                                possibility += brightnessesPossibilityNormalized[s - 1];
                            }
                            brightnessesPossibilityNormalized[s] = Math.Round(possibility, 0);//новая яркость
                        }
                        for (var k = 0; k < brightnessesCount2.Length; k++) //считаем локальную дисперсию
                        {
                            brightnessesPossibility2[k] = (double)brightnessesCount2[k] / x.Count;
                            Ds = Math.Pow((k - MS), 2) * brightnessesPossibility2[k];
                            DS += Ds;
                            DS = Math.Round(DS, 2);
                        }
                        if (MS <= (k0 * MG) || ((DS >= (k1 * DG)) && (DS <= (k2 * DG))))
                        {
                            c = E * brightnessesPossibilityNormalized[input.GetPixel(i, j).G];
                            if (c > 255) c = 255;
                            output.SetPixel(i, j, Color.FromArgb((int)c, (int)c, (int)c));
                        }
                        else
                        {
                            if ((int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G] > 255)
                            {
                                brightnessesPossibilityNormalized[input.GetPixel(i, j).G] = 255;
                            }
                            output.SetPixel(i, j,
                                Color.FromArgb((int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G],
                                (int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G],
                                (int)brightnessesPossibilityNormalized[input.GetPixel(i, j).G])
                            );
                        }
                        MS = 0; DS = 0;
                    }
                }
                int[] histogram = new int[256];
                float max = 0;
                int histHeight = 200;
                Bitmap imga = new Bitmap(256, histHeight + 10);
                Graphics g = Graphics.FromImage(imga);
                for (int i = 0; i < output.Width; i++) //заполняем массив гистограммы
                {
                    for (int j = 0; j < output.Height; j++)
                    {
                        int redValue = output.GetPixel(i, j).R;
                        histogram[redValue]++;
                        if (max < histogram[redValue])
                            max = histogram[redValue];
                    }
                }
                for (int i = 0; i < histogram.Length; i++)
                {
                    float pct = histogram[i] / max;
                    g.DrawLine(Pens.BlueViolet,
                        new Point(i, imga.Height - 10),
                        new Point(i, imga.Height - 10 - (int)(pct * 256))
                        );
                }
                HistogramBox.Image = imga;
                pictureBox1.Image = output;
            }
        }
        private void label1_Click(object sender, EventArgs e){}          
    }
}