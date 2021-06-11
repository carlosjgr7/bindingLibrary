using Java.Lang;
using Java.Util.Concurrent;




namespace bindingLibrary.decode
{
    public class FrameQueue
    {
        string TAG = "FrameQueue";

        BlockingQueue<Frame> queue = ArrayBlockingQueue(60);


        public bool push(Frame frame)
        {
            if (queue.offer(frame, 5, TimeUnit.Milliseconds))
            {
                return true;
            }
            return false;
        }

        public Frame? pop()
        {
            try
            {
                Frame frame = queue.poll(1000, TimeUnit.Milliseconds);
            if (frame == null)
                {
                    System.Console.WriteLine("sin ningun frame que mostrar");
                }
                return frame;
              }
            catch ( InterruptedException e) {
                Thread.CurrentThread().Interrupt();
          }
            return null;
  
        }


        public void clear()
        {
            queue.clear();
         }


    }
}