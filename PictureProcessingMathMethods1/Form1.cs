using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Bitmap input, output;
        public Form1()
        {
            InitializeComponent();
        }
        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(ofd.FileName);
                    input = new Bitmap(pictureBox1.Image);
                }
                catch 
                {
                    MessageBox.Show("Формат файла не поддерживается", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void grayButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {               	
                output = new Bitmap(input.Width, input.Height);
                int R, G, B, i, j;
                                                                              
                if(comboBox1.SelectedIndex == 0)
                {
                	for (j = 0; j < input.Height; j++)
                    	for (i = 0; i < input.Width; i++)
                    	{
                        	R = input.GetPixel(i, j).R;
                        	G = input.GetPixel(i, j).G;
                        	B = input.GetPixel(i, j).B;                        
                    		
                        	B = (int)(0.299 * R + 0.587 * G + 0.114 * B);
                			input.SetPixel(i, j, Color.FromArgb((int)B, (int)B, (int)B));
                		}
                }                
                if(comboBox1.SelectedIndex == 1)
                {
                	for (j = 0; j < input.Height; j++)
                    	for (i = 0; i < input.Width; i++)
                    	{
                        	R = input.GetPixel(i, j).R;
                        	G = input.GetPixel(i, j).G;
                        	B = input.GetPixel(i, j).B;                        
                    	
                        	B = Math.Max(R, Math.Max(G, B));
                			input.SetPixel(i, j, Color.FromArgb((int)B, (int)B, (int)B));
                		}
                }                
                if(comboBox1.SelectedIndex == 2)
                {
                	for (j = 0; j < input.Height; j++)
                    	for (i = 0; i < input.Width; i++)
                    	{
                        	R = input.GetPixel(i, j).R;
                        	G = input.GetPixel(i, j).G;
                        	B = input.GetPixel(i, j).B;                        
                    	
                		B = (R + G + B) / 3;
                		input.SetPixel(i, j, Color.FromArgb((int)B, (int)B, (int)B));
                		}
                }                
                if(comboBox1.SelectedIndex == 3)
                {
                	for (j = 0; j < input.Height; j++)
                    	for (i = 0; i < input.Width; i++)
                    	{
                        	R = input.GetPixel(i, j).R;
                        	G = input.GetPixel(i, j).G;
                        	B = input.GetPixel(i, j).B;                        
                    	
                		B = (Math.Min(R, Math.Min(G, B)) + Math.Max(R, Math.Max(G, B))) / 2;
                		input.SetPixel(i, j, Color.FromArgb((int)B, (int)B, (int)B));
                	    }
                }

                pictureBox1.Image = input;
            }
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = input;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                output = new Bitmap(input.Width, input.Height);
                try
                {
                    if (comboBox2.SelectedIndex == 0)
                    {
                        double treshold = Convert.ToDouble(textBox2.Text);

                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                output.SetPixel(i, j, output.GetPixel(i, j).GetBrightness() < treshold ? System.Drawing.Color.Black : System.Drawing.Color.White);
                            }
                    }
                    if (comboBox2.SelectedIndex == 1)
                    {
                        double diff = Convert.ToDouble(textBox3.Text);
                        double From = Convert.ToDouble(textBox4.Text);
                        double To = Convert.ToDouble(textBox5.Text);

                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;
                                if (B < 0)
                                    B = 0;
                                if (B > 255)
                                    B = 255;
                                if (B > From && B < To)
                                    output.SetPixel(i, j, Color.FromArgb((int)(diff * B), (int)(diff * B), (int)(diff * B)));
                                else
                                    output.SetPixel(i, j, Color.FromArgb(B, B, B));
                            }
                    }
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("Выполните пребразование к ч/б", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }                
                catch (System.FormatException)
                {
                    MessageBox.Show("Неверный формат ввода", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (System.ArgumentException)
                {
                    MessageBox.Show("Введите меньшую степень", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
                pictureBox1.Image = output;
            }
        }        
        void ComboBox2SelectedIndexChanged(object sender, EventArgs e)
		{
            if (comboBox2.SelectedIndex == 0)
            {
                textBox2.Visible = true;                
            }
            else
                textBox2.Visible = false;

            if (comboBox2.SelectedIndex == 1)
            {
                textBox3.Visible = true;                
                textBox4.Visible = true;                
                textBox5.Visible = true;                
            }
            else
            {
                textBox3.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
            }
		}
        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
                textBox2.Text = "Введите порог";
        } 
        private void textBox3_Enter(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }
        private void textBox3_Leave(object sender, EventArgs e)
        {
            if(textBox3.Text == "")
                textBox3.Text = "Введите коэф.";
        }
        private void textBox4_Enter(object sender, EventArgs e)
        {
            textBox4.Text = "";
        }
        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
                textBox4.Text = "От";
        }
        private void textBox5_Enter(object sender, EventArgs e)
        {
            textBox5.Text = "";
        }
        private void textBox5_Leave(object sender, EventArgs e)
        {
            if (textBox5.Text == "")
                textBox5.Text = "До";
        }
        private void button2_Click(object sender, EventArgs e)
        {            
            if (pictureBox1.Image != null)
            {
                output = new Bitmap(input.Width, input.Height);
                try
                {
                    for (int j = 0; j < input.Height; j++)
                        for (int i = 0; i < input.Width; i++)
                        {                            
                            int B = input.GetPixel(i, j).B;

                            output.SetPixel(i, j, Color.FromArgb(255 - B, 255 - B, 255 - B));
                        }
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("Выполните пребразование к ч/б", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                pictureBox1.Image = output;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {           
            if (pictureBox1.Image != null)
            {
                output = new Bitmap(input.Width, input.Height);
                try
                {
                    if (comboBox3.SelectedIndex == 0)
                    {
                        double c = (256 - 1) / Math.Log10(256 - 1 + 1);

                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j, Color.FromArgb((int)(c * Math.Log10(1 + B)), (int)(c * Math.Log10(1 + B)), (int)(c * Math.Log10(1 + B))));
                            }
                    }
                    if (comboBox3.SelectedIndex == 1)
                    {
                        double c = (256 - 1) / Math.Log(256 - 1 + 1);

                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j, Color.FromArgb((int)(c * Math.Log(1 + B)), (int)(c * Math.Log(1 + B)), (int)(c * Math.Log(1 + B))));
                            }
                    }
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("Выполните пребразование к ч/б", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                pictureBox1.Image = output;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                output = new Bitmap(input.Width, input.Height);
                try
                {
                    double y = Convert.ToDouble(textBox1.Text);
                    double c = 255 / Math.Pow(255, y);

                    for (int j = 0; j < input.Height; j++)
                        for (int i = 0; i < input.Width; i++)
                        {
                            int B = input.GetPixel(i, j).B;
                            B = (int)(c * Math.Pow(B, y));
                            if (B > 255)
                                B = 255;
                            if (B < 0)
                                B = 0;
                            output.SetPixel(i, j, Color.FromArgb(B, B, B));
                        }
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("Выполните пребразование к ч/б", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }                
                catch (System.FormatException)
                {
                    MessageBox.Show("Неверный формат ввода", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                pictureBox1.Image = output;
            }
        }
        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                textBox1.Text = "Введите степень";
        }
        private void button5_Click(object sender, EventArgs e)
        {                                   
            if(pictureBox1.Image != null)
            {
                output = new Bitmap(input.Width, input.Height);     
                try
                {
                    chart1.Series[0].Points.Clear();
                    int X1 = int.Parse(textBox6.Text);
                    int X2 = int.Parse(textBox7.Text);
                    int Y1 = int.Parse(textBox8.Text);
                    int Y2 = int.Parse(textBox9.Text);

                    if(X1 < 0 || X2 < 0 || Y1 < 0 || Y2 < 0 || X1 > 255 || X2 > 255 || Y1 > 255 || Y2 > 255)
                        throw new FormatException();
                    if(X1 > X2 || Y1 > Y2)
                        throw new FormatException();

                    for (int j = 0; j < input.Height; j++)
                        for (int i = 0; i < input.Width; i++)
                        {
                            int X = input.GetPixel(i, j).B;                            
                            int Y = (Y2 - Y1) / (X2 - X1) * X - (Y2 - Y1) * X1 / (X2 - X1) + Y1;
                            if (Y < 0)
                                Y = 0;
                            if (Y > 255)
                                Y = 255;
                            output.SetPixel(i, j, Color.FromArgb(Y, Y, Y));
                        }
                    chart1.Series[0].Points.AddXY(0, 0);
                    chart1.Series[0].Points.AddXY(X1, Y1);
                    chart1.Series[0].Points.AddXY(X2, Y2);
                    chart1.Series[0].Points.AddXY(255, 255);
                    chart1.Visible = true;
                    button7.Visible = true;
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("Выполните пребразование к ч/б", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (System.FormatException)
                {
                    MessageBox.Show("Неверный формат ввода", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                pictureBox1.Image = output;
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            chart1.Visible = false;
            chart1.Series[0].Points.Clear();
            button7.Visible = false;
        } 
        private void button6_Click(object sender, EventArgs e)
        {                        
            if(pictureBox1.Image != null)
            {
                output = new Bitmap(input.Width, input.Height);
                try
                {
            	    //if(comboBox4.SelectedIndex == 0)
                    
                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {                                
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j,( (B << (7 - comboBox4.SelectedIndex) )% 256) >= 128 ? System.Drawing.Color.White : System.Drawing.Color.Black);
                            }

                    /*if (comboBox4.SelectedIndex == 1)
                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j, B << (7 - 1) % 256 >= 128 ? System.Drawing.Color.White : System.Drawing.Color.Black);
                            }
                    if (comboBox4.SelectedIndex == 2)
                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j, B << (7 - 2) % 256 >= 128 ? System.Drawing.Color.White : System.Drawing.Color.Black);
                            }
                    if (comboBox4.SelectedIndex == 3)
                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j, B << (7 - 3) % 256 >= 128 ? System.Drawing.Color.White : System.Drawing.Color.Black);
                            }
                    if (comboBox4.SelectedIndex == 4)
                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j, B << (7 - 4) % 256 >= 128 ? System.Drawing.Color.White : System.Drawing.Color.Black);
                            }
                    if (comboBox4.SelectedIndex == 5)
                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j, B << (7 - 5) % 256 >= 128 ? System.Drawing.Color.White : System.Drawing.Color.Black);
                            }
                    if (comboBox4.SelectedIndex == 6)
                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j, B << (7 - 6) % 256 >= 128 ? System.Drawing.Color.White : System.Drawing.Color.Black);
                            }
                    if (comboBox4.SelectedIndex == 7)
                        for (int j = 0; j < input.Height; j++)
                            for (int i = 0; i < input.Width; i++)
                            {
                                int B = input.GetPixel(i, j).B;

                                output.SetPixel(i, j, B << (7 - 7) % 256 >= 128 ? System.Drawing.Color.White : System.Drawing.Color.Black);
                            }*/
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("Выполните пребразование к ч/б", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                pictureBox1.Image = output;
            }
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
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
        private void textBox6_Enter(object sender, EventArgs e)
        {
            textBox6.Text = "";
        }
        private void textBox7_Enter(object sender, EventArgs e)
        {
            textBox7.Text = "";
        }
        private void textBox8_Enter(object sender, EventArgs e)
        {
            textBox8.Text = "";
        }
        private void textBox9_Enter(object sender, EventArgs e)
        {
            textBox9.Text = "";
        }
        private void textBox6_Leave(object sender, EventArgs e)
        {
            if (textBox6.Text == "")
                textBox6.Text = "Введите x1";
        }
        private void textBox7_Leave(object sender, EventArgs e)
        {
            if (textBox7.Text == "")
                textBox7.Text = "Введите x2";
        }
        private void textBox8_Leave(object sender, EventArgs e)
        {
            if (textBox8.Text == "")
                textBox8.Text = "Введите y1";
        }
        private void textBox9_Leave(object sender, EventArgs e)
        {
            if (textBox9.Text == "")
                textBox9.Text = "Введите y2";
        }                                                                                                          		
    }
}