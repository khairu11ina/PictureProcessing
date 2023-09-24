using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cancelButton.Enabled = false;
            saveButton.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            numericUpDown1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
        }
        Bitmap input, output, mask;
        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;)|*.BMP;*.JPG;*.GIF;*.PNG;|All Files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                    input = new Bitmap(pictureBox1.Image);
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
                            input.SetPixel(i, j, Color.FromArgb(B, B, B));
                        }
                    pictureBox1.Image = input;
                    cancelButton.Enabled = true;
                    saveButton.Enabled = true;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    numericUpDown1.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;
                    button5.Enabled = true;
                    button6.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("Формат файла не поддерживается", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = input;
            }
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                SaveFileDialog Save = new SaveFileDialog();
                Save.Title = "Сохранить как";
                Save.OverwritePrompt = true;
                Save.CheckPathExists = true;
                Save.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG";
                Save.ShowHelp = true;
                if (Save.ShowDialog() == DialogResult.OK)
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
        public List<int[]> getVisiblePixels(int x, int y, int n, int width, int height)
        { //берем пиксели для квадрата 3х3
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
                        pixels.Add(new int[] { 10000, 10000 });
                        continue;
                    }
                    pixels.Add(new int[] { xLocal, yLocal });
                }
            }
            return pixels;
        }
        bool isOutOfRange(int x1, int y1, int width1, int height1)
        {
            return x1 < 0 || x1 >= width1 || y1 < 0 || y1 >= height1;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Visible = false;
            Color pixel;
            double value;
            output = new Bitmap(input.Width, input.Height);
            if (pictureBox1.Image != null)
            {                
                if (comboBox1.SelectedIndex == 0)//ср. арифметическое
                {                
                    double[] mask = { 1 / 9, 1 / 9, 1 / 9, 1 / 9, 1 / 9, 1 / 9, 1 / 9, 1 / 9, 1 / 9 };
                    int sum = 0; int mid;
                    for (var i = 0; i < input.Width; i++)
                    {
                        for (var j = 0; j < input.Height; j++)
                        {
                            List<int[]> x = getVisiblePixels(i, j, 1, input.Width, input.Height);
                            for (var q = 0; q < x.Count; q++)
                            {
                                if(x[q][0]!=10000 && x[q][1] != 10000)
                                { 
                                    pixel = input.GetPixel(x[q][0], x[q][1]);
                                    value = Math.Round((double)pixel.R, 0);
                                    sum += (int)value;
                                }
                            }
                            mid = sum / 9;
                            if (mid > 255) mid = 255;
                            output.SetPixel(i, j, Color.FromArgb(mid, mid, mid));
                            sum = 0;
                        }
                    }                    
                } else if (comboBox1.SelectedIndex == 1)//средневзвешанное
                {
                    double[] mask = { 16, 8, 16, 8, 4, 8, 16, 8, 16 };
                    for (var i = 0; i < input.Width; i++)
                    {
                        for (var j = 0; j < input.Height; j++)
                        {
                            List<int[]> x = getVisiblePixels(i, j, 1, input.Width, input.Height);
                            int sum = 0;
                            for (var q = 0; q < x.Count; q++)
                            {
                                if (x[q][0] == 10000 && x[q][1] == 10000)
                                    value = 0;
                                else
                                {
                                    pixel = input.GetPixel(x[q][0], x[q][1]);
                                    value = Math.Round((double)pixel.R, 0) / mask[q];
                                    sum += (int)value;
                                }
                            }
                            if (sum > 255) sum = 255;
                            output.SetPixel(i, j, Color.FromArgb(sum, sum, sum));
                        }                                        
                    }
                }
                else if (comboBox1.SelectedIndex == 2)//распределение по Гауссу
                {
                    int imageSize = input.Width * input.Height;
                    double[] mask = { 1.11751, 1.05712, 1.11751, 1.05712, 0, 1.05712, 1.11751, 1.05712, 1.11751 };
                    int[] brightnessesCount = new int[256];
                    int sum = 0;
                    for (var i = 0; i < input.Width; i++)
                    {
                        for (var j = 0; j < input.Height; j++)
                        {
                            List<int[]> x = getVisiblePixels(i, j, 1, input.Width, input.Height);
                            for (var q = 0; q < x.Count; q++)
                            {

                                if (x[q][0] == 10000 && x[q][1] == 10000)
                                {
                                    value = 0;
                                }
                                else
                                {
                                    pixel = input.GetPixel(x[q][0], x[q][1]);
                                    value = (Math.Round((double)pixel.R, 0) * mask[q]) / 9;
                                    sum += (int)value;
                                }
                            }
                            if (sum > 255)
                            {
                                sum = 255;
                            }
                            output.SetPixel(i, j, Color.FromArgb(sum, sum, sum));
                            sum = 0;
                        }
                    } 
                }
                pictureBox1.Image = output;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Visible = false;
            if (pictureBox1.Image != null)
            {
                Bitmap output = new Bitmap(input.Width, input.Height);
                int imageSize = input.Width * input.Height;
                Color pixel; double value;
                int[] brightnessesCount = new int[256]; int temp;
                for (var i = 0; i < input.Width; i++)
                {
                    for (var j = 0; j < input.Height; j++)
                    {
                        List<int[]> x = getVisiblePixels(i, j, 1, input.Width, input.Height);
                        for (var q = 0; q < x.Count; q++)
                        {

                            if (x[q][0] == 10000 && x[q][1] == 10000)
                            {
                                value = 0;
                            }
                            else
                            {
                                pixel = input.GetPixel(x[q][0], x[q][1]);
                                value = (Math.Round((double)pixel.R, 0));
                                brightnessesCount[q] = (int)value;
                            }
                        }
                        if (comboBox2.SelectedIndex == 0)
                        {
                            for (var q = 0; q < x.Count; q++)
                            {
                                for (var s = q + 1; s < x.Count; s++)
                                {
                                    if (brightnessesCount[q] > brightnessesCount[s])
                                    {
                                        temp = brightnessesCount[q];
                                        brightnessesCount[q] = brightnessesCount[s];
                                        brightnessesCount[s] = temp;
                                    }
                                }
                            }
                            output.SetPixel(i, j, Color.FromArgb(brightnessesCount[5], brightnessesCount[5], brightnessesCount[5]));
                        }
                        else if (comboBox2.SelectedIndex == 1)
                        {
                            int min = brightnessesCount[0];
                            for (var s = 1; s < x.Count; s++)
                            {
                                if (brightnessesCount[s] < min)
                                    min = brightnessesCount[s];
                            }

                            output.SetPixel(i, j, Color.FromArgb(min, min, min));
                        }
                        else if (comboBox2.SelectedIndex == 2)
                        {
                            int max = brightnessesCount[0];
                            for (var s = 1; s < x.Count; s++)
                            {
                                if (brightnessesCount[s] > max)
                                    max = brightnessesCount[s];
                            }

                            output.SetPixel(i, j, Color.FromArgb(max, max, max));
                        }
                    }
                }
                pictureBox1.Image = output;
            }
        } 
        private void button3_Click(object sender, EventArgs e)
        {
            chart1.Visible = false;
            Bitmap output = new Bitmap(input.Width, input.Height);
            Color pixel;
            double value = 0;
            if (pictureBox1.Image != null)
            {
                double[] mask1 = { 0, 1, 0, 1, -4, 1, 0, 1, 0 };
                double[] mask2 = { 1, 1, 1, 1, -8, 1, 1, 1, 1 };
                for (var i = 0; i < input.Width; i++)
                {
                    for (var j = 0; j < input.Height; j++)
                    {
                        List<int[]> x = getVisiblePixels(i, j, 1, input.Width, input.Height);
                        double sum = 0;
                        double brightness = 0;
                        int newBrightness = 0;
                        for (var q = 0; q < x.Count; q++)
                        {
                            if (x[q][0] == 10000 && x[q][1] == 10000)
                            {
                                value = 0;
                            }
                            else
                            {
                                pixel = input.GetPixel(x[q][0], x[q][1]);
                                brightness = Math.Round((double)pixel.R, 0);
                                if (numericUpDown1.Value == 1) value = brightness * mask1[q];
                                if (numericUpDown1.Value == 2) value = brightness * mask2[q];
                                sum += value;
                            }
                        }
                        newBrightness = (int)brightness - (int)sum;
                        if (newBrightness > 255) newBrightness = 255;
                        if (newBrightness < 0) newBrightness = 0;
                        output.SetPixel(i, j, Color.FromArgb(newBrightness, newBrightness, newBrightness));
                    }
                }
                pictureBox1.Image = output;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            chart1.Visible = false;
            Bitmap output = new Bitmap(input.Width, input.Height);
            Color pixel;
            double Gx = 0; double Gy = 0; double Mxy = 0;
            double Gx1 = 0; double Gy1 = 0;
            double Gx2 = 0; double Gy2 = 0;
            if (pictureBox1.Image != null)
            {
                for (var i = 0; i < input.Width; i++)
                {
                    for (var j = 0; j < input.Height; j++)
                    {
                        List<int[]> x = getVisiblePixels(i, j, 1, input.Width, input.Height);
                        double brightness = 0;
                        Gx = 0; Gx1 = 0; Gy = 0; Gy1 = 0; Gy2 = 0; Gx2 = 0;
                        for (var q = 0; q < x.Count; q++)
                        {
                            if (x[q][0] == 10000 && x[q][1] == 10000)
                            {
                                continue;
                            }
                            else
                            {
                                pixel = input.GetPixel(x[q][0], x[q][1]);
                                brightness = Math.Round((double)pixel.R, 0);
                                if (comboBox3.SelectedIndex == 0) //оператор Робертса
                                {
                                    
                                    double[] mask1 = { 0, 0, 0, 0, -1, 0, 0, 0, 1 };
                                    double[] mask2 = { 0, 0, 0, 0, 0, 1, 0, -1, 0 };

                                    if (q == 4) Gx1 = brightness * mask1[q];
                                    if (q == 8) Gx = brightness * mask1[q] + Gx1;
                                    if (q == 5) Gy1 = brightness * mask2[q];
                                    if (q == 7) Gy = brightness * mask2[q] + Gy1;
                                }
                                else if (comboBox3.SelectedIndex == 1) //оператор Собеля
                                {
                                    
                                    double[] mask1 = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };
                                    double[] mask2 = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };

                                    if (q == 0) Gx1 += brightness * mask2[q];
                                    if (q == 1) Gx1 += 2 * brightness * mask2[q];
                                    if (q == 2) Gx1 += brightness * mask2[q];
                                    if (q == 6) Gx2 += brightness * mask2[q];
                                    if (q == 7) Gx2 += 2 * brightness * mask2[q];
                                    if (q == 8) { Gx2 += brightness * mask2[q]; Gx = Gx2 - Gx1; }

                                    if (q == 0) Gy1 += brightness * mask1[q];
                                    if (q == 3) Gy1 += 2 * brightness * mask1[q];
                                    if (q == 6) Gy1 += brightness * mask1[q];
                                    if (q == 2) Gy2 += brightness * mask1[q];
                                    if (q == 5) Gy2 += 2 * brightness * mask1[q];
                                    if (q == 8)
                                    {
                                        Gy2 += brightness * mask1[q];
                                        Gy = Gy2 - Gy1;
                                    }
                                }
                            }
                            Mxy = Gx + Gy;
                            if (comboBox3.SelectedIndex == 0) Mxy = Mxy * 4;
                            if (Mxy > 255) Mxy = 255;
                            if (Mxy < 0) Mxy = 0;
                            output.SetPixel(i, j, Color.FromArgb((int)Mxy, (int)Mxy, (int)Mxy));
                        }
                    }
                    pictureBox1.Image = output;
                }
            } 
        }
        private void button5_Click(object sender, EventArgs e)//Нерезкое маскирование
        {
            chart1.Visible = false;
            if (pictureBox1.Image != null)
            {
                mask = new Bitmap(input.Width, input.Height);
                output = new Bitmap(input.Width, input.Height);
                double[] matrix = new double[] { 0.3, 0.7, 0.3, 1, 2.5, 1, 0.3, 0.7, 0.3 };
                double sum = 0;
                for (int j = 1; j < input.Height - 1; j++)
                    for (int i = 1; i < input.Width - 1; i++)
                    {
                        sum = (input.GetPixel(i - 1, j - 1).B * matrix[0] + input.GetPixel(i - 1, j).B * matrix[1] + input.GetPixel(i - 1, j + 1).B * matrix[2] + input.GetPixel(i, j - 1).B * matrix[3] + input.GetPixel(i, j).B * matrix[4] + input.GetPixel(i, j + 1).B * matrix[5] + input.GetPixel(i + 1, j - 1).B * matrix[6] + input.GetPixel(i + 1, j).B * matrix[7] + input.GetPixel(i + 1, j + 1).B * matrix[8]) / 7;
                        if (sum > 255)
                            sum = 255;
                        output.SetPixel(i, j, Color.FromArgb((int)sum , (int)sum , (int)sum));                        
                    }
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        int R = input.GetPixel(i, j).B - output.GetPixel(i, j).B;
                        if (R > 255)
                            R = 255;
                        if (R < 0)
                            R = 0;
                        mask.SetPixel(i, j, Color.FromArgb(R, R, R));
                        int res = input.GetPixel(i, j).B + trackBar1.Value * mask.GetPixel(i, j).B;
                        if (res > 255)
                            res = 255;
                        if (res < 0)
                            res = 0;
                        output.SetPixel(i ,j, Color.FromArgb(res, res, res));
                    }
                pictureBox1.Image = output;
            }
        }

        private void button6_Click(object sender, EventArgs e)//Применение НМ
        {
            if (pictureBox1.Image != null)
            {
                output = new Bitmap(input.Width, input.Height);
                int s, u1, u2, u3;
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        int B = input.GetPixel(i, j).B;
                        if (B < 96)
                        {
                            u1 = 1; u2 = 0; u3 = 0;

                            s = (u1 * 16 + u2 * 127 + u3 * 239) / (u1 + u2 + u3);
                            output.SetPixel(i, j, Color.FromArgb(s, s, s));
                        }                            
                        else if (B > 192)
                        {
                            u1 = 0; u2 = 0; u3 = 1;

                            s = (u1 * 16 + u2 * 127 + u3 * 239) / (u1 + u2 + u3);
                            output.SetPixel(i, j, Color.FromArgb(s, s, s));
                        }
                        else if(B >= 96 && B < 127)
                        {
                            u2 = (1 - 0) / (126 - 96) * B - (1 - 0) * 96 / (126 - 96) + 0;
                            if (u2 < 0)
                                u2 = 0;
                            if (u2 > 255)
                                u2 = 255;

                            u1 = (0 - 1) / (127 - 96) * B - (0 - 1) * 96 / (127 - 96) + 1;
                            if (u1 < 0)
                                u1 = 0;
                            if (u1 > 255)
                                u1 = 255;

                            u3 = (1 - 0) / (192 - 127) * B - (1 - 0) * 127 / (192 - 127) + 0;
                            if (u3 < 0)
                                u3 = 0;
                            if (u3 > 255)
                                u3 = 255;

                            s = (u1 * 16 + u2 * 127 + u3 * 239) / (u1 + u2 + u3);
                            output.SetPixel(i, j, Color.FromArgb(s, s, s));
                        }
                        else if (B >= 127 && B <= 192)
                        {
                            u2 = (0 - 1) / (192 - 127) * B - (0 - 1) * 127 / (192 - 127) + 1;
                            if (u2 < 0)
                                u2 = 0;
                            if (u2 > 255)
                                u2 = 255;

                            u1 = (0 - 1) / (127 - 96) * B - (0 - 1) * 96 / (127 - 96) + 1;
                            if (u1 < 0)
                                u1 = 0;
                            if (u1 > 255)
                                u1 = 255;

                            u3 = (1 - 0) / (192 - 127) * B - (1 - 0) * 127 / (192 - 127) + 0;
                            if (u3 < 0)
                                u3 = 0;
                            if (u3 > 255)
                                u3 = 255;

                            s = (u1 * 16 + u2 * 127 + u3 * 239) / (u1 + u2 + u3);
                            output.SetPixel(i, j, Color.FromArgb(s, s, s));
                        }                                                                 
                    }
            }
            chart1.Visible = true;
            pictureBox1.Image = output;
        }
        private void Form1_Load(object sender, EventArgs e){}
        private void trackBar1_Scroll(object sender, EventArgs e){}
        private void chart1_Click(object sender, EventArgs e) {}                             
    }
}