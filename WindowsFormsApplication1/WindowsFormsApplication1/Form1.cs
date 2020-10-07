using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
            button5.Visible = false;
            /// button4.Text = "復原";
        }
        Image<Bgr, byte> dest = null;
        Image<Bgr, byte> generate = null;
        Image<Bgr, byte> dummy = null;
        String filename;
        Size size;
        private Capture _capture = null;
        private bool _captureInProgress;
        Image<Bgr, Byte> frame;

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "影像(*.jpg/*.png/*.gif/*.bmp)|*.jpg;*.png;*.gif;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                label1.Visible = true;
                filename = dialog.FileName;
                IntPtr image = CvInvoke.cvLoadImage(filename,
               Emgu.CV.CvEnum.LOAD_IMAGE_TYPE.CV_LOAD_IMAGE_ANYCOLOR);
                size = CvInvoke.cvGetSize(image);
                dest = new Image<Bgr, byte>(size);
                dummy = new Image<Bgr, byte>(size);
                generate = new Image<Bgr, byte>(size);
                CvInvoke.cvCopy(image, dest, IntPtr.Zero);
                pictureBox1.Image = dest.ToBitmap();

            }
        }
        private void ProcessFrame(object sender, EventArgs arg)
        {
            frame = _capture.QueryFrame();
            pictureBox1.Image = frame.ToBitmap();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                for (int i = 0; i < size.Width; i++)
                {
                    for (int j = 0; j < size.Height; j++)
                    {
                        generate.Data[j, i, 0] = (byte)(255 - (int)(dest.Data[j, i, 0])); // blue
                        generate.Data[j, i, 1] = (byte)(255 - (int)(dest.Data[j, i, 1])); // green
                        generate.Data[j, i, 2] = (byte)(255 - (int)(dest.Data[j, i, 2])); // red
                    }
                }
                pictureBox2.Image = generate.ToBitmap();

            }

            if (radioButton2.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                Bitmap bm1 = (Bitmap)pictureBox1.Image;
                int w1 = pictureBox1.Image.Width;
                int h1 = pictureBox1.Image.Height;
                int x;
                int y;
                for (y = 0; y <= h1 - 1; y++)
                {
                    for (x = 0; x <= w1 - 1; x++)
                    {
                        Color c1 = bm1.GetPixel(x, y);
                        int r1 = c1.R;
                        int g1 = c1.G;
                        int b1 = c1.B;
                        int avg1 = (r1 + g1 + b1) / 3;
                        bm1.SetPixel(x, y, Color.FromArgb(avg1, avg1, avg1));
                    }
                }
                pictureBox2.Image = bm1;
            }
            if (radioButton3.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                //拉普拉斯模板
                int[] Laplacian = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
                for (int x = 1; x < Width - 1; x++)
                    for (int y = 1; y < Height - 1; y++)
                    {
                        int r = 0, g = 0, b = 0;
                        int Index = 0;
                        for (int col = -1; col <= 1; col++)
                            for (int row = -1; row <= 1; row++)
                            {
                                pixel = oldBitmap.GetPixel(x + row, y + col); r += pixel.R * Laplacian[Index];
                                g += pixel.G * Laplacian[Index];
                                b += pixel.B * Laplacian[Index];
                                Index++;
                            }
                        //處理顏色值溢出
                        r = r > 255 ? 255 : r;
                        r = r < 0 ? 0 : r;
                        g = g > 255 ? 255 : g;
                        g = g < 0 ? 0 : g;
                        b = b > 255 ? 255 : b;
                        b = b < 0 ? 0 : b;
                        newBitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                    }
                this.pictureBox2.Image = newBitmap;
            }
            if (radioButton4.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                for (int x = 1; x < Width - 1; x++)
                    for (int y = 1; y < Height - 1; y++)
                    {
                        System.Random MyRandom = new Random();
                        int k = MyRandom.Next(123456);
                        //像素塊大小
                        int dx = x + k % 19;
                        int dy = y + k % 19;
                        if (dx >= Width)
                            dx = Width - 1;
                        if (dy >= Height)
                            dy = Height - 1;
                        pixel = oldBitmap.GetPixel(dx, dy);
                        newBitmap.SetPixel(x, y, pixel);
                    }
                this.pictureBox2.Image = newBitmap;
            }

            if (radioButton5.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
               
                for (int x = 0; x < Width - 1; x++)
                {
                    for (int y = 0; y < Height - 1; y++)
                    {
                        int r = 0, g = 0, b = 0;
                        pixel1 = oldBitmap.GetPixel(x, y); int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel1, pixel2;
                        pixel2 = oldBitmap.GetPixel(x + 1, y + 1);
                        r = Math.Abs(pixel1.R - pixel2.R + 128);
                        g = Math.Abs(pixel1.G - pixel2.G + 128);
                        b = Math.Abs(pixel1.B - pixel2.B + 128);
                        if (r > 255)
                            r = 255;
                        if (r < 0)
                            r = 0;
                        if (g > 255)
                            g = 255;
                        if (g < 0)
                            g = 0;
                        if (b > 255)
                            b = 255;
                        if (b < 0)
                            b = 0;
                        newBitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }
                this.pictureBox2.Image = newBitmap;
            }
            if (radioButton6.Checked)
            {
                groupBox2.Visible = true;
                groupBox3.Visible = true;
                groupBox4.Visible = true;
                int blue_channel = 0, green_channel = 0, red_channel = 0;
                for (int i = 0; i < groupBox2.Controls.Count; i++)
                {
                    if (((RadioButton)groupBox2.Controls[i]).Checked)
                    {
                        blue_channel = i;
                    }
                }
                for (int i = 0; i < groupBox3.Controls.Count; i++)
                {
                    if (((RadioButton)groupBox3.Controls[i]).Checked)
                    {
                        green_channel = i;
                    }
                }
                for (int i = 0; i < groupBox4.Controls.Count; i++)
                {
                    if (((RadioButton)groupBox4.Controls[i]).Checked)
                    {
                        red_channel = i;
                    }
                }
                for (int i = 0; i < size.Width; i++)
                {
                    for (int j = 0; j < size.Height; j++)
                    {
                        generate.Data[j, i, 0] = dest.Data[j, i, blue_channel]; // blue
                        generate.Data[j, i, 1] = dest.Data[j, i, green_channel]; // green
                        generate.Data[j, i, 2] = dest.Data[j, i, red_channel]; // red
                    }
                }
                pictureBox2.Image = generate.ToBitmap();
            }
            if (radioButton7.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                //拉普拉斯模板
                int[] Laplacian = { 9, 0, 9, 0, -40, 0, 9, 0, 9 };
                for (int x = 1; x < Width - 1; x++)
                    for (int y = 1; y < Height - 1; y++)
                    {
                        int r = 0, g = 0, b = 0;
                        int Index = 0;
                        for (int col = -1; col <= 1; col++)
                            for (int row = -1; row <= 1; row++)
                            {
                                pixel = oldBitmap.GetPixel(x + row, y + col); r += pixel.R * Laplacian[Index];
                                g += pixel.G * Laplacian[Index];
                                b += pixel.B * Laplacian[Index];
                                Index++;
                            }
                        //處理顏色值溢出
                        r = r > 255 ? 255 : r;
                        r = r < 0 ? 0 : r;
                        g = g > 255 ? 255 : g;
                        g = g < 0 ? 0 : g;
                        b = b > 255 ? 255 : b;
                        b = b < 0 ? 0 : b;
                        newBitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                    }
                this.pictureBox2.Image = newBitmap;
            }
            if (radioButton8.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                //拉普拉斯模板
                int[] Laplacian = { 2, 2, -2, 2, 2, -2, 2, 2, -2 };
                for (int x = 1; x < Width - 1; x++)
                    for (int y = 1; y < Height - 1; y++)
                    {
                        int r = 0, g = 0, b = 0;
                        int Index = 0;
                        for (int col = -1; col <= 1; col++)
                            for (int row = -1; row <= 1; row++)
                            {
                                pixel = oldBitmap.GetPixel(x + row, y + col); r += pixel.R * Laplacian[Index];
                                g += pixel.G * Laplacian[Index];
                                b += pixel.B * Laplacian[Index];
                                Index++;
                            }
                        //處理顏色值溢出
                        r = r > 255 ? 255 : r;
                        r = r < 0 ? 0 : r;
                        g = g > 255 ? 255 : g;
                        g = g < 0 ? 0 : g;
                        b = b > 255 ? 255 : b;
                        b = b < 0 ? 0 : b;
                        newBitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                    }
                this.pictureBox2.Image = newBitmap;
            }
            if (radioButton9.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                //拉普拉斯模板
                int[] Laplacian = { -3, +9, +19, -20, +7, +2, -10, +3, -7 };
                for (int x = 1; x < Width - 1; x++)
                    for (int y = 1; y < Height - 1; y++)
                    {
                        int r = 0, g = 0, b = 0;
                        int Index = 0;
                        for (int col = -1; col <= 1; col++)
                            for (int row = -1; row <= 1; row++)
                            {
                                pixel = oldBitmap.GetPixel(x + row, y + col); r += pixel.R * Laplacian[Index];
                                g += pixel.G * Laplacian[Index];
                                b += pixel.B * Laplacian[Index];
                                Index++;
                            }
                        //處理顏色值溢出
                        r = r > 255 ? 255 : r;
                        r = r < 0 ? 0 : r;
                        g = g > 255 ? 255 : g;
                        g = g < 0 ? 0 : g;
                        b = b > 255 ? 255 : b;
                        b = b < 0 ? 0 : b;
                        newBitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                    }
                this.pictureBox2.Image = newBitmap;
            }
            if (radioButton19.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                //拉普拉斯模板
                int[] Laplacian = { +2, -1, +4, -3, 0, -8, +10, -1,0 };
                for (int x = 1; x < Width - 1; x++)
                    for (int y = 1; y < Height - 1; y++)
                    {
                        int r = 0, g = 0, b = 0;
                        int Index = 0;
                        for (int col = -1; col <= 1; col++)
                            for (int row = -1; row <= 1; row++)
                            {
                                pixel = oldBitmap.GetPixel(x + row, y + col); r += pixel.R * Laplacian[Index];
                                g += pixel.G * Laplacian[Index];
                                b += pixel.B * Laplacian[Index];
                                Index++;
                            }
                        //處理顏色值溢出
                        r = r > 255 ? 255 : r;
                        r = r < 0 ? 0 : r;
                        g = g > 255 ? 255 : g;
                        g = g < 0 ? 0 : g;
                        b = b > 255 ? 255 : b;
                        b = b < 0 ? 0 : b;
                        newBitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                    }
                this.pictureBox2.Image = newBitmap;
            }
            if (radioButton20.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                for (int i = 0; i < size.Width; i++)
                {
                    for (int j = 0; j < size.Height; j++)
                    {
                        generate.Data[j, i, 0] = (byte)((int)(dest.Data[j, i, 0])); // blue
                        generate.Data[j, i, 1] = (byte)((int)(dest.Data[j, i, 1])); // green
                        generate.Data[j, i, 2] = (byte)((int)(dest.Data[j, i, 2])); // red
                    }
                }
                for (int i = 0; i < size.Width; i++)
                {
                    for (int j = 0; j < size.Height; j++)
                    {
                        if (generate.Data[j, i, 0] >= 0 && generate.Data[j, i, 0] <= 255 && generate.Data[j, i, 1] >= 0 && generate.Data[j, i, 1] <= 255 && generate.Data[j, i, 2] >= 0 && generate.Data[j, i, 2] <= 255)
                        {
                            generate.Data[j, i, 0] = 255;
                            generate.Data[j, i, 0] = 0;
                            generate.Data[j, i, 0] = 0;
                        }
                    }
                }
                pictureBox2.Image = generate.ToBitmap();
            }
            if (radioButton21.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                for (int i = 0; i < size.Width; i++)
                {
                    for (int j = 0; j < size.Height; j++)
                    {
                        generate.Data[j, i, 0] = (byte)((int)(dest.Data[j, i, 0])); // blue
                        generate.Data[j, i, 1] = (byte)((int)(dest.Data[j, i, 1])); // green
                        generate.Data[j, i, 2] = (byte)((int)(dest.Data[j, i, 2])); // red
                    }
                }
                for (int i = 0; i < size.Width; i++)
                {
                    for (int j = 0; j < size.Height; j++)
                    {
                        if (generate.Data[j, i, 0] >= 0 && generate.Data[j, i, 0] <= 255 && generate.Data[j, i, 1] >= 0 && generate.Data[j, i, 1] <= 255 && generate.Data[j, i, 2] >= 0 && generate.Data[j, i, 2] <= 255)
                        {
                            generate.Data[j, i, 1] = 255;
                            generate.Data[j, i, 1] = 0;
                            generate.Data[j, i, 1] = 0;

                        }
                    }
                }
                pictureBox2.Image = generate.ToBitmap();
            }
            if (radioButton22.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                for (int i = 0; i < size.Width; i++)
                {
                    for (int j = 0; j < size.Height; j++)
                    {
                        generate.Data[j, i, 0] = (byte)((int)(dest.Data[j, i, 0])); // blue
                        generate.Data[j, i, 1] = (byte)((int)(dest.Data[j, i, 1])); // green
                        generate.Data[j, i, 2] = (byte)((int)(dest.Data[j, i, 2])); // red
                    }
                }
                for (int i = 0; i < size.Width; i++)
                {
                    for (int j = 0; j < size.Height; j++)
                    {
                        if (generate.Data[j, i, 0] >= 0 && generate.Data[j, i, 0] <= 255 && generate.Data[j, i, 1] >= 0 && generate.Data[j, i, 1] <= 255 && generate.Data[j, i, 2] >= 0 && generate.Data[j, i, 2] <= 255)
                        {
                            generate.Data[j, i, 2] = 255;
                            generate.Data[j, i, 2] = 0;
                            generate.Data[j, i, 2] = 0;

                        }
                    }
                }
                pictureBox2.Image = generate.ToBitmap();
            }
            if (radioButton23.Checked)
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                //拉普拉斯模板
                int[] Laplacian = { 0, 1,0,1,1, 1, 0, 1, 0 };
                for (int x = 1; x < Width - 1; x++)
                    for (int y = 1; y < Height - 1; y++)
                    {
                        int r = 0, g = 0, b = 0;
                        int Index = 0;
                        for (int col = -1; col <= 1; col++)
                            for (int row = -1; row <= 1; row++)
                            {
                                pixel = oldBitmap.GetPixel(x + row, y + col); r += pixel.R * Laplacian[Index];
                                g += pixel.G * Laplacian[Index];
                                b += pixel.B * Laplacian[Index];
                                Index++;
                            }
                        //處理顏色值溢出
                        r = r > 255 ? 255 : r;
                        r = r < 0 ? 0 : r;
                        g = g > 255 ? 255 : g;
                        g = g < 0 ? 0 : g;
                        b = b > 255 ? 255 : b;
                        b = b < 0 ? 0 : b;
                        newBitmap.SetPixel(x - 1, y - 1, Color.FromArgb(r, g, b));
                    }
                this.pictureBox2.Image = newBitmap;
            }
        }
       

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < size.Width; i++)
            {
                for (int j = 0; j < size.Height; j++)
                {
                    generate.Data[j, i, 0] = (byte)((int)(dest.Data[j, i, 0])); // blue
                    generate.Data[j, i, 1] = (byte)((int)(dest.Data[j, i, 1])); // green
                    generate.Data[j, i, 2] = (byte)((int)(dest.Data[j, i, 2])); // red
                }
            }
            pictureBox2.Image = generate.ToBitmap();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            #region if capture is not created, create it now
            if (_capture == null)
            {
                try
                {
                    _capture = new Capture();
                    _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS, 30);
                    _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 240);
                    _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 320);
                }

                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
            #endregion
            if (_capture != null)
            {
                if (_captureInProgress)
                { //stop the capture
                    Application.Idle -= new EventHandler(ProcessFrame);
                    button4.Text = "Start";
                    button5.Visible = true;
                }
                else
                {
                    //start the capture
                    Application.Idle += new EventHandler(ProcessFrame);
                    button4.Text = "Stop";
                    button5.Visible = false;
                }
                _captureInProgress = !_captureInProgress;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "影像(*.jpg/*.png/*.gif/*.bmp)|*.jpg;*.png;*.gif;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filename = dialog.FileName;
                pictureBox1.Image.Save(filename);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "影像(*.jpg/*.png/*.gif/*.bmp)|*.jpg;*.png;*.gif;*.bmp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filename = dialog.FileName;
                pictureBox2.Image.Save(filename);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < size.Width; i++)
            {
                for (int j = 0; j < size.Height; j++)
                {
                    generate.Data[j, i, 0] = (byte)((int)(dest.Data[j, i, 0])); // blue
                    generate.Data[j, i, 1] = (byte)((int)(dest.Data[j, i, 1])); // green
                    generate.Data[j, i, 2] = (byte)((int)(dest.Data[j, i, 2])); // red
                }
            }
            for (int i = 0; i < size.Width; i++)
            {
                for (int j = 0; j < size.Height; j++)
                {
                    if (generate.Data[j, i, 0] >= 0 && generate.Data[j, i, 0] <= 255 && generate.Data[j, i, 1] >= 0 && generate.Data[j, i, 1] <= 255 && generate.Data[j, i, 2] >= 0 && generate.Data[j, i, 2] <= 255)
                    {
                        if (generate.Data[j, i, 0] >= 210 && generate.Data[j, i, 1] >= 210 && generate.Data[j, i, 2] >= 210)
                        {
                            if (i == 0 || j == 0 || i > Width-1 || j > Height-1)
                            {
                                generate.Data[j, i, 0] = (byte)((int)dest.Data[j, i, 0]);
                                generate.Data[j, i, 1] = (byte)((int)dest.Data[j, i, 1]);
                                generate.Data[j, i, 2] = (byte)((int)dest.Data[j, i, 2]);
                            }
                            else
                            {
                                generate.Data[j, i, 0] = (byte)((int)(generate.Data[j - 1, i - 1, 0] + generate.Data[j - 1, i - 1, 0] + generate.Data[j, i - 1, 0] + generate.Data[j, i, 0] )/4);
                                generate.Data[j, i, 1] = (byte)((int)(generate.Data[j - 1, i - 1, 1] + generate.Data[j - 1, i - 1, 1] + generate.Data[j, i - 1, 1] + generate.Data[j, i, 1] )/4);
                                generate.Data[j, i, 2] = (byte)((int)(generate.Data[j - 1, i - 1, 2] + generate.Data[j - 1, i - 1, 2] + generate.Data[j, i - 1, 2] + generate.Data[j, i, 2] )/4);
                            }
                        }    
                    }
                }
            }
            pictureBox2.Image = generate.ToBitmap();
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

    }
}