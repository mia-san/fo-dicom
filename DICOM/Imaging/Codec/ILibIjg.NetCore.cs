using System;


namespace Dicom.Imaging.Codec
{
    /// <summary>
    /// libijg(8|12|16) interface
    /// </summary>
    interface ILibIjg : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="stride"></param>
        /// <returns></returns>
        bool jpeg_compress_compress(byte[] image, int stride);

        /// <summary>
        /// construct context for compression
        /// </summary>
        /// <returns></returns>
        bool jpeg_compress_construct();

        /// <summary>
        /// get compressed image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        bool jpeg_compress_get_image(byte[] buffer);

        /// <summary>
        /// get compressed image length
        /// </summary>
        /// <returns></returns>
        int jpeg_compress_get_length();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        JpegColorSpace jpeg_compress_get_jpeg_color_space();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool jpeg_compress_set_image_height(int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool jpeg_compress_set_image_width(int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool jpeg_compress_set_in_color_space(int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool jpeg_compress_set_input_components(int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool jpeg_compress_set_quality(int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool jpeg_compress_set_sample_factor(int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool jpeg_compress_set_smoothing_factor(int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predictor"></param>
        /// <param name="pointTransform"></param>
        /// <returns></returns>
        bool jpeg_compress_simple_lossless(int predictor, int pointTransform);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool jpeg_compress_simple_progressive();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool jpeg_compress_simple_spectral_selection();

        /// <summary>
        /// create decompression context for the image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        bool jpeg_decompress_construct(byte[] image);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool jpeg_decompress_decompress();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int jpeg_decompress_get_data_precision();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        bool jpeg_decompress_get_image(byte[] buffer);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int jpeg_decompress_get_length();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        JpegColorSpace jpeg_decompress_get_out_color_space();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int jpeg_decompress_read_header();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool jpeg_decompress_set_jpeg_color_space(int value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool jpeg_decompress_set_out_color_space(int value);
    }
}
