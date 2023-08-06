using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ServiceGeneric
{
    partial class ServiceGenericAutomation : ServiceBase
    {
        static System.Timers.Timer _timer;
        static string timeString = string.Empty;
        static string _ScheduledRunningTime = "8:00 PM";
        public ServiceGenericAutomation()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ///*live version*/
            _timer = new System.Timers.Timer();
            _timer.AutoReset = true;
            _timer.Enabled = true;
            Console.WriteLine("===> Generic Automation Service started ... ");
            ///*live version*/

            /*debug mode*/
            //while (true)
            //{
            //    try
            //    {
            //        ProcessInvoke.ExecuteFunction();
            //    }
            //    catch (Exception ee)
            //    {
            //        ee.Data.Clear();
            //    }
            //    finally
            //    {
            //        Thread.Sleep(1500);
            //    }
            //}
            /*debug mode*/
            try
            {
                _timer = new System.Timers.Timer();
                _timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;//Every one minute
                _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                _timer.Start();
            }
            catch (Exception ex)
            {
                //Displays and Logs Message
                Console.WriteLine(" Error occured at ===> " + ex.Message.ToString());
                UTILITIES.WriteLog("\" Error occured \" ===> " + ex.Message.ToString());
            }

            _stop.Reset();
            ServiceTimer_Tick(this, null);
            //ThreadPool.RegisterWaitForSingleObject(_stop, new WaitOrTimerCallback(PeriodicProcess), null, 1000, true);
        }
        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Displays and Logs Message
            Console.WriteLine("timer_Elapsed method at :" + DateTime.Now);
            UTILITIES.WriteLog("timer_Elapsed method at :" + DateTime.Now);

            string _CurrentTime = String.Format("{0:t}", DateTime.Now);
            if (_CurrentTime == _ScheduledRunningTime)
            {
                ProcessInvoke.ExecuteFunction();
            }
        }
        private void ServiceTimer_Tick(object sender, ElapsedEventArgs e)
        {
            timeString = ConfigurationManager.AppSettings["StartTime"];
            DateTime t = DateTime.Parse(timeString);
            if (t > DateTime.Now)
            {
                ProcessInvoke.ExecuteFunction();
                Thread.Sleep(1000);
            }
            else
            {
                Console.WriteLine("===> Task time not yet reached, Service paused ... ");
                Thread.Sleep(1000);
            }

            _timer.Stop();
            Thread.Sleep(1000000);
            SetTimer();
        }
        protected override void OnStop()
        {
            _timer.AutoReset = false;
            _timer.Enabled = false;
            Console.WriteLine("===> Generic Automation Service stopped ... ");
            _stop.Set();
        }

        public void Start()
        {
            OnStart(null);
        }
        public void Stop()
        {
            OnStop();
        }

        private void PeriodicProcess(object state, bool timeout)
        {
            if (timeout)
            {
                // Periodic processing here
                //CommonFunctions.WriteLog(string.Format("{0}\tOk....", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")));

                ProcessInvoke.ExecuteFunction();

                // Then queue another wait
                ThreadPool.RegisterWaitForSingleObject(_stop, new WaitOrTimerCallback(PeriodicProcess), null, 86400000, true);
            }
        }
        private double GetNextInterval()
        {
            timeString = ConfigurationManager.AppSettings["StartTime"];
            DateTime t = DateTime.Parse(timeString);
            TimeSpan ts = new TimeSpan();
            int x;
            ts = t - DateTime.Now;
            if (ts.TotalMilliseconds < 0)
            {
                ts = t.AddDays(1) - DateTime.Now;//Here you can increase the timer interval based on your requirments.   
            }
            return ts.TotalMilliseconds;
        }
        private void SetTimer()
        {
            try
            {
                double inter = (double)GetNextInterval();
                _timer.Interval = inter;
                _timer.Start();
            }
            catch (Exception ex)
            {
            }
        }

        private ManualResetEvent _stop = new ManualResetEvent(false);
        // ------------------------------------
    }
}
