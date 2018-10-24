namespace Dicom.Imaging.Codec
{
    /// <summary>
    /// borrowed from jpeglib*.h
    /// /// </summary>
    public enum JpegColorSpace
    {
        JCS_UNKNOWN,    /* error/unspecified */
        JCS_GRAYSCALE,  /* monochrome */
        JCS_RGB,        /* red/green/blue */
        JCS_YCbCr,      /* Y/Cb/Cr (also known as YUV) */
        JCS_CMYK,       /* C/M/Y/K */
        JCS_YCCK        /* Y/Cb/Cr/K */
    };
}
