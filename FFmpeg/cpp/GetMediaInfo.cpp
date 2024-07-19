//GetMediaInfo.cpp
extern "C" {
#include <libavformat/avformat.h>
#include <libavutil/dict.h>
#include <stdio.h>
}

// 函数用于打开文件并获取文件信息
void GetFileInfo(const char* filename) {
    // 注册所有的解码器
    av_register_all();

    // 打开输入流并分配格式上下文
    AVFormatContext* formatContext = nullptr;
    if (avformat_open_input(&formatContext, filename, nullptr, nullptr) < 0) {
        // 打开输入流失败
        fprintf(stderr, "Could not open input file '%s'\n", filename);
        return;
    }

    // 查找流信息
    if (avformat_find_stream_info(formatContext, nullptr) < 0) {
        // 找不到流信息
        fprintf(stderr, "Failed to retrieve input stream information\n");
        avformat_close_input(&formatContext);
        return;
    }

    // 打印格式信息
    printf("Format: %s\n", formatContext->iformat->name);
    printf("Duration: %lld us (%lld ms)\n", formatContext->duration, formatContext->duration / 1000);
    printf("Bitrate: %lld\n", formatContext->bit_rate);

    // 打印流信息
    for (unsigned int i = 0; i < formatContext->nb_streams; i++) {
        AVStream* stream = formatContext->streams[i];
        AVCodecParameters* codecParams = stream->codecpar;
        AVCodec* codec = avcodec_find_decoder(codecParams->codec_id);

        printf("\nStream %d:\n", i);
        printf("Codec: %s\n", codec ? codec->name : "None");
        printf("Codec Type: %s\n", av_get_media_type_string(codecParams->codec_type));
        printf("Duration: %lld us (%lld ms)\n", stream->duration, stream->duration / 1000);
        printf("Bitrate: %lld\n", codecParams->bit_rate);
        printf("Width: %d\n", codecParams->width);
        printf("Height: %d\n", codecParams->height);
    }

    // 释放格式上下文
    avformat_close_input(&formatContext);
}