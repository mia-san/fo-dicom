/*
*   
*/
GLOBAL(void)
logging(context *ctx, const char *message, ...)
{
    if (ctx->m_logger)
    {
        ctx->m_logger(message);
    }
}

/*
*   allocate context
*/
GLOBAL(context *)
jpeg_context_alloc(void (*logger)(const char *))
{
    context *ctx = calloc(1, sizeof(context));
    if (ctx)
    {
        ctx->m_logger = logger;
    }
    return ctx;
}

/*
*
*/
GLOBAL(void)
jpeg_context_free(context *ctx)
{
    if (ctx != NULL)
    {
        if (setjmp(ctx->m_jmpbuf) == 0)
        {
            if (ctx->m_valid)
            {
                ctx->m_valid = FALSE;

                if (ctx->m_common.is_decompressor)
                {
                    jpeg_destroy_decompress(&ctx->m_dinfo);
                }
                else
                {
                    jpeg_destroy_compress(&ctx->m_cinfo);
                }
            }
        }
    }

    free(ctx);
}
