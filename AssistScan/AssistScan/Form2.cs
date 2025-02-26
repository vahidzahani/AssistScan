using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace AssistScan
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }

       
        
        private string getFileName(string path)
        {
            
            //return Path.GetFileNameWithoutExtension(path);
            return path;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in droppedFiles)
            {
                //string filename=getFileName(file);
                if (Class1.Checkfile(file)==true)
                    listBox1.Items.Add(file);
            }
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                string outpath = textOut.Text;
                if (!Directory.Exists(outpath))
                    Directory.CreateDirectory(outpath);

                Class1 cls = new Class1();
                progressBar1.Visible = true;
                progressBar1.Maximum = listBox1.Items.Count;
                progressBar1.Value = 0;
                int Quality = int.Parse(textQuality.Text);
                int processedCount = 0;

                foreach (var item in listBox1.Items)
                {
                    try
                    {
                        progressBar1.Value++;
                        string fname = Path.GetFileNameWithoutExtension(item.ToString());
                        string outfile = outpath + @"\" + fname + ".jpg";

                        Image img = Image.FromFile(item.ToString());

                        // اگر چک باکس CHKInvert فعال باشد، اینورت رنگ‌ها را انجام می‌دهیم
                        if (CHKInvert.Checked)
                        {
                            img = InvertImageColors(img);
                        }

                        cls.CompressImage(img, Quality, outfile);
                        processedCount++;
                    }
                    catch (OutOfMemoryException ex)
                    {
                        // Handle the out-of-memory exception
                        Console.WriteLine($"Out of memory encountered when processing item {processedCount}: {item.ToString()}");
                        Console.WriteLine($"Error message: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");

                        // Continue processing the remaining images
                        continue;
                    }
                }

                progressBar1.Visible = false;
                MessageBox.Show($"The conversion was successful. {processedCount} images processed.");
                System.Diagnostics.Process.Start("explorer.exe", outpath);
            }
            finally
            {
                // Perform cleanup tasks
                progressBar1.Visible = false;
            }
        }

        private Image InvertImageColors(Image original)
        {
            // تبدیل تصویر به Bitmap برای پردازش پیکسل‌ها
            Bitmap bmp = new Bitmap(original);

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    // دریافت رنگ پیکسل
                    Color pixelColor = bmp.GetPixel(x, y);

                    // محاسبه رنگ معکوس
                    Color invertedColor = Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B);

                    // اعمال رنگ معکوس به پیکسل
                    bmp.SetPixel(x, y, invertedColor);
                }
            }

            return bmp;
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textQuality.Text=trackBar1.Value.ToString();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                SelectedPath = Environment.CurrentDirectory
            };
            dialog.ShowDialog();
            string selectedFolder = dialog.SelectedPath;
            textOut.Text = selectedFolder;


        }

        private void textOut_TextChanged(object sender, EventArgs e)
        {

        }

        private void textOut_DoubleClick(object sender, EventArgs e)
        {

            System.Diagnostics.Process.Start("explorer.exe", textOut.Text);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
