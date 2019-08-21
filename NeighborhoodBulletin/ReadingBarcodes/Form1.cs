using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leadtools.Codecs;
using Leadtools.Forms.Common;
using Leadtools;
using Leadtools.Barcode;
using System.Windows.Forms;


namespace ReadingBarcodes
{
    public partial class Form1 : Form
    {
        private BarcodeEngine barcodeEngineInstance; // The barcode engine 
        private RasterImage theImage; // The current loaded image 
        private string imageFileName; // Last file name we loaded; this is used in the "Writing Barcodes" tutorial 

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            // Requires a license file that unlocks 1D barcode read functionality. 
            string MY_LICENSE_FILE = @"C:\LEADTOOLS 20\Common\License\leadtools.lic";
            string MY_DEVELOPER_KEY = System.IO.File.ReadAllText(@"C:\LEADTOOLS 20\Common\License\leadtools.lic.key");
            RasterSupport.SetLicense(MY_LICENSE_FILE, MY_DEVELOPER_KEY);

            // Create the BarcodeEngine instance 
            barcodeEngineInstance = new BarcodeEngine();

            readBarcodesButton.Click += readBarcodesButton_Click;
            loadImageButton.Click += loadImageButton_Click;

            base.OnLoad(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Dispose of the image 
            if (theImage != null)
            {
                theImage.Dispose();
            }

            base.OnFormClosed(e);
        }

        private void loadImageButton_Click(object sender, EventArgs e)
        {
            string fileName = @"C:\Users\Public\Documents\LEADTOOLS Images\Barcode1.tif";
            // Or uncomment the following to load your own file 
            //using(OpenFileDialog dlg = new OpenFileDialog()) 
            //{ 
            //   if(dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK) 
            //   { 
            //      fileName = dlg.FileName; 
            //   } 
            //   else 
            //   { 
            //      return; 
            //   } 
            //} 

            // Load the image 
            using (RasterCodecs codecs = new RasterCodecs())
            {
                RasterImage newImage = codecs.Load(fileName, 0, CodecsLoadByteOrder.BgrOrGray, 1, 1);

                // Dispose of the image 
                if (theImage != null)
                {
                    theImage.Dispose();
                }

                theImage = newImage;
                imageFileName = fileName;
            }
            MessageBox.Show("Image loaded");
        }

        private void readBarcodesButton_Click(object sender, EventArgs e)
        {
            if (theImage == null)
            {
                loadImageButton_Click(sender, e);
            }

            try
            {
                // Read all the barcodes 
                // The first parameter is the image from which to read the barcodes. 
                // The second parameter is the search rectangle. Pass an empty rectangle to search the entire image. 
                // The third parameter is the maximum number of barcodes to read. Pass 0 for all found in the image. 
                // The last parameter is an array of the BarcodeSymbology that we are interested in finding. Pass null (or Nothing) 
                // to find all available barcodes found in the image and supported by the current license. 
                BarcodeData[] dataArray = barcodeEngineInstance.Reader.ReadBarcodes(theImage, LeadRect.Empty, 0, null);

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} barcode(s) found", dataArray.Length);
                sb.AppendLine();

                for (int i = 0; i < dataArray.Length; i++)
                {
                    BarcodeData data = dataArray[i];

                    sb.AppendFormat("Symbology: {0}, Location: {1}, Data: {2}", data.Symbology.ToString(), data.Bounds.ToString(), data.Value);
                    sb.AppendLine();
                }

                MessageBox.Show(sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
