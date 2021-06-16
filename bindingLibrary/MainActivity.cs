using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using bindingLibrary.decode;
using Java.Lang;
using Java.Util.Concurrent.Atomic;


namespace bindingLibrary
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISurfaceHolderCallback
    {


        private FrameQueue videoFrameQueue = new FrameQueue();
        private RtspThread rtspThread = null;
        private VideoDecodeThread videoDecodeThread = null;
        private AtomicBoolean rtspStopped = new AtomicBoolean(true);
        private Button btnStartStop = null;
        private Surface surface = null;
        private int surfaceWidth = 1920;
        private int surfaceHeight = 1080;
        private CheckBox checkVideo = null;
        private TextView textStatus = null;
        private View progressBar = null;
        private string videoMimeType = "";



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



        public void onRtspClientStarted()
        {
            rtspStopped.Set(false);
            btnStartStop.Text = "Stop RTSP";
        }

        public void onRtspClientStopped()
        {
            rtspStopped.Set(true);
            btnStartStop.Text = "Start RTSP";
            videoDecodeThread.Interrupt();
            videoDecodeThread = null;
        }

        public void onRtspClientConnected()
        {
     
            if ((videoMimeType!="") && (checkVideo!.Checked))
            {
               videoDecodeThread = new VideoDecodeThread(surface!, videoMimeType, surfaceWidth, surfaceHeight, videoFrameQueue);
               videoDecodeThread?.Start();
            }
        
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            surface = holder.Surface;
            surfaceWidth = width;
            surfaceHeight = height;

            if (videoDecodeThread != null)
                {
                    videoDecodeThread.Interrupt();
                    videoDecodeThread = new VideoDecodeThread(surface, videoMimeType, width, height, videoFrameQueue);
                    videoDecodeThread.Start();
                }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            throw new System.NotImplementedException();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            videoDecodeThread?.Interrupt();
            videoDecodeThread = null;    
        }

        private class RtspThread: Thread
        {
            public override void Run()
            {
                base.Run();
                new Handler(Looper.MainLooper).Post(new);
            }

        }
    }
}