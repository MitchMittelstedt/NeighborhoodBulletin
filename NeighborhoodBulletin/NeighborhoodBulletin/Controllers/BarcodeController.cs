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


namespace BarcodeController
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }

        public static void ShowMessage(string message, bool error = false)
        {
            var origColor = Console.ForegroundColor;

            if (error)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(message);
            Console.ForegroundColor = origColor;
        }

        public static bool SetLicense()
        {
            if (RasterSupport.KernelExpired)
            {
                try
                {
                    // Replace these with the path to the LEADTOOLS license file
                    string licenseFilePath = @"C:\LEADTOOLS 20\Support\Common\License\LEADTOOLS.LIC";
                    string developerKey = System.IO.File.ReadAllText(@"C:\LEADTOOLS 20\Support\Common\License\LEADTOOLS.LIC.key");

                    if (licenseFilePath == null)
                    {
                        string commonLicenseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        commonLicenseDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(commonLicenseDir, "../../../../../../../Support/Common/License"));
                        if (System.IO.Directory.Exists(commonLicenseDir))
                        {
                            string commonLicenseFilePath = System.IO.Path.Combine(commonLicenseDir, "LEADTOOLS.lic");
                            string commonDeveloperKeyFilePath = System.IO.Path.Combine(commonLicenseDir, "LEADTOOLS.lic.key");

                            bool commonLicenseFileFound = System.IO.File.Exists(commonLicenseFilePath);
                            bool commonDeveloperKeyFileFound = System.IO.File.Exists(commonDeveloperKeyFilePath);
                            if (commonLicenseFileFound && commonDeveloperKeyFileFound)
                            {
                                licenseFilePath = commonLicenseFilePath;
                                developerKey = System.IO.File.ReadAllText(commonDeveloperKeyFilePath);
                            }
                            else
                            {
                                if (!commonLicenseFileFound)
                                    Console.WriteLine($"License file not found:{Environment.NewLine}{commonLicenseFilePath}");
                                if (!commonDeveloperKeyFileFound)
                                    Console.WriteLine($"Developer key file not found:{Environment.NewLine}{commonDeveloperKeyFilePath}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Common license directory does not exist:{Environment.NewLine}{commonLicenseDir}");
                        }
                    }

                    if (licenseFilePath != null && developerKey != null)
                        RasterSupport.SetLicense(licenseFilePath, developerKey);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (RasterSupport.KernelExpired)
            {
                string msg = "Your license file is missing, invalid or expired. LEADTOOLS will not function. Please contact LEAD Sales for information on obtaining a valid license.";
                ShowMessage($"*******************************************************************************{Environment.NewLine}", true);
                ShowMessage($"*** NOTE: {msg} ***{Environment.NewLine}", true);
                ShowMessage($"*******************************************************************************{Environment.NewLine}", true);
                return false;
            }

            return true;
        }
    }

    [DataContract]
    public class BarcodeResults
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }
        [DataMember(Name = "bounds")]
        public LeadRect Bounds { get; set; }
        [DataMember(Name = "symbology")]
        public string Symbology { get; set; }
    }

    public class BarcodeController : Controller
    {
        public BarcodeController()
        {
            // Set the license
            Program.SetLicense();
            if (RasterSupport.KernelExpired)
            {
                Console.WriteLine($"Please Set Your Runtime License...{Environment.NewLine}Exiting Demo...");
                return;
            }
        }

        /// <summary>
        /// Loads an image and recognized the barcodes on the image.
        /// </summary>
        /// <param name="uri">The source URI. http, ftp and file protocols are supported.</param>
        /// <returns>A Json containing the recognized barcodes and their properties.</returns>
        /// Example:
        /// /Barcode/RecognizeBarcodes?uri=https://demo.leadtools.com/images/tiff/barcode1.tif
        [HttpGet]
        public async Task<BarcodeResults[]> RecognizeBarcodes(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            BarcodeResults[] barcodeResults;
            var barcodeEngine = new BarcodeEngine();

            // Create RasterCodecs
            using (var codecs = new RasterCodecs())
            {
                codecs.Options.RasterizeDocument.Load.Resolution = 300;

                // Load the raster image
                using (ILeadStream leadStream = await LeadStream.Factory.FromUri(uri))
                using (RasterImage image = await codecs.LoadAsync(leadStream))
                {
                    var barcodeReader = barcodeEngine.Reader;

                    // Get the symbologies to read
                    var symbologies = new List<BarcodeSymbology>();
                    symbologies.AddRange(barcodeReader.GetAvailableSymbologies());

                    InitBarcodeReader(barcodeReader, false);

                    // Read Barcodes
                    BarcodeData[] barcodes = barcodeEngine.Reader.ReadBarcodes(image, LeadRect.Empty, 0, symbologies.ToArray());

                    if (barcodes.Length == 0)
                    {
                        // Did not find any barcodes. Try again with DoublePass enabled
                        InitBarcodeReader(barcodeReader, true);

                        // Do not read MicroPDF417 in this pass since it is too slow
                        if (symbologies != null && symbologies.Contains(BarcodeSymbology.MicroPDF417))
                            symbologies.Remove(BarcodeSymbology.MicroPDF417);

                        barcodes = barcodeEngine.Reader.ReadBarcodes(image, LeadRect.Empty, 0, symbologies.ToArray());
                    }

                    barcodeResults = new BarcodeResults[barcodes.Length];

                    // Add the barcodes found to our results
                    for (int i = 0; i < barcodes.Length; i++)
                    {
                        BarcodeData barcode = barcodes[i];

                        barcodeResults[i] = new BarcodeResults
                        {
                            Value = barcodes[i].Value,
                            Bounds = barcodes[i].Bounds,
                            Symbology = barcodes[i].Symbology.ToString()
                        };
                    }
                }
            }

            return barcodeResults;
        }

        private static void InitBarcodeReader(BarcodeReader reader, bool doublePass)
        {
            // Default options to read most barcodes
            reader.ImageType = BarcodeImageType.Unknown;

            // Both directions for 1D
            OneDBarcodeReadOptions oneDOptions = reader.GetDefaultOptions(BarcodeSymbology.UPCA) as OneDBarcodeReadOptions;
            oneDOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

            GS1DatabarStackedBarcodeReadOptions gs1Options = reader.GetDefaultOptions(BarcodeSymbology.GS1DatabarStacked) as GS1DatabarStackedBarcodeReadOptions;
            gs1Options.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

            FourStateBarcodeReadOptions fourStateOptions = reader.GetDefaultOptions(BarcodeSymbology.USPS4State) as FourStateBarcodeReadOptions;
            fourStateOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

            PatchCodeBarcodeReadOptions patchCodeOptions = reader.GetDefaultOptions(BarcodeSymbology.PatchCode) as PatchCodeBarcodeReadOptions;
            patchCodeOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

            PostNetPlanetBarcodeReadOptions postNetOptions = reader.GetDefaultOptions(BarcodeSymbology.PostNet) as PostNetPlanetBarcodeReadOptions;
            postNetOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

            PharmaCodeBarcodeReadOptions pharmaCodeOptions = reader.GetDefaultOptions(BarcodeSymbology.PharmaCode) as PharmaCodeBarcodeReadOptions;
            pharmaCodeOptions.SearchDirection = BarcodeSearchDirection.HorizontalAndVertical;

            // Double pass
            oneDOptions.EnableDoublePass = doublePass;

            DatamatrixBarcodeReadOptions dataMatrixOptions = reader.GetDefaultOptions(BarcodeSymbology.Datamatrix) as DatamatrixBarcodeReadOptions;
            dataMatrixOptions.EnableDoublePass = doublePass;

            PDF417BarcodeReadOptions pdf417Options = reader.GetDefaultOptions(BarcodeSymbology.PDF417) as PDF417BarcodeReadOptions;
            pdf417Options.EnableDoublePass = doublePass;

            MicroPDF417BarcodeReadOptions microPdf4127Options = reader.GetDefaultOptions(BarcodeSymbology.MicroPDF417) as MicroPDF417BarcodeReadOptions;
            microPdf4127Options.EnableDoublePass = doublePass;

            QRBarcodeReadOptions qrOptions = reader.GetDefaultOptions(BarcodeSymbology.QR) as QRBarcodeReadOptions;
            qrOptions.EnableDoublePass = doublePass;

            reader.ImageType = BarcodeImageType.Unknown;
        }
    }
}
