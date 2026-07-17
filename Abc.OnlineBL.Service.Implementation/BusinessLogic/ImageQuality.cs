using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Abc.OnlineBL.Service.Implementation.BusinessLogic
{
    public class ImageQuality
    {
        private decimal resolution;
        private decimal width;
        private decimal height;
        private decimal minimumMegaPixels;
        private decimal recommendedMegaPixels;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ImageQuality"/> class.
        /// </summary>
        public ImageQuality() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ImageQuality"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="resolution">The resolution.</param>
        public ImageQuality(decimal width, decimal height, decimal resolution)
        {
            this.width = width;
            this.height = height;
            this.resolution = resolution;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageQuality"/> class.
        /// </summary>
        /// <param name="minimumMegaPixels">The minimum mega pixels.</param>
        /// <param name="recommendedMegaPixels">The recommended mega pixels.</param>
        public ImageQuality(decimal minimumMegaPixels, decimal recommendedMegaPixels)
        {
            this.minimumMegaPixels = minimumMegaPixels;
            this.recommendedMegaPixels = recommendedMegaPixels;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>        
        public decimal Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>        
        public decimal Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        /// <value>The resolution.</value>        
        public decimal Resolution
        {
            get { return resolution; }
            set { resolution = value; }
        }

        /// <summary>
        /// Any image that are greater or equals to the recommended mega pixels are good quality images.
        /// </summary>
        /// <value>The recommended mega pixels.</value>
        public decimal RecommendedMegaPixels
        {
            get { return recommendedMegaPixels; }
            set { recommendedMegaPixels = value; }
        }


        /// <summary>
        /// Any image that are greater or equals to the minimum mega pixels and less than recommended
        /// mega pixels are OK images, but should be issued a warning to the user. Any image less this value
        /// is not acceptable
        /// </summary>
        /// <value>The minimum mega pixles.</value>
        public decimal MinimumMegaPixels
        {
            get { return minimumMegaPixels; }
            set { minimumMegaPixels = value; }
        }

        /// <summary>
        /// Gets the image quality in mega pixel.
        /// </summary>
        /// <returns>Width*Height value</returns>
        public decimal GetInMegaPixel()
        {
            return ((width * height) / 1000000);
        }

        #region GetImageQuality
        /// <summary>
        /// Gets the image quality.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <returns></returns>
        public static ImageQuality GetImageQuality(string inputFile)
        {
            ImageMagickObject.MagickImage img = new ImageMagickObject.MagickImage();
            string exceptionString = string.Format("Error in: Identify -format '%w|%h|%x|%y' {0}", inputFile);

            object[] prams = { "-format", "'%w|%h|%x|%y'", inputFile };
            string ret = img.Identify(ref prams) as string;

            if (string.IsNullOrEmpty(ret))
            {
                throw new Exception(exceptionString);
            }

            ret = ret.Replace("'", "");

            string[] parts = ret.Split('|');
            if (parts.Length == 4)
            {
                // Return string format
                // 1024|768|71 Pixels Per Inch|71 Pixels Per Inch
                string sWidth = parts[0];
                string sHeight = parts[1];
                string sXRes = parts[2].Split(' ')[0];
                string sYRes = parts[3].Split(' ')[0];

                if (!string.IsNullOrEmpty(sWidth) && !string.IsNullOrEmpty(sHeight) &&
                        !string.IsNullOrEmpty(sXRes) && !string.IsNullOrEmpty(sYRes))
                {
					try
					{
						int width = Convert.ToInt32(sWidth);
						int height = Convert.ToInt32(sHeight);
						float xRes = 0; float.TryParse(sXRes, out xRes);
						float yRes = 0; float.TryParse(sYRes, out yRes);

						ImageQuality imgQ = new ImageQuality(width, height,Convert.ToDecimal((xRes + yRes) / 2));
						return imgQ;
					}
					catch (Exception ex)
					{
						Logger.Exception(ex, "Image Magic Output: {0}", ret);
						throw;
					}                    
                }
                else
                {
                    throw new Exception(exceptionString);
                }
            }
            else
            {
                throw new Exception(exceptionString);
            }
        }
        #endregion

        public static bool CreateThumbnail(string inFile)
        {
            if (File.Exists(inFile))
            {
                ImageMagickObject.MagickImage img = new ImageMagickObject.MagickImage();                
                object[] prams = { inFile, "-thumbnail", "100x100", inFile };
                img.Convert(ref prams);
                return true;
            }
            return false;
        }
    }
}
