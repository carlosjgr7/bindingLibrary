using Android.Media;
using Android.Views;
using Java.Lang;


namespace bindingLibrary.decode
{
    public class VideoDecodeThread : Thread
    {
        private  string TAG = "VideoDecodeThread";
        private  Surface surface;
        private  string mimeType;
        private  int width;
        private  int height;
        private  FrameQueue videoFrameQueue;

        public VideoDecodeThread( Surface surface, string mimeType, int width, int height, FrameQueue videoFrameQueue)
        {
            this.surface = surface;
            this.mimeType = mimeType;
            this.width = width;
            this.height = height;
            this.videoFrameQueue = videoFrameQueue;
        }



        // esta funncion debo pasarla a un hilo para que corra en segundo plano
        public void Run()
        {
            MediaCodec decoder = MediaCodec.CreateDecoderByType(mimeType);
            MediaFormat format = MediaFormat.CreateVideoFormat(mimeType, width, height);

            decoder.Configure(format, surface, null, 0);
            decoder.Start();
            MediaCodec.BufferInfo bufferInfo = new MediaCodec.BufferInfo();

            while (/*Revisar si el hilo sigue andando*/true)
            {
                int inIndex = decoder.DequeueInputBuffer(10000L);
                if (inIndex >= 0)
                {
                    Java.Nio.ByteBuffer byteBuffer = decoder.GetInputBuffer(inIndex);

                    byteBuffer?.Rewind();

                    Frame frame = new Frame();
                    try
                    {
                        frame = videoFrameQueue.pop();
                        if (frame == null)
                        {
                            System.Console.WriteLine(TAG, "Empty frame");
                        }
                        else
                        {
                            byteBuffer.Put(frame.data, frame.offset, frame.length);
                            decoder.QueueInputBuffer(inIndex, frame.offset, frame.length, frame.timestamp, 0);
                        }
                    }
                    catch (Exception e)
                    {
                        e.PrintStackTrace();
                    }
                }
                try
                {

                    int outIndex = decoder.DequeueOutputBuffer(bufferInfo, 10000);
                    switch (outIndex)
                    {
                        case (int)MediaCodecInfoState.OutputFormatChanged:
                            System.Console.WriteLine(TAG, "Decoder format changed: " + decoder.OutputFormat);
                            break;
                        case (int)MediaCodecInfoState.TryAgainLater:
                            if (outIndex >= 0)
                            {
                                decoder.ReleaseOutputBuffer(outIndex, bufferInfo.Size != 0);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    e.PrintStackTrace();
                }

                // All decoded frames have been rendered, we can stop playing now
                int flags = (int)bufferInfo.Flags;
                int mediaCodeFlags = (int)MediaCodecBufferFlags.EndOfStream;

                if ((flags != 0) && (mediaCodeFlags != 0))
                {
                    System.Console.WriteLine(TAG, "OutputBuffer BUFFER_FLAG_END_OF_STREAM");
                    break;
                }

            }
            decoder.Stop();
            decoder.Release();
            videoFrameQueue.clear();
        }
    }
}

