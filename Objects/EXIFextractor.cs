using System;
using System.Collections;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;

namespace Penguin.Images.Objects
{
    /// <summary>
    /// EXIFextractor Class.
    /// Code found elsewhere. Not well commented. Improvements needed for documentation
    /// </summary>
    public class EXIFextractor : IEnumerable
    {
        #region Properties

        //
        internal int Count => properties.Count;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs an instance of this class with the provided data
        /// </summary>
        /// <param name="bmp">a bitmap to use as the source</param>
        /// <param name="sp"></param>
        public EXIFextractor(ref System.Drawing.Bitmap bmp, string sp)
        {
            properties = new Hashtable();
            this.sp = sp;
            this.bmp = bmp ?? throw new ArgumentNullException(nameof(bmp));
            myHash = new TranslationDictionary();
            BuildDB(bmp.PropertyItems);
        }

        /// <summary>
        /// Constructs an instance of this class with the provided data
        /// </summary>
        /// <param name="file">The file path to use as a source</param>
        /// <param name="sp"></param>
        public EXIFextractor(string file, string sp)
        {
            properties = new Hashtable();
            this.sp = sp;

            myHash = new TranslationDictionary();
            //
            BuildDB(GetExifProperties(file));
        }

        #endregion Constructors

        #region Indexers

        /// <summary>
        /// Get the individual property value by supplying property name
        /// These are the valid property names :
        ///
        /// "Exif IFD"
        /// "Gps IFD"
        /// "New Subfile Type"
        /// "Subfile Type"
        /// "Image Width"
        /// "Image Height"
        /// "Bits Per Sample"
        /// "Compression"
        /// "Photometric Interp"
        /// "Thresh Holding"
        /// "Cell Width"
        /// "Cell Height"
        /// "Fill Order"
        /// "Document Name"
        /// "Image Description"
        /// "Equip Make"
        /// "Equip Model"
        /// "Strip Offsets"
        /// "Orientation"
        /// "Samples PerPixel"
        /// "Rows Per Strip"
        /// "Strip Bytes Count"
        /// "Min Sample Value"
        /// "Max Sample Value"
        /// "X Resolution"
        /// "Y Resolution"
        /// "Planar Config"
        /// "Page Name"
        /// "X Position"
        /// "Y Position"
        /// "Free Offset"
        /// "Free Byte Counts"
        /// "Gray Response Unit"
        /// "Gray Response Curve"
        /// "T4 Option"
        /// "T6 Option"
        /// "Resolution Unit"
        /// "Page Number"
        /// "Transfer Funcition"
        /// "Software Used"
        /// "Date Time"
        /// "Artist"
        /// "Host Computer"
        /// "Predictor"
        /// "White Point"
        /// "Primary Chromaticities"
        /// "ColorMap"
        /// "Halftone Hints"
        /// "Tile Width"
        /// "Tile Length"
        /// "Tile Offset"
        /// "Tile ByteCounts"
        /// "InkSet"
        /// "Ink Names"
        /// "Number Of Inks"
        /// "Dot Range"
        /// "Target Printer"
        /// "Extra Samples"
        /// "Sample Format"
        /// "S Min Sample Value"
        /// "S Max Sample Value"
        /// "Transfer Range"
        /// "JPEG Proc"
        /// "JPEG InterFormat"
        /// "JPEG InterLength"
        /// "JPEG RestartInterval"
        /// "JPEG LosslessPredictors"
        /// "JPEG PointTransforms"
        /// "JPEG QTables"
        /// "JPEG DCTables"
        /// "JPEG ACTables"
        /// "YCbCr Coefficients"
        /// "YCbCr Subsampling"
        /// "YCbCr Positioning"
        /// "REF Black White"
        /// "ICC Profile"
        /// "Gamma"
        /// "ICC Profile Descriptor"
        /// "SRGB RenderingIntent"
        /// "Image Title"
        /// "Copyright"
        /// "Resolution X Unit"
        /// "Resolution Y Unit"
        /// "Resolution X LengthUnit"
        /// "Resolution Y LengthUnit"
        /// "Print Flags"
        /// "Print Flags Version"
        /// "Print Flags Crop"
        /// "Print Flags Bleed Width"
        /// "Print Flags Bleed Width Scale"
        /// "Halftone LPI"
        /// "Halftone LPIUnit"
        /// "Halftone Degree"
        /// "Halftone Shape"
        /// "Halftone Misc"
        /// "Halftone Screen"
        /// "JPEG Quality"
        /// "Grid Size"
        /// "Thumbnail Format"
        /// "Thumbnail Width"
        /// "Thumbnail Height"
        /// "Thumbnail ColorDepth"
        /// "Thumbnail Planes"
        /// "Thumbnail RawBytes"
        /// "Thumbnail Size"
        /// "Thumbnail CompressedSize"
        /// "Color Transfer Function"
        /// "Thumbnail Data"
        /// "Thumbnail ImageWidth"
        /// "Thumbnail ImageHeight"
        /// "Thumbnail BitsPerSample"
        /// "Thumbnail Compression"
        /// "Thumbnail PhotometricInterp"
        /// "Thumbnail ImageDescription"
        /// "Thumbnail EquipMake"
        /// "Thumbnail EquipModel"
        /// "Thumbnail StripOffsets"
        /// "Thumbnail Orientation"
        /// "Thumbnail SamplesPerPixel"
        /// "Thumbnail RowsPerStrip"
        /// "Thumbnail StripBytesCount"
        /// "Thumbnail ResolutionX"
        /// "Thumbnail ResolutionY"
        /// "Thumbnail PlanarConfig"
        /// "Thumbnail ResolutionUnit"
        /// "Thumbnail TransferFunction"
        /// "Thumbnail SoftwareUsed"
        /// "Thumbnail DateTime"
        /// "Thumbnail Artist"
        /// "Thumbnail WhitePoint"
        /// "Thumbnail PrimaryChromaticities"
        /// "Thumbnail YCbCrCoefficients"
        /// "Thumbnail YCbCrSubsampling"
        /// "Thumbnail YCbCrPositioning"
        /// "Thumbnail RefBlackWhite"
        /// "Thumbnail CopyRight"
        /// "Luminance Table"
        /// "Chrominance Table"
        /// "Frame Delay"
        /// "Loop Count"
        /// "Pixel Unit"
        /// "Pixel PerUnit X"
        /// "Pixel PerUnit Y"
        /// "Palette Histogram"
        /// "Exposure Time"
        /// "F-Number"
        /// "Exposure Prog"
        /// "Spectral Sense"
        /// "ISO Speed"
        /// "OECF"
        /// "Ver"
        /// "DTOrig"
        /// "DTDigitized"
        /// "CompConfig"
        /// "CompBPP"
        /// "Shutter Speed"
        /// "Aperture"
        /// "Brightness"
        /// "Exposure Bias"
        /// "MaxAperture"
        /// "SubjectDist"
        /// "Metering Mode"
        /// "LightSource"
        /// "Flash"
        /// "FocalLength"
        /// "Maker Note"
        /// "User Comment"
        /// "DTSubsec"
        /// "DTOrigSS"
        /// "DTDigSS"
        /// "FPXVer"
        /// "ColorSpace"
        /// "PixXDim"
        /// "PixYDim"
        /// "RelatedWav"
        /// "Interop"
        /// "FlashEnergy"
        /// "SpatialFR"
        /// "FocalXRes"
        /// "FocalYRes"
        /// "FocalResUnit"
        /// "Subject Loc"
        /// "Exposure Index"
        /// "Sensing Method"
        /// "FileSource"
        /// "SceneType"
        /// "CfaPattern"
        /// "Gps Ver"
        /// "Gps LatitudeRef"
        /// "Gps Latitude"
        /// "Gps LongitudeRef"
        /// "Gps Longitude"
        /// "Gps AltitudeRef"
        /// "Gps Altitude"
        /// "Gps GpsTime"
        /// "Gps GpsSatellites"
        /// "Gps GpsStatus"
        /// "Gps GpsMeasureMode"
        /// "Gps GpsDop"
        /// "Gps SpeedRef"
        /// "Gps Speed"
        /// "Gps TrackRef"
        /// "Gps Track"
        /// "Gps ImgDirRef"
        /// "Gps ImgDir"
        /// "Gps MapDatum"
        /// "Gps DestLatRef"
        /// "Gps DestLat"
        /// "Gps DestLongRef"
        /// "Gps DestLong"
        /// "Gps DestBearRef"
        /// "Gps DestBear"
        /// "Gps DestDistRef"
        /// "Gps DestDist"
        /// </summary>
        public object this[string index] => properties[index];

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Gets a list of EXIF properties for the image found at the provided path
        /// </summary>
        /// <param name="fileName">The file to check</param>
        /// <returns>An array of EXIF properties found on the image</returns>
        public static PropertyItem[] GetExifProperties(string fileName)
        {
            using FileStream stream = new(fileName, FileMode.Open, FileAccess.Read);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream,
                             /* useEmbeddedColorManagement = */ true,
                             /* validateImageData = */ false);
            return image.PropertyItems;
        }

        /// <summary>
        /// Returns an enumerator for the properties extracted
        /// </summary>
        /// <returns>an enumerator for the properties extracted</returns>
        public IEnumerator GetEnumerator()
        {
            // TODO:  Add EXIFextractor.GetEnumerator implementation
            return new EXIFextractorEnumerator(properties);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public void SetTag(int id, string data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Encoding ascii = Encoding.ASCII;
            SetTag(id, data.Length, 0x2, ascii.GetBytes(data));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="len"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void SetTag(int id, int len, short type, byte[] data)
        {
            PropertyItem p = CreatePropertyItem(type, id, len, data);
            bmp.SetPropertyItem(p);
            BuildDB(bmp.PropertyItems);
        }

        public override string ToString()
        {
            return data;
        }

        #endregion Methods

        #region Fields

        private readonly System.Drawing.Bitmap bmp;

        private readonly TranslationDictionary myHash;

        private readonly Hashtable properties;

        private readonly string sp;

        private string data;

        #endregion Fields

        /// <summary>
        ///
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private static uint ConvertToInt16U(byte[] arr)
        {
            return arr.Length != 2 ? 0 : (uint)Convert.ToUInt16((arr[1] << 8) | arr[0]);
        }

        private static int ConvertToInt32(byte[] arr)
        {
            return arr.Length != 4 ? 0 : (arr[3] << 24) | (arr[2] << 16) | (arr[1] << 8) | arr[0];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private static uint ConvertToInt32U(byte[] arr)
        {
            return arr.Length != 4 ? 0 : Convert.ToUInt32((arr[3] << 24) | (arr[2] << 16) | (arr[1] << 8) | arr[0]);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tag"></param>
        /// <param name="len"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static PropertyItem CreatePropertyItem(short type, int tag, int len, byte[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            PropertyItem item;

            // Loads a PropertyItem from a Jpeg image stored in the assembly as a resource.
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream emptyBitmapStream = assembly.GetManifestResourceStream("Framework.Content.Images.decoy.jpg");
            System.Drawing.Image empty = System.Drawing.Image.FromStream(emptyBitmapStream);

            item = empty.PropertyItems[0];

            // Copies the data to the property item.
            item.Type = type;
            item.Len = len;
            item.Id = tag;
            item.Value = new byte[value.Length];
            value.CopyTo(item.Value, 0);

            return item;
        }

        /// <summary>
        ///
        /// </summary>
        private void BuildDB(System.Drawing.Imaging.PropertyItem[] parr)
        {
            properties.Clear();
            //
            data = "";
            //
            Encoding ascii = Encoding.ASCII;
            //
            foreach (System.Drawing.Imaging.PropertyItem p in parr)
            {
                string v = "";
                string name = (string)myHash[p.Id];
                // tag not found. skip it
                if (name == null)
                {
                    continue;
                }
                //
                data += name + ": ";
                //
                //1 = BYTE An 8-bit unsigned integer.,
                if (p.Type == 0x1)
                {
                    v = p.Value[0].ToString();
                }
                //2 = ASCII An 8-bit byte containing one 7-bit ASCII code. The final byte is terminated with NULL.,
                else if (p.Type == 0x2)
                {
                    // string
                    v = ascii.GetString(p.Value);
                }
                //3 = SHORT A 16-bit (2 -byte) unsigned integer,
                else if (p.Type == 0x3)
                {
                    // orientation // lookup table
                    switch (p.Id)
                    {
                        case 0x8827: // ISO
                            v = "ISO-" + ConvertToInt16U(p.Value).ToString();
                            break;

                        case 0xA217: // sensing method
                            {
                                v = ConvertToInt16U(p.Value) switch
                                {
                                    1 => "Not defined",
                                    2 => "One-chip color area sensor",
                                    3 => "Two-chip color area sensor",
                                    4 => "Three-chip color area sensor",
                                    5 => "Color sequential area sensor",
                                    7 => "Trilinear sensor",
                                    8 => "Color sequential linear sensor",
                                    _ => " reserved",
                                };
                            }
                            break;

                        case 0x8822: // aperture
                            v = ConvertToInt16U(p.Value) switch
                            {
                                0 => "Not defined",
                                1 => "Manual",
                                2 => "Normal program",
                                3 => "Aperture priority",
                                4 => "Shutter priority",
                                5 => "Creative program (biased toward depth of field)",
                                6 => "Action program (biased toward fast shutter speed)",
                                7 => "Portrait mode (for closeup photos with the background out of focus)",
                                8 => "Landscape mode (for landscape photos with the background in focus)",
                                _ => "reserved",
                            };
                            break;

                        case 0x9207: // metering mode
                            v = ConvertToInt16U(p.Value) switch
                            {
                                0 => "unknown",
                                1 => "Average",
                                2 => "CenterWeightedAverage",
                                3 => "Spot",
                                4 => "MultiSpot",
                                5 => "Pattern",
                                6 => "Partial",
                                255 => "Other",
                                _ => "reserved",
                            };
                            break;

                        case 0x9208: // light source
                            {
                                v = ConvertToInt16U(p.Value) switch
                                {
                                    0 => "unknown",
                                    1 => "Daylight",
                                    2 => "Fluorescent",
                                    3 => "Tungsten",
                                    17 => "Standard light A",
                                    18 => "Standard light B",
                                    19 => "Standard light C",
                                    20 => "D55",
                                    21 => "D65",
                                    22 => "D75",
                                    255 => "other",
                                    _ => "reserved",
                                };
                            }
                            break;

                        case 0x9209:
                            {
                                v = ConvertToInt16U(p.Value) switch
                                {
                                    0 => "Flash did not fire",
                                    1 => "Flash fired",
                                    5 => "Strobe return light not detected",
                                    7 => "Strobe return light detected",
                                    _ => "reserved",
                                };
                            }
                            break;

                        default:
                            v = ConvertToInt16U(p.Value).ToString();
                            break;
                    }
                }
                //4 = LONG A 32-bit (4 -byte) unsigned integer,
                else if (p.Type == 0x4)
                {
                    // orientation // lookup table
                    v = ConvertToInt32U(p.Value).ToString();
                }
                //5 = RATIONAL Two LONGs. The first LONG is the numerator and the second LONG expresses the//denominator.,
                else if (p.Type == 0x5)
                {
                    // rational
                    byte[] n = new byte[p.Len / 2];
                    byte[] d = new byte[p.Len / 2];
                    Array.Copy(p.Value, 0, n, 0, p.Len / 2);
                    Array.Copy(p.Value, p.Len / 2, d, 0, p.Len / 2);
                    uint a = ConvertToInt32U(n);
                    uint b = ConvertToInt32U(d);
                    Rational r = new(a, b);
                    //
                    //convert here
                    //
                    v = p.Id switch
                    {
                        // aperture
                        0x9202 => "F/" + Math.Round(Math.Pow(Math.Sqrt(2), r.ToDouble()), 2).ToString(),
                        0x920A => r.ToDouble().ToString(),
                        0x829A => r.ToDouble().ToString(),
                        // F-number
                        0x829D => "F/" + r.ToDouble().ToString(),
                        _ => r.ToString("/"),
                    };
                }
                //7 = UNDEFINED An 8-bit byte that can take any value depending on the field definition,
                else if (p.Type == 0x7)
                {
                    switch (p.Id)
                    {
                        case 0xA300:
                            {
                                v = p.Value[0] == 3 ? "DSC" : "reserved";
                                break;
                            }
                        case 0xA301:
                            v = p.Value[0] == 1 ? "A directly photographed image" : "Not a directly photographed image";

                            break;

                        default:
                            v = "-";
                            break;
                    }
                }
                //9 = SLONG A 32-bit (4 -byte) signed integer (2's complement notation),
                else if (p.Type == 0x9)
                {
                    v = ConvertToInt32(p.Value).ToString();
                }
                //10 = SRATIONAL Two SLONGs. The first SLONG is the numerator and the second SLONG is the
                //denominator.
                else if (p.Type == 0xA)
                {
                    // rational
                    byte[] n = new byte[p.Len / 2];
                    byte[] d = new byte[p.Len / 2];
                    Array.Copy(p.Value, 0, n, 0, p.Len / 2);
                    Array.Copy(p.Value, p.Len / 2, d, 0, p.Len / 2);
                    int a = ConvertToInt32(n);
                    int b = ConvertToInt32(d);
                    Rational r = new(a, b);
                    //
                    // convert here
                    //
                    v = p.Id switch
                    {
                        // shutter speed
                        0x9201 => "1/" + Math.Round(Math.Pow(2, r.ToDouble()), 2).ToString(),
                        0x9203 => Math.Round(r.ToDouble(), 4).ToString(),
                        _ => r.ToString("/"),
                    };
                }
                // add it to the list
                if (properties[name] == null)
                {
                    properties.Add(name, v);
                }
                // cat it too
                data += v;
                data += sp;
            }
        }
    }

    /// <summary>
    /// Summary description for translation.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "<Pending>")]
    public class TranslationDictionary : Hashtable
    {
        #region Constructors

        /// <summary>
        ///
        /// </summary>
        public TranslationDictionary()
        {
            Add(0x8769, "Exif IFD");
            Add(0x8825, "Gps IFD");
            Add(0xFE, "New Subfile Type");
            Add(0xFF, "Subfile Type");
            Add(0x100, "Image Width");
            Add(0x101, "Image Height");
            Add(0x102, "Bits Per Sample");
            Add(0x103, "Compression");
            Add(0x106, "Photometric Interp");
            Add(0x107, "Thresh Holding");
            Add(0x108, "Cell Width");
            Add(0x109, "Cell Height");
            Add(0x10A, "Fill Order");
            Add(0x10D, "Document Name");
            Add(0x10E, "Image Description");
            Add(0x10F, "Equip Make");
            Add(0x110, "Equip Model");
            Add(0x111, "Strip Offsets");
            Add(0x112, "Orientation");
            Add(0x115, "Samples PerPixel");
            Add(0x116, "Rows Per Strip");
            Add(0x117, "Strip Bytes Count");
            Add(0x118, "Min Sample Value");
            Add(0x119, "Max Sample Value");
            Add(0x11A, "X Resolution");
            Add(0x11B, "Y Resolution");
            Add(0x11C, "Planar Config");
            Add(0x11D, "Page Name");
            Add(0x11E, "X Position");
            Add(0x11F, "Y Position");
            Add(0x120, "Free Offset");
            Add(0x121, "Free Byte Counts");
            Add(0x122, "Gray Response Unit");
            Add(0x123, "Gray Response Curve");
            Add(0x124, "T4 Option");
            Add(0x125, "T6 Option");
            Add(0x128, "Resolution Unit");
            Add(0x129, "Page Number");
            Add(0x12D, "Transfer Funcition");
            Add(0x131, "Software Used");
            Add(0x132, "Date Time");
            Add(0x13B, "Artist");
            Add(0x13C, "Host Computer");
            Add(0x13D, "Predictor");
            Add(0x13E, "White Point");
            Add(0x13F, "Primary Chromaticities");
            Add(0x140, "ColorMap");
            Add(0x141, "Halftone Hints");
            Add(0x142, "Tile Width");
            Add(0x143, "Tile Length");
            Add(0x144, "Tile Offset");
            Add(0x145, "Tile ByteCounts");
            Add(0x14C, "InkSet");
            Add(0x14D, "Ink Names");
            Add(0x14E, "Number Of Inks");
            Add(0x150, "Dot Range");
            Add(0x151, "Target Printer");
            Add(0x152, "Extra Samples");
            Add(0x153, "Sample Format");
            Add(0x154, "S Min Sample Value");
            Add(0x155, "S Max Sample Value");
            Add(0x156, "Transfer Range");
            Add(0x200, "JPEG Proc");
            Add(0x201, "JPEG InterFormat");
            Add(0x202, "JPEG InterLength");
            Add(0x203, "JPEG RestartInterval");
            Add(0x205, "JPEG LosslessPredictors");
            Add(0x206, "JPEG PointTransforms");
            Add(0x207, "JPEG QTables");
            Add(0x208, "JPEG DCTables");
            Add(0x209, "JPEG ACTables");
            Add(0x211, "YCbCr Coefficients");
            Add(0x212, "YCbCr Subsampling");
            Add(0x213, "YCbCr Positioning");
            Add(0x214, "REF Black White");
            Add(0x8773, "ICC Profile");
            Add(0x301, "Gamma");
            Add(0x302, "ICC Profile Descriptor");
            Add(0x303, "SRGB RenderingIntent");
            Add(0x320, "Image Title");
            Add(0x8298, "Copyright");
            Add(0x5001, "Resolution X Unit");
            Add(0x5002, "Resolution Y Unit");
            Add(0x5003, "Resolution X LengthUnit");
            Add(0x5004, "Resolution Y LengthUnit");
            Add(0x5005, "Print Flags");
            Add(0x5006, "Print Flags Version");
            Add(0x5007, "Print Flags Crop");
            Add(0x5008, "Print Flags Bleed Width");
            Add(0x5009, "Print Flags Bleed Width Scale");
            Add(0x500A, "Halftone LPI");
            Add(0x500B, "Halftone LPIUnit");
            Add(0x500C, "Halftone Degree");
            Add(0x500D, "Halftone Shape");
            Add(0x500E, "Halftone Misc");
            Add(0x500F, "Halftone Screen");
            Add(0x5010, "JPEG Quality");
            Add(0x5011, "Grid Size");
            Add(0x5012, "Thumbnail Format");
            Add(0x5013, "Thumbnail Width");
            Add(0x5014, "Thumbnail Height");
            Add(0x5015, "Thumbnail ColorDepth");
            Add(0x5016, "Thumbnail Planes");
            Add(0x5017, "Thumbnail RawBytes");
            Add(0x5018, "Thumbnail Size");
            Add(0x5019, "Thumbnail CompressedSize");
            Add(0x501A, "Color Transfer Function");
            Add(0x501B, "Thumbnail Data");
            Add(0x5020, "Thumbnail ImageWidth");
            Add(0x502, "Thumbnail ImageHeight");
            Add(0x5022, "Thumbnail BitsPerSample");
            Add(0x5023, "Thumbnail Compression");
            Add(0x5024, "Thumbnail PhotometricInterp");
            Add(0x5025, "Thumbnail ImageDescription");
            Add(0x5026, "Thumbnail EquipMake");
            Add(0x5027, "Thumbnail EquipModel");
            Add(0x5028, "Thumbnail StripOffsets");
            Add(0x5029, "Thumbnail Orientation");
            Add(0x502A, "Thumbnail SamplesPerPixel");
            Add(0x502B, "Thumbnail RowsPerStrip");
            Add(0x502C, "Thumbnail StripBytesCount");
            Add(0x502D, "Thumbnail ResolutionX");
            Add(0x502E, "Thumbnail ResolutionY");
            Add(0x502F, "Thumbnail PlanarConfig");
            Add(0x5030, "Thumbnail ResolutionUnit");
            Add(0x5031, "Thumbnail TransferFunction");
            Add(0x5032, "Thumbnail SoftwareUsed");
            Add(0x5033, "Thumbnail DateTime");
            Add(0x5034, "Thumbnail Artist");
            Add(0x5035, "Thumbnail WhitePoint");
            Add(0x5036, "Thumbnail PrimaryChromaticities");
            Add(0x5037, "Thumbnail YCbCrCoefficients");
            Add(0x5038, "Thumbnail YCbCrSubsampling");
            Add(0x5039, "Thumbnail YCbCrPositioning");
            Add(0x503A, "Thumbnail RefBlackWhite");
            Add(0x503B, "Thumbnail CopyRight");
            Add(0x5090, "Luminance Table");
            Add(0x5091, "Chrominance Table");
            Add(0x5100, "Frame Delay");
            Add(0x5101, "Loop Count");
            Add(0x5110, "Pixel Unit");
            Add(0x5111, "Pixel PerUnit X");
            Add(0x5112, "Pixel PerUnit Y");
            Add(0x5113, "Palette Histogram");
            Add(0x829A, "Exposure Time");
            Add(0x829D, "F-Number");
            Add(0x8822, "Exposure Prog");
            Add(0x8824, "Spectral Sense");
            Add(0x8827, "ISO Speed");
            Add(0x8828, "OECF");
            Add(0x9000, "Ver");
            Add(0x9003, "DTOrig");
            Add(0x9004, "DTDigitized");
            Add(0x9101, "CompConfig");
            Add(0x9102, "CompBPP");
            Add(0x9201, "Shutter Speed");
            Add(0x9202, "Aperture");
            Add(0x9203, "Brightness");
            Add(0x9204, "Exposure Bias");
            Add(0x9205, "MaxAperture");
            Add(0x9206, "SubjectDist");
            Add(0x9207, "Metering Mode");
            Add(0x9208, "LightSource");
            Add(0x9209, "Flash");
            Add(0x920A, "FocalLength");
            Add(0x927C, "Maker Note");
            Add(0x9286, "User Comment");
            Add(0x9290, "DTSubsec");
            Add(0x9291, "DTOrigSS");
            Add(0x9292, "DTDigSS");
            Add(0xA000, "FPXVer");
            Add(0xA001, "ColorSpace");
            Add(0xA002, "PixXDim");
            Add(0xA003, "PixYDim");
            Add(0xA004, "RelatedWav");
            Add(0xA005, "Interop");
            Add(0xA20B, "FlashEnergy");
            Add(0xA20C, "SpatialFR");
            Add(0xA20E, "FocalXRes");
            Add(0xA20F, "FocalYRes");
            Add(0xA210, "FocalResUnit");
            Add(0xA214, "Subject Loc");
            Add(0xA215, "Exposure Index");
            Add(0xA217, "Sensing Method");
            Add(0xA300, "FileSource");
            Add(0xA301, "SceneType");
            Add(0xA302, "CfaPattern");
            Add(0x0, "Gps Ver");
            Add(0x1, "Gps LatitudeRef");
            Add(0x2, "Gps Latitude");
            Add(0x3, "Gps LongitudeRef");
            Add(0x4, "Gps Longitude");
            Add(0x5, "Gps AltitudeRef");
            Add(0x6, "Gps Altitude");
            Add(0x7, "Gps GpsTime");
            Add(0x8, "Gps GpsSatellites");
            Add(0x9, "Gps GpsStatus");
            Add(0xA, "Gps GpsMeasureMode");
            Add(0xB, "Gps GpsDop");
            Add(0xC, "Gps SpeedRef");
            Add(0xD, "Gps Speed");
            Add(0xE, "Gps TrackRef");
            Add(0xF, "Gps Track");
            Add(0x10, "Gps ImgDirRef");
            Add(0x11, "Gps ImgDir");
            Add(0x12, "Gps MapDatum");
            Add(0x13, "Gps DestLatRef");
            Add(0x14, "Gps DestLat");
            Add(0x15, "Gps DestLongRef");
            Add(0x16, "Gps DestLong");
            Add(0x17, "Gps DestBearRef");
            Add(0x18, "Gps DestBear");
            Add(0x19, "Gps DestDistRef");
            Add(0x1A, "Gps DestDist");
        }

        #endregion Constructors
    }

    //
    // dont touch this class. its for IEnumerator
    //
    //
    internal class EXIFextractorEnumerator : IEnumerator
    {
        #region Properties

        public object Current => (index.Key, index.Value);

        #endregion Properties

        #region Methods

        public bool MoveNext()
        {
            return index != null && index.MoveNext();
        }

        public void Reset()
        {
            index = null;
        }

        #endregion Methods

        #region Constructors

        internal EXIFextractorEnumerator(Hashtable exif)
        {
            Reset();
            index = exif.GetEnumerator();
        }

        #endregion Constructors

        #region Fields

        private IDictionaryEnumerator index;

        #endregion Fields
    }

    /// <summary>
    /// private class
    /// </summary>
    internal class Rational
    {
        #region Constructors

        public Rational(int n, int d)
        {
            this.n = n;
            this.d = d;
            Simplify(ref this.n, ref this.d);
        }

        public Rational(uint n, uint d)
        {
            this.n = Convert.ToInt32(n);
            this.d = Convert.ToInt32(d);

            Simplify(ref this.n, ref this.d);
        }

        public Rational()
        {
            n = d = 0;
        }

        #endregion Constructors

        #region Methods

        public double ToDouble()
        {
            return d == 0 ? 0.0 : Math.Round(Convert.ToDouble(n) / Convert.ToDouble(d), 2);
        }

        public string ToString(string sp)
        {
            sp ??= "/";

            return $"{n}{sp}{d}";
        }

        #endregion Methods

        #region Fields

        private readonly int d;
        private readonly int n;

        #endregion Fields

        private int Euclid(int a, int b)
        {
            return b == 0 ? a : Euclid(b, a % b);
        }

        private void Simplify(ref int a, ref int b)
        {
            if (a == 0 || b == 0)
            {
                return;
            }

            int gcd = Euclid(a, b);
            a /= gcd;
            b /= gcd;
        }
    }
}