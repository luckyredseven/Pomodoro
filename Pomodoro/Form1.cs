using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shell;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Pomodoro
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        int intervalTime = 0;
        TimeSpan minutes;
        DateTime start; 
        DateTime endTime;

        public Form1()
        {
            InitializeComponent();
            txt_interval.Text = "25";
        }


        //
        //
        private void btn_go_Click(object sender, EventArgs e)
        {
            intervalTime = Convert.ToInt16(txt_interval.Text);
            minutes = TimeSpan.FromMinutes(intervalTime); 
            start = DateTime.UtcNow; // Use UtcNow instead of Now
            endTime = start.AddMinutes(intervalTime); //endTime is a member, not a local variable

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan remainingTime = endTime - DateTime.UtcNow;
            if (remainingTime < TimeSpan.Zero)
            {
                this.Text = "Pomodoro";
                label1.Text = "Done!";
                timer1.Enabled = false;
                FlashWindowEx(this);
            }
            else
            {
                this.Text = remainingTime.ToString();
            }
        }

        //Flash both the window caption and taskbar button.
        //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
        public const UInt32 FLASHW_ALL = 3;

        // Flash continuously until the window comes to the foreground. 
        public const UInt32 FLASHW_TIMERNOFG = 12;

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        // Do the flashing - this does not involve a raincoat.
        public static bool FlashWindowEx(Form form)
        {
            IntPtr hWnd = form.Handle;
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }
    }
}

