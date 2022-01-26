using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create a Bitmap object from a file.
            Bitmap myBitmap = new Bitmap(1000, 1000);

            long tickCount64 = Environment.TickCount64;

            // Set each pixel in myBitmap to black.
            for (int Xcount = 0; Xcount < myBitmap.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < myBitmap.Height; Ycount++)
                {
                    myBitmap.SetPixel(Xcount, Ycount, Color.FromArgb(0x88, 0xff, 0, 0));
                }
            }

            long tickCount642 = Environment.TickCount64;
            long v = tickCount642 - tickCount64;
            Debug.WriteLine($"pre: {v}");

            pictureBox1.Image = myBitmap;
        }

        IntPtr ppp;
        private void button2_Click(object sender, EventArgs e)
        {
            long tickCount64 = Environment.TickCount64;

            unsafe
            {
                int w = 10;
                int h = 10;
                IntPtr intPtr = Marshal.AllocHGlobal(w * h * 4);
                ppp = intPtr;

                uint* pix = (uint*)intPtr;

                for (int ih = 0; ih < h; ih++)
                {
                    for (int iw = 0; iw < w; iw++)
                    {
                        pix[ih * w + iw] = 0x880000ff;
                    }
                }

                long tickCount642 = Environment.TickCount64;
                long v = tickCount642 - tickCount64;
                Debug.WriteLine($"pre2: {v}");

                //Create a new bitmap.
                Bitmap newBitmap = new Bitmap(w, h, w * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, intPtr);
                pictureBox2.Image = newBitmap;
                //newBitmap.Save("bmp.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Marshal.FreeHGlobal(ppp);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(@"C:\Users\luckh\Downloads\1122.png");
            //bitmap.Save("2.bmp", ImageFormat.Bmp);

            Bitmap bitmap1 = new Bitmap("bmp.bmp");
            Color color = bitmap1.GetPixel(0, 0);

            // Retrieve the bitmap data from the bitmap.
            System.Drawing.Imaging.BitmapData bmpData = bitmap1.LockBits(new Rectangle(0, 0, bitmap1.Width, bitmap1.Height),
                ImageLockMode.ReadOnly, bitmap1.PixelFormat);


            Bitmap newBitmap = new Bitmap(bmpData.Width, bmpData.Height, bmpData.Stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, bmpData.Scan0);

            pictureBox2.Image = newBitmap;

            bitmap1.UnlockBits(bmpData);

        }
    }
}