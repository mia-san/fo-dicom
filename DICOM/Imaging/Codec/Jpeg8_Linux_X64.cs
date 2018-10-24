using System;
using System.Runtime.InteropServices;
using Dicom.Log;


namespace Dicom.Imaging.Codec
{
    /// <summary>
    /// 
    /// </summary>
    class Jpeg8_Linux_X64 : ILibIjg
    {
        /// <summary>
        /// 
        /// </summary>
        static readonly Logger logger = LogManager.GetLogger(typeof(Jpeg8_Linux_X64).Name);


        /// <summary>
        /// 
        /// </summary>
        public const string LibraryPath = "linux-x64/libijg8";


        /// <summary>
        /// logging delegate
        /// </summary>
        /// <param name="message"></param>
        delegate void LoggingCallback(string message);

        /// <summary>
        /// keep reference to delegate
        /// </summary>
        /// <param name="message"></param>
        static LoggingCallback _loggingCallback = message => logger.Debug("IJG: {0}", message);

        /// <summary>
        /// 
        /// </summary>
        static readonly IntPtr _loggingCallbackPtr = Marshal.GetFunctionPointerForDelegate(
            _loggingCallback
        );


        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public IntPtr Context { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static IntPtr jpeg_context_alloc(IntPtr logger);

        /// <summary>
        /// 
        /// </summary>
        public Jpeg8_Linux_X64()
        {
            this.Context = jpeg_context_alloc(_loggingCallbackPtr);
            if (this.Context == null)
            {
                throw new DicomCodecException("jpeg_context_alloc failed");
                //  NOTREACHED
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        [DllImport(LibraryPath)]
        extern static void jpeg_context_free(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            jpeg_context_free(this.Context);
            this.Context = IntPtr.Zero;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="image"></param>
        /// <param name="stride"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_compress(IntPtr context, Byte[] image, Int32 stride);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="stride"></param>
        /// <returns></returns>
        public bool jpeg_compress_compress(byte[] image, int stride)
        {
            return jpeg_compress_compress(this.Context, image, stride);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_construct(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool jpeg_compress_construct()
        {
            return jpeg_compress_construct(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_get_image(IntPtr context, Byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool jpeg_compress_get_image(byte[] buffer)
        {
            return jpeg_compress_get_image(this.Context, buffer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Int32 jpeg_compress_get_length(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int jpeg_compress_get_length()
        {
            return jpeg_compress_get_length(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Int32 jpeg_compress_get_jpeg_color_space(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JpegColorSpace jpeg_compress_get_jpeg_color_space()
        {
            return (JpegColorSpace)jpeg_compress_get_jpeg_color_space(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_set_image_height(IntPtr context, Int32 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool jpeg_compress_set_image_height(int value)
        {
            return jpeg_compress_set_image_height(this.Context, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_set_image_width(IntPtr context, Int32 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool jpeg_compress_set_image_width(int value)
        {
            return jpeg_compress_set_image_width(this.Context, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_set_in_color_space(IntPtr context, Int32 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool jpeg_compress_set_in_color_space(int value)
        {
            return jpeg_compress_set_in_color_space(this.Context, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_set_input_components(IntPtr context, Int32 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool jpeg_compress_set_input_components(int value)
        {
            return jpeg_compress_set_input_components(this.Context, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_set_quality(IntPtr context, Int32 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool jpeg_compress_set_quality(int value)
        {
            return jpeg_compress_set_quality(this.Context, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_set_sample_factor(IntPtr context, Int32 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool jpeg_compress_set_sample_factor(int value)
        {
            return jpeg_compress_set_sample_factor(this.Context, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_set_smoothing_factor(IntPtr context, Int32 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool jpeg_compress_set_smoothing_factor(int value)
        {
            return jpeg_compress_set_smoothing_factor(this.Context, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="predictor"></param>
        /// <param name="pointTransform"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_simple_lossless(IntPtr context, Int32 predictor, Int32 pointTransform);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predictor"></param>
        /// <param name="pointTransform"></param>
        /// <returns></returns>
        public bool jpeg_compress_simple_lossless(int predictor, int pointTransform)
        {
            return jpeg_compress_simple_lossless(this.Context, predictor, pointTransform);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_simple_progressive(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool jpeg_compress_simple_progressive()
        {
            return jpeg_compress_simple_progressive(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_compress_simple_spectral_selection(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool jpeg_compress_simple_spectral_selection()
        {
            return jpeg_compress_simple_spectral_selection(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="length"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_decompress_construct(IntPtr context, Int32 length, Byte[] image);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool jpeg_decompress_construct(byte[] image)
        {
            return jpeg_decompress_construct(this.Context, image.Length, image);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_decompress_decompress(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool jpeg_decompress_decompress()
        {
            return jpeg_decompress_decompress(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static int jpeg_decompress_get_data_precision(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int jpeg_decompress_get_data_precision()
        {
            return jpeg_decompress_get_data_precision(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_decompress_get_image(IntPtr context, Byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public bool jpeg_decompress_get_image(byte[] buffer)
        {
            return jpeg_decompress_get_image(this.Context, buffer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Int32 jpeg_decompress_get_length(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int jpeg_decompress_get_length()
        {
            return jpeg_decompress_get_length(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Int32 jpeg_decompress_get_out_color_space(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JpegColorSpace jpeg_decompress_get_out_color_space()
        {
            return (JpegColorSpace)jpeg_decompress_get_out_color_space(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Int32 jpeg_decompress_read_header(IntPtr context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int jpeg_decompress_read_header()
        {
            return jpeg_decompress_read_header(this.Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_decompress_set_jpeg_color_space(IntPtr context, Int32 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool jpeg_decompress_set_jpeg_color_space(int value)
        {
            return jpeg_decompress_set_jpeg_color_space(this.Context, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DllImport(LibraryPath)]
        extern static Boolean jpeg_decompress_set_out_color_space(IntPtr context, Int32 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool jpeg_decompress_set_out_color_space(int value)
        {
            return jpeg_decompress_set_out_color_space(this.Context, value);
        }
    }
}
