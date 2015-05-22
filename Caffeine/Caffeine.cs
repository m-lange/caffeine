using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Caffeine
{
    class Caffeine
    {
        private NotifyIcon notifyIcon;
        private Timer timer;

        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem exit;
        private ToolStripMenuItem displayRequired;
        private ToolStripMenuItem systemRequired;


        public Caffeine()
        {
            exit = new ToolStripMenuItem();
            exit.Text = Resource.Exit;
            exit.Click += new EventHandler(exit_Click);

            displayRequired = new ToolStripMenuItem();
            displayRequired.Text = Resource.DisplayRequired;
            displayRequired.Checked = true;
            displayRequired.CheckState = CheckState.Checked;
            displayRequired.Click += new EventHandler(displayRequired_Click);

            systemRequired = new ToolStripMenuItem();
            systemRequired.Text = Resource.SystemRequired;
            systemRequired.Checked = true;
            systemRequired.CheckState = CheckState.Checked;
            systemRequired.Click += new EventHandler(systemRequired_Click);

            contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(new ToolStripItem[] {
                displayRequired,
                systemRequired,
                new ToolStripSeparator(),
                exit
            });

            notifyIcon = new NotifyIcon();
            notifyIcon.BalloonTipTitle = Resource.BalloonTipTitle;
            notifyIcon.BalloonTipText = Resource.BalloonTipText;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.Text = Resource.BalloonTipTitle;
            notifyIcon.Icon = Resource.NotifyIcon;
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.Visible = true;

            timer = new Timer();
            timer.Interval = 30000;
            timer.Tick += new EventHandler(SetThreadExecutionState);

            SetThreadExecutionState();
            notifyIcon.ShowBalloonTip(5000);
            timer.Start();
        }


        private void exit_Click(object sender, EventArgs e)
        {
            timer.Stop();
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            notifyIcon.Visible = false;
            Application.Exit();
        }


        private void displayRequired_Click(object sender, EventArgs e)
        {
            displayRequired.Checked = !displayRequired.Checked;
            SetThreadExecutionState();
        }


        private void systemRequired_Click(object sender, EventArgs e)
        {
            systemRequired.Checked = !systemRequired.Checked;
            SetThreadExecutionState();
        }


        private void SetThreadExecutionState(object sender, EventArgs e)
        {
            SetThreadExecutionState();
            JiggleMouse();
        }


        private void SetThreadExecutionState()
        {
            EXECUTION_STATE esFlags = EXECUTION_STATE.ES_CONTINUOUS;
            if (displayRequired.Checked)
                esFlags |= EXECUTION_STATE.ES_DISPLAY_REQUIRED;
            if (systemRequired.Checked)
                esFlags |= EXECUTION_STATE.ES_SYSTEM_REQUIRED;

            SetThreadExecutionState(esFlags);
        }


        private void JiggleMouse()
        {
            if (displayRequired.Checked)
            {
                INPUT input = new INPUT();
                input.type = 0;
                input.mi = new MOUSEINPUT();
                input.mi.dx = 0;
                input.mi.dy = 0;
                input.mi.mouseData = 0;
                input.mi.dwFlags = 0x0001;
                input.mi.time = 0;
                input.mi.dwExtraInfo = IntPtr.Zero;
                SendInput(1, new INPUT[] { input }, Marshal.SizeOf(input));
            }
        }


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);


        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
    }


    [FlagsAttribute]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public int type;
        public MOUSEINPUT mi;
    }

}
