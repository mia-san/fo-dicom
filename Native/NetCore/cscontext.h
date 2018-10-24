/* */
typedef struct jpeg_common_struct jpeg_common_struct;
typedef struct jpeg_compress_struct jpeg_compress_struct;
typedef struct jpeg_decompress_struct jpeg_decompress_struct;
typedef struct jpeg_destination_mgr jpeg_destination_mgr;
typedef struct jpeg_error_mgr jpeg_error_mgr;
typedef struct jpeg_source_mgr jpeg_source_mgr;

/*
*   context
*/
typedef struct
{
    union {
        /* common context */
        jpeg_common_struct m_common;

        /* compression context */
        jpeg_compress_struct m_cinfo;

        /* decompression context */
        jpeg_decompress_struct m_dinfo;
    };

    /* error manager */
    jpeg_error_mgr m_err;

    /* decompression - source manager */
    jpeg_source_mgr m_src;

    /* compression - destination manager */
    jpeg_destination_mgr m_dest;

    /* setjmp/longjmp context */
    jmp_buf m_jmpbuf;

    /* logger */
    void (*m_logger)(const char *message);

    /* decompression - decompressed image */
    void *m_image;

    /* compression - array of buffers */
    void **m_buffers;

    /* decompression - */
    size_t m_bytes_per_sample;

    /* decompressed image length */
    size_t m_length;

    /* true during context is valid */
    boolean m_valid;

} context;
