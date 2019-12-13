using System;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace EmuTarkov_Launcher
{
	public partial class Main : Form
	{
		private Monitor monitor;

		public Main()
		{
			InitializeComponent();
			InitializeLauncher();
		}

		private void InitializeLauncher()
		{
			// load configs
			Globals.LauncherConfig = Json.Load<LauncherConfig>(Globals.LauncherConfigFile);
			Globals.ClientConfig = Json.Load<ClientConfig>(Globals.ClientConfigFile);

			// set input
			EmailInput.Text = Globals.LauncherConfig.Email;
			PasswordInput.Text = Globals.LauncherConfig.Password;
			UrlInput.Text = Globals.LauncherConfig.BackendUrl;

			// setup monitor
			monitor = new Monitor("EscapeFromTarkov", 1000, MonitorCallback);
		}

		private void MonitorCallback(Monitor monitor)
		{
			// stop monitoring
			monitor.Stop();

			// show window
			this.Show();
		}

		private void EmailInput_TextChanged(object sender, EventArgs e)
		{
			// set and save input
			Globals.LauncherConfig.Email = EmailInput.Text;
			Json.Save<LauncherConfig>(Globals.LauncherConfigFile, Globals.LauncherConfig);
		}

		private void PasswordInput_TextChanged(object sender, EventArgs e)
		{
			// set and save input
			Globals.LauncherConfig.Password = PasswordInput.Text;
			Json.Save<LauncherConfig>(Globals.LauncherConfigFile, Globals.LauncherConfig);
		}

		private void UrlInput_TextChanged(object sender, EventArgs e)
		{
			// set and save input
			Globals.LauncherConfig.BackendUrl = UrlInput.Text;
			Json.Save<LauncherConfig>(Globals.LauncherConfigFile, Globals.LauncherConfig);
		}

		private void StartGame_Click(object sender, EventArgs e)
		{
			// detect if executable is found
			if (!System.IO.File.Exists(Globals.ClientExecutable))
			{
				MessageBox.Show("The launcher is not running from the Escape From Tarkov directory");
				return;
			}

			// set backend url
			Globals.ClientConfig.BackendUrl = Globals.LauncherConfig.BackendUrl;
			Json.Save<ClientConfig>(Globals.ClientConfigFile, Globals.ClientConfig);

			ProcessStartInfo clientProcess = new ProcessStartInfo(Globals.ClientExecutable);
			clientProcess.Arguments = GenerateToken(Globals.LauncherConfig.Email, Globals.LauncherConfig.Password) + " -screenmode=fullscreen";
			clientProcess.UseShellExecute = false;
			clientProcess.WorkingDirectory = Environment.CurrentDirectory;

			Process.Start(clientProcess);
			monitor.Start();

			if (Globals.LauncherConfig.MinimizeToTray)
			{
				TrayIcon.Visible = true;
				this.Hide();
			}
		}

		private string GenerateToken(string email, string password)
		{
			LoginToken token = new LoginToken(email, password);
			string beginKey = "-bC5vLmcuaS5u=";
			string endKey = "= -token=0";

			// serialize login token
			string serialized = Json.Serialize(token);

			// encode login token to base64
			string result = Convert.ToBase64String(Encoding.UTF8.GetBytes(serialized));

			// add begin and end part of the token
			return beginKey + result + endKey;
		}

		private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.Show();
			this.WindowState = FormWindowState.Normal;
		}

		private void Main_Resize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Normal)
			{
				TrayIcon.Visible = false;
			}
		}
	}
}
