using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AssistScan
{
    internal class Class1
    {
        public void CompressImage(Image sourceImage, int imageQuality, string savePath)
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
            }
            catch (System.Exception ex)
            {
                System.Console.Write(ex.Message);
            }
        }
    }
}
