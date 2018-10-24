using System;
using System.Runtime.InteropServices;
using Dicom.IO.Buffer;


namespace Dicom.Imaging.Codec
{
    /// <summary>
    ///
    /// </summary>
    public class JpegCodecBase : IJpegNativeCodec
    {
        /// <summary>
        ///
        /// </summary>
        public JpegMode Mode { get; }

        /// <summary>
        ///
        /// </summary>
        public int PointTransform { get; }

        /// <summary>
        ///
        /// </summary>
        public int Predictor { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int Precision { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="predictor"></param>
        /// <param name="pointTransform"></param>
        public JpegCodecBase(JpegMode mode, int predictor, int pointTransform, int precision)
        {
            this.Mode = mode;
            this.PointTransform = pointTransform;
            this.Precision = precision;
            this.Predictor = predictor;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="oldPixelData"></param>
        /// <param name="newPixelData"></param>
        /// <param name="jpegParams"></param>
        /// <param name="frame"></param>
        public void Decode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams jpegParams, int frame)
        {
            using (var codec = this.CreateCodecInterface())
            {
                this.jpeg_decompress_construct(codec, oldPixelData.GetFrame(frame).Data);

                this.jpeg_decompress_read_header(codec);

                newPixelData.PhotometricInterpretation = oldPixelData.PhotometricInterpretation;
                if (jpegParams.ConvertColorspaceToRGB && (this.jpeg_decompress_get_out_color_space(codec) == JpegColorSpace.JCS_YCbCr || this.jpeg_decompress_get_out_color_space(codec) == JpegColorSpace.JCS_RGB))
                {
                    if (oldPixelData.PixelRepresentation == PixelRepresentation.Signed)
                    {
                        throw new DicomCodecException("JPEG codec unable to perform colorspace conversion on signed pixel data");
                        //  NOTREACHED
                    }

                    this.jpeg_decompress_set_out_color_space(codec, JpegColorSpace.JCS_RGB);
                    newPixelData.PhotometricInterpretation = PhotometricInterpretation.Rgb;
                    newPixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;
                }
                else
                {
                    this.jpeg_decompress_set_jpeg_color_space(codec, JpegColorSpace.JCS_UNKNOWN);
                    this.jpeg_decompress_set_out_color_space(codec, JpegColorSpace.JCS_UNKNOWN);
                }

                this.jpeg_decompress_decompress(codec);

                //  get decompressed image
                var image = this.jpeg_decompress_get_image(codec);

                if (newPixelData.PlanarConfiguration == PlanarConfiguration.Planar && 1 < newPixelData.SamplesPerPixel)
                {
                    if (oldPixelData.SamplesPerPixel != 3 || 8 < oldPixelData.BitsStored)
                    {
                        throw new DicomCodecException("Planar reconfiguration only implemented for SamplesPerPixel=3 && BitsStores <= 8");
                        //  NOTREACHED
                    }

                    image = NativePixelData.InterleavedToPlanar24(image);
                }

                IByteBuffer buffer;
                if (1 * 1024 * 1024 <= image.Length || 1 < oldPixelData.NumberOfFrames)
                {
                    buffer = new TempFileBuffer(image);
                }
                else
                {
                    buffer = new MemoryByteBuffer(image);
                }

                newPixelData.AddFrame(buffer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="image"></param>
        void jpeg_decompress_construct(ILibIjg codec, byte[] image)
        {
            if (!codec.jpeg_decompress_construct(image))
            {
                throw new DicomCodecException("jpeg_decompress_construct failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        void jpeg_decompress_read_header(ILibIjg codec)
        {
            if (codec.jpeg_decompress_read_header() == JpegReadHeader.JPEG_SUSPENDED)
            {
                throw new DicomCodecException("Unable to decompress JPEG: Suspended");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <returns></returns>
        JpegColorSpace jpeg_decompress_get_out_color_space(ILibIjg codec)
        {
            return (JpegColorSpace)codec.jpeg_decompress_get_out_color_space();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="value"></param>
        void jpeg_decompress_set_out_color_space(ILibIjg codec, JpegColorSpace value)
        {
            if (!codec.jpeg_decompress_set_out_color_space((int)value))
            {
                throw new DicomCodecException($"jpeg_decompress_set_out_color_space({value}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="value"></param>
        void jpeg_decompress_set_jpeg_color_space(ILibIjg codec, JpegColorSpace value)
        {
            if (!codec.jpeg_decompress_set_jpeg_color_space((int)value))
            {
                throw new DicomCodecException($"jpeg_decompress_set_jpeg_color_space({value}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        void jpeg_decompress_decompress(ILibIjg codec)
        {
            if (!codec.jpeg_decompress_decompress())
            {
                throw new DicomCodecException("JPEG codec unable to decompress");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <returns></returns>
        byte[] jpeg_decompress_get_image(ILibIjg codec)
        {
            //  round up length to even
            var length = (codec.jpeg_decompress_get_length() + 1) & ~1;
            var image = new byte[length];
            if (!codec.jpeg_decompress_get_image(image))
            {
                throw new DicomCodecException("jpeg_decompress_get_image failed");
                //  NOTREACHED
            }

            return image;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="oldPixelData"></param>
        /// <param name="newPixelData"></param>
        /// <param name="parameters"></param>
        /// <param name="frame"></param>
        public void Encode(DicomPixelData oldPixelData, DicomPixelData newPixelData, DicomJpegParams parameters, int frame)
        {
            if ((oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrIct) ||
                (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.YbrRct))
            {
                throw new DicomCodecException("Photometric Interpretation '{0}' not supported by JPEG encoder!", oldPixelData.PhotometricInterpretation);
                //  NOTREACHED
            }

            if (oldPixelData.PlanarConfiguration == PlanarConfiguration.Planar && 1 < oldPixelData.SamplesPerPixel)
            {
                if (oldPixelData.SamplesPerPixel != 3 || 8 < oldPixelData.BitsStored)
                {
                    throw new DicomCodecException("Planar reconfiguration only implemented for SamplesPerPixel=3 && BitsStores <= 8");
                    //  NOTREACHED
                }

                newPixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;
            }

            using (var codec = this.CreateCodecInterface())
            {
                this.jpeg_compress_construct(codec);

                this.CompressNative(codec, oldPixelData, parameters, frame);

                if (oldPixelData.PhotometricInterpretation == PhotometricInterpretation.Rgb && this.jpeg_compress_get_jpeg_color_space(codec) == JpegColorSpace.JCS_YCbCr)
                {
                    if (parameters.SampleFactor == DicomJpegSampleFactor.SF422)
                    {
                        newPixelData.PhotometricInterpretation = PhotometricInterpretation.YbrFull422;
                    }
                    else
                    {
                        newPixelData.PhotometricInterpretation = PhotometricInterpretation.YbrFull;
                    }
                }

                var image = this.jpeg_compress_get_image(codec);

                IByteBuffer buffer;
                if (1 * 1024 * 1024 <= image.Length || 1 < oldPixelData.NumberOfFrames)
                {
                    buffer = new TempFileBuffer(image);
                }
                else
                {
                    buffer = new MemoryByteBuffer(image);
                }

                newPixelData.AddFrame(buffer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="pixelData"></param>
        /// <param name="parameters"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        void CompressNative(ILibIjg codec, DicomPixelData pixelData, DicomJpegParams parameters, int frame)
        {
            this.jpeg_compress_set_image_height(codec, pixelData.Height);
            this.jpeg_compress_set_image_width(codec, pixelData.Width);
            this.jpeg_compress_set_input_components(codec, pixelData.SamplesPerPixel);
            this.jpeg_compress_set_in_color_space(codec, this.getJpegColorSpace(pixelData.PhotometricInterpretation));

            //  set quality, sample factor and compression mode.
            switch (this.Mode)
            {
                case JpegMode.Baseline:
                case JpegMode.Sequential:
                    this.jpeg_compress_set_quality(codec, parameters.Quality);
                    this.jpeg_compress_set_sample_factor(codec, parameters.SampleFactor);
                    break;

                case JpegMode.Lossless:
                    this.jpeg_compress_simple_lossless(codec, this.Predictor, this.PointTransform);
                    this.jpeg_compress_set_sample_factor(codec, parameters.SampleFactor);
                    break;

                case JpegMode.Progressive:
                    this.jpeg_compress_set_quality(codec, parameters.Quality);
                    this.jpeg_compress_simple_progressive(codec);
                    this.jpeg_compress_set_sample_factor(codec, parameters.SampleFactor);
                    break;

                case JpegMode.SpectralSelection:
                    this.jpeg_compress_set_quality(codec, parameters.Quality);
                    this.jpeg_compress_simple_spectral_selection(codec);
                    this.jpeg_compress_set_sample_factor(codec, parameters.SampleFactor);
                    break;

                default:
                    break;
            }

            //  set somoothing factor
            this.jpeg_compress_set_smoothing_factor(codec, parameters.SmoothingFactor);

            //  compress
            this.jpeg_compress_compress(codec, pixelData, frame);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        void jpeg_compress_construct(ILibIjg codec)
        {
            if (!codec.jpeg_compress_construct())
            {
                throw new DicomCodecException("jpeg_compress_construct failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="value"></param>
        void jpeg_compress_set_image_height(ILibIjg codec, int value)
        {
            if (!codec.jpeg_compress_set_image_height(value))
            {
                throw new DicomCodecException($"jpeg_compress_set_image_height({value}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="value"></param>
        void jpeg_compress_set_image_width(ILibIjg codec, int value)
        {
            if (!codec.jpeg_compress_set_image_width(value))
            {
                throw new DicomCodecException($"jpeg_compress_set_image_width({value}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="value"></param>
        void jpeg_compress_set_input_components(ILibIjg codec, int value)
        {
            if (!codec.jpeg_compress_set_input_components(value))
            {
                throw new DicomCodecException($"jpeg_compress_set_input_components({value}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="value"></param>
        void jpeg_compress_set_in_color_space(ILibIjg codec, JpegColorSpace value)
        {
            if (!codec.jpeg_compress_set_in_color_space((int)value))
            {
                throw new DicomCodecException($"jpeg_compress_set_in_color_space({value}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="value"></param>
        void jpeg_compress_set_quality(ILibIjg codec, int value)
        {
            if (!codec.jpeg_compress_set_quality(value))
            {
                throw new DicomCodecException($"jpeg_compress_set_quality({value}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="value"></param>
        void jpeg_compress_set_sample_factor(ILibIjg codec, DicomJpegSampleFactor value)
        {
            if (!codec.jpeg_compress_set_sample_factor((int)value))
            {
                throw new DicomCodecException($"jpeg_compress_set_sample_factor({value}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="predictor"></param>
        /// <param name="pointTransform"></param>
        void jpeg_compress_simple_lossless(ILibIjg codec, int predictor, int pointTransform)
        {
            if (!codec.jpeg_compress_simple_lossless(predictor, pointTransform))
            {
                throw new DicomCodecException($"jpeg_compress_simple_lossless({predictor},{pointTransform}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        void jpeg_compress_simple_progressive(ILibIjg codec)
        {
            if (!codec.jpeg_compress_simple_progressive())
            {
                throw new DicomCodecException("jpeg_compress_simple_progressive failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        void jpeg_compress_simple_spectral_selection(ILibIjg codec)
        {
            if (!codec.jpeg_compress_simple_spectral_selection())
            {
                throw new DicomCodecException("jpeg_compress_simple_spectral_selection failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="value"></param>
        void jpeg_compress_set_smoothing_factor(ILibIjg codec, int value)
        {
            if (!codec.jpeg_compress_set_smoothing_factor(value))
            {
                throw new DicomCodecException($"jpeg_compress_set_smoothing_factor({value}) failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="pixelData"></param>
        /// <param name="frame"></param>
        void jpeg_compress_compress(ILibIjg codec, DicomPixelData pixelData, int frame)
        {
            byte[] image = pixelData.GetFrame(frame).Data;

            if (pixelData.BitsAllocated == 16 && pixelData.BitsStored <= 8)
            {
                image = NativePixelData.UnpackLow16(image);
            }

            if (pixelData.PlanarConfiguration == PlanarConfiguration.Planar && 1 < pixelData.SamplesPerPixel)
            {
                image = NativePixelData.PlanarToInterleaved24(image);
            }

            //  bytes per sample
            var bpp = (pixelData.BitsStored <= 8) ? 1 : pixelData.BytesAllocated;

            //  stride(bytes in row) = pixels per row * samples per pixel * bytes per sample.
            var stride = pixelData.Width * pixelData.SamplesPerPixel * bpp;
            if (!codec.jpeg_compress_compress(image, stride))
            {
                throw new DicomCodecException("jpeg_compress_compress failed");
                //  NOTREACHED
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <returns></returns>
        byte[] jpeg_compress_get_image(ILibIjg codec)
        {
            //  round up to even length.
            var size = (codec.jpeg_compress_get_length() + 1) & ~1;
            var image = new byte[size];
            if (!codec.jpeg_compress_get_image(image))
            {
                throw new DicomCodecException("jpeg_compress_get_image failed");
                //  NOTREACHED
            }

            return image;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <returns></returns>
        JpegColorSpace jpeg_compress_get_jpeg_color_space(ILibIjg codec)
        {
            return (JpegColorSpace)codec.jpeg_compress_get_jpeg_color_space();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photometricInterpretation"></param>
        /// <returns></returns>
        JpegColorSpace getJpegColorSpace(PhotometricInterpretation photometricInterpretation)
        {
            return
                photometricInterpretation == PhotometricInterpretation.Rgb ? JpegColorSpace.JCS_RGB :
                photometricInterpretation == PhotometricInterpretation.Monochrome1 ? JpegColorSpace.JCS_GRAYSCALE :
                photometricInterpretation == PhotometricInterpretation.Monochrome2 ? JpegColorSpace.JCS_GRAYSCALE :
                photometricInterpretation == PhotometricInterpretation.YbrFull ? JpegColorSpace.JCS_YCbCr :
                photometricInterpretation == PhotometricInterpretation.YbrFull422 ? JpegColorSpace.JCS_YCbCr :
                photometricInterpretation == PhotometricInterpretation.YbrPartial422 ? JpegColorSpace.JCS_YCbCr :
                photometricInterpretation == PhotometricInterpretation.PaletteColor ? JpegColorSpace.JCS_UNKNOWN :
                JpegColorSpace.JCS_UNKNOWN;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="pixelData"></param>
        /// <returns></returns>
        public int ScanHeaderForPrecision(DicomPixelData pixelData)
        {
            using (var codec = this.CreateCodecInterface())
            {
                this.jpeg_decompress_construct(codec, pixelData.GetFrame(0).Data);

                this.jpeg_decompress_read_header(codec);

                return this.jpeg_decompress_get_data_precision(codec);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec"></param>
        /// <returns></returns>
        int jpeg_decompress_get_data_precision(ILibIjg codec)
        {
            return codec.jpeg_decompress_get_data_precision();
        }

        /// <summary>
        /// return libijg interface for current environment
        /// </summary>
        /// <returns></returns>
        ILibIjg CreateCodecInterface()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                switch (RuntimeInformation.ProcessArchitecture)
                {
                    case Architecture.X64:
                        switch (this.Precision)
                        {
                            case 8:
                                return new Jpeg8_Linux_X64();
                            case 12:
                                return new Jpeg12_Linux_X64();
                            case 16:
                                return new Jpeg16_Linux_X64();
                            default:
                                throw new NotSupportedException();
                                //  NOTREACHED
                        }

                    case Architecture.X86:
                        switch (this.Precision)
                        {
                            case 8:
                                return new Jpeg8_Linux_X86();
                            case 12:
                                return new Jpeg12_Linux_X86();
                            case 16:
                                return new Jpeg16_Linux_X86();
                            default:
                                throw new NotSupportedException();
                                //  NOTREACHED
                        }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                //FIXME
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //FIXME
            }

            throw new DicomCodecException("cannot determine platform ");
        }
    }
}
