using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using WIA;

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
        private void button5_Click(object sender, EventArgs e)
        {
            printPictureBox(pictureBox1,"A4");
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
            string folderName = "tmpfolder_export"; // the name of the folder you want to create

            string pathString = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName); // create the full path to the folder

            var dialog = new WIA.CommonDialog();
            var file = dialog.ShowAcquireImage(WIA.WiaDeviceType.ScannerDeviceType);
            var str_tmp = pathString + @"\" + fn_uniqname() + "." + file.FileExtension;
            file.SaveFile(str_tmp);

            Image image = Image.FromFile(str_tmp);
            // Set the Image property of the PictureBox to the loaded image
            pictureBox1.Image = image;


            //// create a new instance of the WIA CommonDialog class
            //WIA.CommonDialog dialog = new WIA.CommonDialog();

            //// display the scanner selection dialog
            //Device device = dialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, true, false);

            //if (device != null)
            //{
            //    // create a new instance of the WIA Item class
            //    Item item = device.Items[1];

            //    // set the image format and resolution
            //    SetWIAProperty(item.Properties, "6146", 1);  // 1 = BMP format
            //    SetWIAProperty(item.Properties, "6147", 300);  // 300 DPI

            //    // scan the image and save it to a MemoryStream
            //    ImageFile image = (ImageFile)dialog.ShowTransfer(item, "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}", false);
            //    MemoryStream stream = new MemoryStream((byte[])image.FileData.get_BinaryData());

            //    // load the image from the MemoryStream into the picture box
            //    pictureBox1.Image = Image.FromStream(stream);

            //    // clean up the resources
            //    image = null;
            //    stream.Dispose();
            //}

        }

        private void SetWIAProperty(WIA.Properties properties, string v1, int v2)
        {
            throw new NotImplementedException();
        }
    }
}
