const DicomJpegSampleFactor_SF444 = 0;
const DicomJpegSampleFactor_SF422 = 1;
const DicomJpegSampleFactor_Unknown = 2;

/*
*
*/
METHODDEF(void)
error_exit(j_common_ptr common)
{
    context *ctx = (context *)common;

    common->err->output_message(common);

    /* LOKTOFEIT */
    longjmp(&ctx->m_jmpbuf, -1);
}

/*
*
*/
METHODDEF(void)
output_message(j_common_ptr common)
{
    context *ctx = (context *)common;

    /* format message into buffer */
    char buffer[JMSG_LENGTH_MAX];
    common->err->format_message(common, buffer);

    /* invoke logger */
    logging(buffer);
}

/* buffer table size */
const size_t BUFFER_TABLE_SIZE = 16;

/* size of each block */
const size_t BUFFER_BLOCK_SIZE = 1 * 1024 * 1024;

/*
* destination manager - empty_output_buffer
*/
METHODDEF(boolean)
empty_output_buffer(j_common_ptr common)
{
    context *ctx = (context *)common;
    j_compress_ptr cinfo = &ctx->m_cinfo;

    size_t c = ctx->m_length / BUFFER_BLOCK_SIZE;
    if (c % BUFFER_TABLE_SIZE == 0)
    {
        /* expand buffer table */
        void **buffers = cinfo->mem->alloc_small(cinfo, JPOOL_PERMANENT, (c + BUFFER_TABLE_SIZE) * sizeof(void *));
        memcpy(buffers, ctx->m_buffers, c * sizeof(void *));
        ctx->m_buffers = buffers;
    }

    /* allocate buffer block */
    void *buffer = cinfo->mem->alloc_large(cinfo, JPOOL_PERMANENT, BUFFER_BLOCK_SIZE);
    ctx->m_buffers[c] = buffer;

    /* update total number of bytes */
    ctx->m_length += BUFFER_BLOCK_SIZE;

    /* update destination manager */
    cinfo->dest->next_output_byte = buffer;
    cinfo->dest->free_in_buffer = BUFFER_BLOCK_SIZE;

    return TRUE;
}

/*
* destination manager - init_destination
*/
METHODDEF(void)
init_destination(j_common_ptr common)
{
}

/*
* destination manager - term_destination
*/
METHODDEF(void)
term_destination(j_common_ptr common)
{
    context *ctx = (context *)common;

    /* adjust total number of bytes */
    ctx->m_length -= ctx->m_cinfo.dest->free_in_buffer;
}

/*
*   setup context for compression
*/
GLOBAL(boolean)
jpeg_compress_construct(context *ctx)
{
    jpeg_error_mgr *err = jpeg_std_error(&ctx->m_err);
    err->error_exit = error_exit;
    err->output_message = output_message;
    ctx->m_cinfo.err = err;

    /* return NULL on error */
    if (setjmp(&ctx->m_jmpbuf) != 0)
    {
        return FALSE;
        /* NOTREACHED */
    }

    jpeg_create_compress(&ctx->m_cinfo);
    ctx->m_valid = TRUE;

    jpeg_set_defaults(&ctx->m_cinfo);
    ctx->m_cinfo.optimize_coding = TRUE;

    return TRUE;
}

/*
*   compression - total number of bytes written - getter
*/
GLOBAL(int32_t)
jpeg_compress_get_image_length(context *ctx)
{
    return ctx->m_length;
}

/*
*   compression - compressed image - getter
*/
GLOBAL(boolean)
jpeg_compress_get_image(context *ctx, void *buffer)
{
    char *pby = buffer;
    void **ppv = ctx->m_buffers;
    size_t cby = ctx->m_length;
    for (size_t cby = 0; cby < ctx->m_length; cby += BUFFER_BLOCK_SIZE, pby += BUFFER_BLOCK_SIZE, ++ppv)
    {
        size_t c = min(ctx->m_length - cby, BUFFER_BLOCK_SIZE);
        memcpy(pby, *ppv, c);
    }

    return TRUE;
}

/*
*   compression - bytes per sample - setter
*/
GLOBAL(boolean)
jpeg_compress_set_bytes_per_sample(context *ctx, int32_t value)
{
    ctx->m_bytes_per_sample = value;
    return TRUE;
}

/*
*   compression - image_height - setter
*/
GLOBAL(boolean)
jpeg_compress_set_image_height(context *ctx, int32_t value)
{
    ctx->m_cinfo.image_height = value;
    return TRUE;
}

/*
*   compression - image_width - setter
*/
GLOBAL(boolean)
jpeg_compress_set_image_width(context *ctx, int32_t value)
{
    ctx->m_cinfo.image_width = value;
    return TRUE;
}

/*
*   compression - input_components - setter
*/
GLOBAL(boolean)
jpeg_compress_set_input_components(context *ctx, int32_t value)
{
    ctx->m_cinfo.input_components = value;
    return TRUE;
}

/*
*   compression - input_components - setter
*/
GLOBAL(boolean)
jpeg_compress_set_in_color_space(context *ctx, int32_t value)
{
    ctx->m_cinfo.in_color_space = value;
    return TRUE;
}

/*
*   compression - set_quality
*/
GLOBAL(boolean)
jpeg_compress_set_quality(context *ctx, int32_t value)
{
    boolean rc = FALSE;

    if (setjmp(&ctx->m_jmpbuf) == 0)
    {
        jpeg_set_quality(&ctx->m_cinfo, value, FALSE);
        rc = TRUE;
    }

    return rc;
}

/*
*   compression - smoothing_factor - setter
*/
GLOBAL(boolean)
jpeg_compress_set_smoothing_factor(context *ctx, int32_t value)
{
    ctx->m_cinfo.smoothing_factor = value;
    return TRUE;
}

/*
*
*/
GLOBAL(boolean)
jpeg_compress_simple_spectral_selection(context *ctx)
{
    boolean rc = FALSE;

    if (setjmp(&ctx->m_jmpbuf) == 0)
    {
        jpeg_simple_spectral_selection(&ctx->m_cinfo);
        rc = TRUE;
    }

    return rc;
}

/*
*
*/
GLOBAL(boolean)
jpeg_compress_simple_progressive(context *ctx)
{
    boolean rc = FALSE;

    if (setjmp(&ctx->m_jmpbuf) == 0)
    {
        jpeg_simple_progression(&ctx->m_cinfo);
        rc = TRUE;
    }

    return rc;
}

/*
*
*/
GLOBAL(boolean)
jpeg_compress_simple_lossless(context *ctx, int32_t predictor, int32_t pointTransform)
{
    boolean rc = FALSE;

    if (setjmp(&ctx->m_jmpbuf) == 0)
    {
        jpeg_simple_lossless(&ctx->m_cinfo, predictor, pointTransform);
        rc = TRUE;
    }

    return rc;
}

/*
*   initialize sampling factors
*/
GLOBAL(boolean)
jpeg_compress_set_sample_factor(context *ctx, int32_t sampleFactor)
{
    boolean rc = FALSE;
    j_compress_ptr cinfo = &ctx->m_cinfo;

    if (setjmp(&ctx->m_jmpbuf) == 0)
    {
        if (cinfo->lossless)
        {
            jpeg_set_colorspace(cinfo, cinfo->in_color_space);
            cinfo->comp_info[0].h_samp_factor = 1;
            cinfo->comp_info[0].v_samp_factor = 1;
        }
        else if (cinfo->jpeg_color_space == JCS_YCbCr && sampleFactor != DicomJpegSampleFactor_Unknown)
        {
            switch (sampleFactor)
            {
            case DicomJpegSampleFactor_SF444:
                /* 4:4:4 sampling (no subsampling) */
                cinfo->comp_info[0].h_samp_factor = 1;
                cinfo->comp_info[0].v_samp_factor = 1;
                break;

            case DicomJpegSampleFactor_SF422:
                /* 4:2:2 sampling (horizontal subsampling of chroma components) */
                cinfo->comp_info[0].h_samp_factor = 2;
                cinfo->comp_info[0].v_samp_factor = 1;
                break;

                //case DicomJpegSampleFactor::SF411: /* 4:1:1 sampling (horizontal and vertical subsampling of chroma components) */
                //	cinfo->comp_info[0].h_samp_factor = 2;
                //	cinfo->comp_info[0].v_samp_factor = 2;
                //	break;

            default:
                break;
            }
        }
        else
        {
            if (sampleFactor == DicomJpegSampleFactor_Unknown)
            {
                jpeg_set_colorspace(cinfo, cinfo->in_color_space);
            }

            // JPEG color space is not YCbCr, disable subsampling.
            cinfo->comp_info[0].h_samp_factor = 1;
            cinfo->comp_info[0].v_samp_factor = 1;
        }

        for (int i = 1; i < MAX_COMPONENTS; ++i)
        {
            cinfo->comp_info[i].h_samp_factor = 1;
            cinfo->comp_info[i].v_samp_factor = 1;
        }

        rc = TRUE;
    }

    return rc;
}

/**
*
*/
GLOBAL(boolean)
jpeg_compress_compress(context *ctx, const void *image, int32_t stride)
{
    boolean rc = FALSE;
    j_compress_ptr cinfo = &ctx->m_cinfo;

    if (setjmp(&ctx->m_jmpbuf) == 0)
    {
        // specify destination manager
        jpeg_destination_mgr dest;
        dest.init_destination = init_destination;
        dest.empty_output_buffer = empty_output_buffer;
        dest.term_destination = term_destination;
        cinfo->dest = &dest;

        jpeg_start_compress(cinfo, TRUE);

        for (JSAMPROW row = image; cinfo->next_scanline < cinfo->image_height; row += cinfo->next_scanline * stride)
        {
            jpeg_write_scanlines(cinfo, &row, 1);
        }

        jpeg_finish_compress(cinfo);
        rc = TRUE;
    }

    return rc;
}
