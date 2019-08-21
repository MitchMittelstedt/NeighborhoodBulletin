// *************************************************************
// Copyright (c) 1991-2019 LEAD Technologies, Inc.              
// All Rights Reserved.                                         
// *************************************************************
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Codecs;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using NeighborhoodBulletin;
using System.Text;

namespace NeighborhoodBulletin.Controllers
{
    public class BarcodesController
    {
        private BarcodeEngine barcodeEngineInstance; // The barcode engine 
        private RasterImage theImage; // The current loaded image 
        private string imageFileName; // Last file name we loaded; this is used in the "Writing Barcodes" tutorial 

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
            }
            catch (Exception ex)
            {
            }
        }

    }


}
