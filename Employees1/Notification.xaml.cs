using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Animation;
//using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Employees1
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Window
    {
        DoubleAnimation anim;
        int left;
        int top;
        DependencyProperty prop;
        int end;
        public Notification()
        {
            InitializeComponent();
           
            TrayPos tpos = new TrayPos();
            tpos.getXY((int)this.Width, (int)this.Height, out top, out left, out prop, out end);
            this.Top = top;
            this.Left = left;
            anim = new DoubleAnimation(end, TimeSpan.FromSeconds(5));

        }
        private void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            //connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
            //connection.Open();
            //cmd = "SELECT distinct Expiration_date FROM Person";
            //command = new SQLiteCommand(cmd, connection);
            //reader = command.ExecuteReader();
            AnimationClock clock = anim.CreateClock();
            this.ApplyAnimationClock(prop, clock);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
