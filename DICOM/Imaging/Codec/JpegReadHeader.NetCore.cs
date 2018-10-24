namespace Dicom.Imaging.Codec
{
    /// <summary>
    /// return code from jpeg_reader_header
    /// excerpt from jpeglib*.h
    /// </summary>
    public static class JpegReadHeader
    {
        /// <summary>
        /// Suspended due to lack of input data
        /// </summary>
        public const int JPEG_SUSPENDED = 0;

        /// <summary>
        /// Found valid image datastream
        /// </summary>
        public const int JPEG_HEADER_OK = 1;

        /// <summary>
        /// Found valid table-specs-only datastream
        /// </summary>
        public const int JPEG_HEADER_TABLES_ONLY = 2;
    }
}
