using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using WIA;
using System.Threading;

namespace AssistScan
{
    public partial class Form1 : Form
    {
        private Point startPoint;
        private Point endPoint;


        int x1;
        int y1;
        int x2;
        int y2;
        private bool isDrawing = false;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set the properties of the OpenFileDialog
            openFileDialog1.Filter = "Image Files (*.bmp, *.jpg, *.jpeg, *.gif, *.png)|*.bmp;*.jpg;*.jpeg;*.gif;*.png";
            openFileDialog1.Title = "Select an Image File";

            // Show the OpenFileDialog and wait for the user to select a file
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Load the selected file into an Image object
                Image image = Image.FromFile(openFileDialog1.FileName);

                // Set the Image property of the PictureBox to the loaded image
                pictureBox1.Image = image;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            // Set the properties of the SaveFileDialog
            saveFileDialog1.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png";
            saveFileDialog1.Title = "Save an Image File";

            // Show the SaveFileDialog and wait for the user to select a file name and location
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the file name and extension from the SaveFileDialog
                string fileName = saveFileDialog1.FileName;

                // Get the image from the PictureBox control
                Image image = pictureBox1.Image;

                // Save the image to the selected file in the selected format
                image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg); // or use ImageFormat.Bmp or ImageFormat.Png
            }
        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            isDrawing = true;

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // Set the ending point of the rectangle
            //endPoint = e.Location;
            isDrawing = false;

            // Force the PictureBox to repaint itself to show the new rectangle
            pictureBox1.Invalidate();

                       

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // If the user is drawing a rectangle (i.e. left mouse button is down)
            if (isDrawing)
            {

                // Set the current endpoint of the rectangle to the current mouse position
                endPoint = e.Location;
                if (endPoint.X<0)
                {
                    endPoint.X = 0;
                }
                if (endPoint.Y<0)
                {
                    endPoint.Y = 0;
                }


                // Force the PictureBox to repaint itself to show the updated rectangle
                pictureBox1.Invalidate();
            }
            

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // If the user is drawing a rectangle
            if (isDrawing)
            {

                x1 = startPoint.X;
                y1 = startPoint.Y;
                x2 = endPoint.X;
                y2 = endPoint.Y;
                int tmp = 0;

                if (x2 < x1)
                {
                    tmp = x1; x1 = x2; x2 = tmp;
                }
                if(y2<y1)
                {
                    tmp = y1;y1 = y2;y2 = tmp;
                }


                // Calculate the width and height of the rectangle
                int width = Math.Abs(x2 - x1);
                int height = Math.Abs(y2 - y1);



                // Draw the rectangle using the current pen and the calculated position and size
                Pen pen = new Pen(Color.Red, 2);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                e.Graphics.DrawRectangle(pen, x1, y1, width, height);

            }
            fn_imagecrop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += Application.ProductVersion.ToString();
        }


        private void fn_imagecrop()
        {
            //MessageBox.Show(pictureBox1.Image.Width.ToString() + "" + pictureBox1.Width);
            // Get the position and size of the crop area
            float a = pictureBox1.Image.Width / (float)pictureBox1.Width;
            float b = pictureBox1.Image.Height / (float)pictureBox1.Height;
            //MessageBox.Show(a.ToString());

            int x = (int)(x1 * a); // example x position
            int y = (int)(y1*b); // example y position
            
            int width = (int)(x2*a) - x;
            int height = (int)(y2*b) - y;

            if (width<=0 || height<=0)
            {
                return;
            }

            // Create a new Bitmap object to hold the cropped image
            Bitmap bmp = new Bitmap(width, height);

            // Create a Graphics object from the Bitmap
            Graphics g = Graphics.FromImage(bmp);

            // Draw the portion of the image specified by the crop area onto the Graphics object
            g.DrawImage(pictureBox1.Image, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);

            // Dispose of the Graphics object to free up resources
            g.Dispose();

            // Set the cropped image as the image displayed in the second PictureBox control
            pictureBox2.Image = bmp;

        }
        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            // Set the properties of the SaveFileDialog
            saveFileDialog1.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png";
            saveFileDialog1.Title = "Save an Image File";

            // Show the SaveFileDialog and wait for the user to select a file name and location
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the file name and extension from the SaveFileDialog
                string fileName = saveFileDialog1.FileName;

                // Get the image from the PictureBox control
                Image image = pictureBox2.Image;

                // Save the image to the selected file in the selected format
                image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg); // or use ImageFormat.Bmp or ImageFormat.Png
            }
        }
        private void printPictureBox(PictureBox pictureBox,string mode)
        {
            // Create a PrintDocument object
            PrintDocument printDocument = new PrintDocument();

            // Set the PrintPage event handler
            printDocument.PrintPage += (sender, e) =>
            {
                // Get the image from the PictureBox control
                Image image = pictureBox.Image;

                // Calculate the size and position of the image on the page
                float pageWidth = e.PageSettings.PrintableArea.Width;
                float pageHeight = e.PageSettings.PrintableArea.Height;
                float imageWidth = image.Width;
                float imageHeight = image.Height;
                float scaleX = pageWidth / imageWidth;
                float scaleY = pageHeight / imageHeight;
                float scale = Math.Min(scaleX, scaleY);
                float offsetX = (pageWidth - imageWidth * scale) / 2;
                float offsetY = (pageHeight - imageHeight * scale) / 2;
                RectangleF imageRect = new RectangleF(offsetX, offsetY, imageWidth * scale, imageHeight * scale);

                // Draw the image on the page
                e.Graphics.DrawImage(image, imageRect);
            };

            // Set the page settings to A4 size
            if (mode == "A4")
            {
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
            }
            else
            {
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("A5", 583, 827);

            }


            // Create a Print Dialog

            // Set the Print Document property
            printDialog1.Document = printDocument;

            // Show the Print Dialog
            DialogResult result = printDialog1.ShowDialog();

            // If the user clicks OK, print the document
            if (result == DialogResult.OK)
            {
                printDocument.Print();
            }


        }
      
        private void button6_Click_1(object sender, EventArgs e)
        {
            printPictureBox(pictureBox2,"A5");
        }
        
        private void convertPictureBoxToGrayscale(PictureBox pictureBox)
        {
            // Get the image from the PictureBox control
            Image image = pictureBox.Image;

            // Create a grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
            new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
            new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
            new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 },
            new float[] { 0, 0, 0, 0, 1 }
                });

            // Create an ImageAttributes object with the grayscale ColorMatrix
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);

            // Create a new Bitmap object to hold the grayscale image
            Bitmap grayscaleImage = new Bitmap(image.Width, image.Height);

            // Create a Graphics object to draw the grayscale image
            Graphics graphics = Graphics.FromImage(grayscaleImage);

            // Draw the grayscale image using the ImageAttributes object
            graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);

            // Dispose of the Graphics object
            graphics.Dispose();

            // Set the grayscale image as the PictureBox image
            pictureBox.Image = grayscaleImage;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            convertPictureBoxToGrayscale(pictureBox2);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string folderName = "tmpfolder_export"; // the name of the folder you want to create
            string pathString = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName); // create the full path to the folder

            // Check if the folder exists and create it if it doesn't
            if (!Directory.Exists(pathString))
            {
                Directory.CreateDirectory(pathString);
                Console.WriteLine("Folder created at: " + pathString);
            }
            else
            {
                Console.WriteLine("Folder already exists at: " + pathString);
            }

            // open the folder using the default system file explorer
            Process.Start(pathString);



            // get the current date and time
            DateTime now = DateTime.Now;

            // create a unique name based on the current date and time
            string uniqueName = $"{now.Year}-{now.Month}-{now.Day}_{now.Hour}-{now.Minute}-{now.Second}-{now.Millisecond}";

            Image image = pictureBox2.Image;

            // Save the image to the selected file in the selected format
            image.Save(pathString+@"\"+uniqueName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg); // or use ImageFormat.Bmp or ImageFormat.Png

        }
        private string fn_uniqname()
        {
            DateTime now = DateTime.Now;
            string uniqueName = $"{now.Year}-{now.Month}-{now.Day}_{now.Hour}-{now.Minute}-{now.Second}-{now.Millisecond}";
            return (uniqueName);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            try
            {
                string folderName = "tmpfolder_export"; // the name of the folder you want to create
                string pathString = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName); // create the full path to the folder

                var dialog = new WIA.CommonDialog();
                var file = dialog.ShowAcquireImage(WIA.WiaDeviceType.ScannerDeviceType);

                var str_tmp = pathString + @"\" + fn_uniqname() + "." + file.FileExtension;
                file.SaveFile(str_tmp);

                Image image = Image.FromFile(str_tmp);
                // Set the Image property of the PictureBox to the loaded image
                pictureBox1.Image = image;
                pictureBox2.Image = image;
            }
            catch (Exception)
            {

                textBoxLog.Text+="Error from scanner \r\n";
            }
            this.Enabled = true;


        }


        private void SetWIAProperty(WIA.Properties properties, string v1, int v2)
        {
            throw new NotImplementedException();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        
        private void timer_auto_Tick(object sender, EventArgs e)
        {

            if (chk_auto.Checked == true)
            {
                timer_auto.Enabled = false;
                fn_check_auto_scan();
                timer_auto.Enabled = true;
            }
            else label2.Text = "Not Active";
                        
        }
        private void fn_check_auto_scan()
        {
            string strPath= @"C:\scaner_q";
            if (!Directory.Exists(strPath)) {
                Directory.CreateDirectory(strPath);
            }
            label2.Text = DateTime.Now.ToString();
            string[] files = Directory.GetFiles(strPath,"*.txt");
            textBox1.Clear();
            foreach (var item in files)
            {
                textBox1.Text+= item + System.Environment.NewLine;
            }


            timer_auto.Enabled = false;
            foreach (var item in files)
            {
                
                //string pathString = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath); // create the full path to the folder

                fn_fast_scan(strPath + @"\" + Path.GetFileNameWithoutExtension(item) , "jpg");
                File.Delete(item);

            }
            timer_auto.Enabled = true;


        }
        private void CompressImage(Image sourceImage, int imageQuality, string savePath)
        {

            try
            {
                ImageCodecInfo jpegCodec = null;
                EncoderParameter imageQualitysParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, imageQuality);
                ImageCodecInfo[] allCodecs = ImageCodecInfo.GetImageEncoders();
                EncoderParameters codecParameter = new EncoderParameters(1);
                codecParameter.Param[0] = imageQualitysParameter;
                for (int i = 0; i < allCodecs.Length; i++)
                {
                    if (allCodecs[i].MimeType == "image/jpeg")
                    {
                        jpegCodec = allCodecs[i];
                        break;
                    }
                }
                if (File.Exists(savePath)) { File.Delete(savePath); }


                sourceImage.Save(savePath, jpegCodec, codecParameter);
                sourceImage.Dispose();
                //sourceImage = null;

            }
            catch(Exception e)
            {
                throw e;
                // Release the semaphore
            }
        }
        private void fn_fast_scan(string fname,string extension) {
            
                //string fname2 = fname + "." + extension;
                string tmp_fname = @"C:\scaner_q\TMP.JPG";
                //string pp=System.IO.Path.GetDirectoryName(fname2);

                this.Enabled = false;

                pictureBox1.InitialImage = null;
                pictureBox2.InitialImage = null;






                if (File.Exists(tmp_fname)) { File.Delete(tmp_fname); }

                var deviceManager = new DeviceManager();
                DeviceInfo scannerDeviceInfo = null;

                // پیدا کردن دستگاه اسکنر
                foreach (DeviceInfo deviceInfo in deviceManager.DeviceInfos)
                {
                    if (deviceInfo.Type == WiaDeviceType.ScannerDeviceType)
                    {
                        scannerDeviceInfo = deviceInfo;
                        break;
                    }
                }

                if (scannerDeviceInfo != null)
                {
                    Device scannerDevice = scannerDeviceInfo.Connect();

                    if (scannerDevice != null)
                    {
                        Item scannerItem = scannerDevice.Items[1];

                        SetDefaultScannerSettings(scannerItem);

                        ImageFile image = (ImageFile)scannerItem.Transfer(FormatID.wiaFormatJPEG);


                        // ذخیره تصویر اسکن شده
                        if (File.Exists(tmp_fname)) { File.Delete(tmp_fname); }

                        image.SaveFile(tmp_fname);

                        //CompressImage(Image.FromFile(fname), 50, @"f:\aaa.jpg");
                        Image img = Image.FromFile(tmp_fname);
                        CompressImage(img, 50, fname + "." + extension);
                        if (File.Exists(tmp_fname)) { File.Delete(tmp_fname); }


                        //if (pp != @"C:\scaner_q")
                        //{
                        //    Image img = Image.FromFile(fname);
                        //    pictureBox1.Image = img;
                        //    pictureBox2.Image = pictureBox1.Image;
                        //}
                    }
                    else
                    {
                        //MessageBox.Show("دستگاه اسکنر موجود نیست.");
                    textBoxLog.Text += "دستگاه اسکنر موجود نیست. \r\n";
                        

                    }
                }
                else
                {
                    //MessageBox.Show("هیچ دستگاه اسکنر موجود نیست.");
                    textBoxLog.Text += "هیچ دستگاه اسکنر موجود نیست. \r\n";
                }

                this.Enabled = true;
            
        }

      

        // تابع کمکی برای دریافت کدک مربوط به یک نوع تصویر
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo encoder in encoders)
            {
                if (encoder.MimeType == mimeType)
                {
                    return encoder;
                }
            }
            return null;
        }

        private static void SetDefaultScannerSettings(Item item)
        {
            // مثال: تنظیم رزولوشن به 300 دی‌پی‌آی (DPI)
            Property resolutionProperty = item.Properties["6147"]; // 6147 به معنای ویژگی رزولوشن در WIA است
            if (resolutionProperty != null)
            {
                resolutionProperty.set_Value(300);
            }

            // مثال: تنظیم حالت رنگی به رنگی
            Property colorProperty = item.Properties["6146"]; // 6146 به معنای ویژگی حالت رنگی در WIA است
            if (colorProperty != null)
            {
                colorProperty.set_Value(WiaImageIntent.ColorIntent);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

            //fn_fast_scan(@"tmpfolder_export\scan_image","jpg");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            pictureBox2.Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
            pictureBox2.Invalidate();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
                frm.ShowDialog();
        }
    }
}
