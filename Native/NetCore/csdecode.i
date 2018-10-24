/*
*
*/
METHODDEF(void)
error_exit(j_common_ptr common)
{
    context *ctx = (context *)common;

    common->err->output_message(common);

    /* LOKTOFEIT */
    longjmp(ctx->m_jmpbuf, -1);
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
    logging(ctx, buffer);
}

/*
*   jpeg_source_mgr - init_source 
*/
METHODDEF(void)
init_source(j_decompress_ptr cinfo)
{
}

/*
*   jpeg_source_mgr - fill_input_buffer
*
*   as whole image data is given at construction,
*   we don't have anything to fill buffer.
*/
METHODDEF(boolean)
fill_input_buffer(j_decompress_ptr cinfo)
{
    return FALSE;
}

/*
*   jpeg_source_mgr - skip_input_data
*/
void skip_input_data(j_decompress_ptr cinfo, long num_bytes)
{
}

/*
*   jpeg_source_mgr - term_source
*/
void term_source(j_decompress_ptr cinfo)
{
}

/*
*
*/
GLOBAL(void)
jpeg_decompress_destruct(context *ctx)
{
    if (ctx != NULL)
    {
        if (setjmp(ctx->m_jmpbuf) == 0)
        {
            jpeg_destroy_decompress(&ctx->m_dinfo);
        }
    }

    free(ctx);
}

/**
*   decompress image in byte array
*
*   CLR may have to copy whole image from managed memory to here, but this is easy to understand.
*/
GLOBAL(boolean)
jpeg_decompress_construct(context *ctx, size_t length, void *image)
{
    jpeg_error_mgr *err = jpeg_std_error(&ctx->m_err);
    err->error_exit = error_exit;
    err->output_message = output_message;
    ctx->m_dinfo.err = err;

    /* return NULL on error */
    if (setjmp(ctx->m_jmpbuf) != 0)
    {
        jpeg_decompress_destruct(ctx);
        return FALSE;
        /* NOTREACHED */
    }

    jpeg_create_decompress(&ctx->m_dinfo);
    ctx->m_valid = TRUE;

    /* duplicate whole image */
    void *buffer = ctx->m_dinfo.mem->alloc_large((j_common_ptr)&ctx->m_dinfo, JPOOL_PERMANENT, length);
    memcpy(buffer, image, length);
    ctx->m_src.bytes_in_buffer = length;
    ctx->m_src.next_input_byte = buffer;
    ctx->m_src.init_source = init_source;
    ctx->m_src.fill_input_buffer = fill_input_buffer;
    ctx->m_src.skip_input_data = skip_input_data;
    ctx->m_src.resync_to_restart = jpeg_resync_to_restart;
    ctx->m_src.term_source = term_source;
    ctx->m_dinfo.src = &ctx->m_src;

    return TRUE;
}

/*
*
*/
GLOBAL(int32_t)
jpeg_decompress_get_out_color_space(context *ctx)
{
    return ctx->m_dinfo.out_color_space;
}

/*
*
*/
GLOBAL(int32_t)
jpeg_decompress_read_header(context *ctx)
{
    int rc = JPEG_SUSPENDED;

    if (setjmp(ctx->m_jmpbuf) == 0)
    {
        rc = jpeg_read_header(&ctx->m_dinfo, TRUE);
    }

    return rc;
}

/*
*   out_color_space - setter
*/
GLOBAL(boolean)
jpeg_decompress_set_jpeg_color_space(context *ctx, int32_t value)
{
    ctx->m_dinfo.jpeg_color_space = value;
    return TRUE;
}

/*
*   out_color_space - setter
*/
GLOBAL(boolean)
jpeg_decompress_set_out_color_space(context *ctx, int32_t value)
{
    ctx->m_dinfo.out_color_space = value;
    return TRUE;
}

/*
*   decompress
*/
GLOBAL(boolean)
jpeg_decompress_decompress(context *ctx)
{
    boolean rc = FALSE;

    if (setjmp(ctx->m_jmpbuf) == 0)
    {
        jpeg_calc_output_dimensions(&ctx->m_dinfo);

        jpeg_start_decompress(&ctx->m_dinfo);

        /* samples per row */
        int samples_per_row = ctx->m_dinfo.output_width * ctx->m_dinfo.output_components;

        /* bytes per row */
        int bytes_per_row = sizeof(JSAMPLE) * samples_per_row;

        /* number of bytes for image. might be odd number. */
        int frame_size = bytes_per_row * ctx->m_dinfo.output_height;

        /* allocate output buffer */
        ctx->m_length = frame_size;
        ctx->m_image = ctx->m_dinfo.mem->alloc_large((j_common_ptr)&ctx->m_dinfo, JPOOL_PERMANENT, frame_size);

        JSAMPROW row = ctx->m_image;
        while (ctx->m_dinfo.output_scanline < ctx->m_dinfo.output_height)
        {
            JDIMENSION row_count = jpeg_read_scanlines(&ctx->m_dinfo, &row, 1);
            row += samples_per_row * row_count;
        }

        rc = TRUE;
    }

    return rc;
}

/*
*   decompressed image length - getter
*/
GLOBAL(int32_t)
jpeg_decompress_get_length(context *ctx)
{
    return ctx->m_length;
}

/*
*   decompressed image - getter
*/
GLOBAL(boolean)
jpeg_decompress_get_image(context *ctx, void *image)
{
    memcpy(image, ctx->m_image, ctx->m_length);
    return TRUE;
}
