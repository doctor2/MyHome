using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Employees1
{
    public class Empl
    {
        public string FIO { get; set; }
        public string TypePost { get; set; }
        public string TypeCor { get; set; }
        public string Expiration_date { get; set; }
        public string Department { get; set; }

    }
    public partial class MainWindow : Window
    {
        Notification notify;
        //NotifyIcon TrayPos = new NotifyIcon();
        //NotifyIcon notifyIcon1 = new NotifyIcon();
        string temp;
        private void timerTick(object sender, EventArgs e)
        {
            timer.Start();
            connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
            connection.Open();
            cmd = "SELECT distinct Expiration_date FROM Person";
            command = new SQLiteCommand(cmd, connection);
            reader = command.ExecuteReader();

            foreach (DbDataRecord record in reader)
            {
                temp = String.Format("{0:dd/MM/yyyy }", record["Expiration_date"]);
                if (String.Format("{0:dd/MM/yyyy }", DateTime.Today.AddDays(10)) == temp)
                {
                    notify = new Notification();
                    notify.Show();
                }
                if (String.Format("{0:dd/MM/yyyy }", DateTime.Today.AddDays(7)) == temp)
                {
                    notify = new Notification();
                    notify.Show();
                }
                if (String.Format("{0:dd/MM/yyyy }", DateTime.Today.AddDays(5)) == temp)
                {
                    notify = new Notification();
                    notify.Show();
                }
                if (String.Format("{0:dd/MM/yyyy }", DateTime.Today.AddDays(3)) == temp)
                {
                    Show();
                    notify = new Notification();
                    notify.Show();
                }
                if (String.Format("{0:dd/MM/yyyy }", DateTime.Today) == temp)
                {
                    Show();
                    notify = new Notification();
                    notify.Owner = this;
                    notify.Show();
                    //показ всплывающего окна с сообщением из трея System.Windows.Forms.
                   // TrayPos.ShowBalloonTip(50000, "Сообщение", "Я свернулась:)", ToolTipIcon.Info); //, ToolTipIcon.Warning
                }
              //  AddDepart.Text = String.Format("{0:dd/MM/yyyy }", record["Expiration_date"]) + AddDepart.Text;//record["Expiration_date"].ToString());
              //  AddPos.Text = String.Format("{0:dd/MM/yyyy }", DateTime.Today.AddDays(3));
            }
            connection.Close();
            //Search.Visibility = Visibility.Hidden;
        }
    }
}
