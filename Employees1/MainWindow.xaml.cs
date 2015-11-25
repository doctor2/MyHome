using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Data.SQLite;
using System.Data.Common;

//using System.Data;

namespace Employees1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            // timer.Interval = new TimeSpan(24, 0, 0);
            timer.Interval = new TimeSpan(0, 0, 60);
            timer.Start();
        }
        string databaseName = "Employees.db";
        string cmd;
        SQLiteCommand command;
        SQLiteDataReader reader;
        SQLiteConnection connection;
        System.Windows.Threading.DispatcherTimer timer;
        List<Empl> itemList;// = new List<Empl>();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           //  var db = new SQLiteConnection("Employees.db");
          //  SQLiteConnection.CreateFile(databaseName);
            connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
            connection.Open();
            //command = new SQLiteCommand("CREATE TABLE datact1 (abc int NOT NULL);", connection);
            //command.ExecuteNonQuery();
            //SQLiteCommand command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;", connection);
            //command.ExecuteNonQuery();

            //cmd = "Drop TABLE Person;";
           // cmd = "CREATE TABLE Person(FIO nvarchar(120) NOT NULL, TypePost nvarchar(200) NOT NULL,Department nvarchar(200) NOT NULL, TypeCor nvarchar(200) NOT NULL, Expiration_date Date);";
            //command.ExecuteNonQuery();
            //command = new SQLiteCommand(cmd, connection);
            //reader = command.ExecuteReader();
         // command = new SQLiteCommand(cmd, connection);// SQLiteCommand
           //reader = command.ExecuteReader();
             //cmd = "SELECT * FROM sqlite_master WHERE type='table' ORDER BY name;";
             //cmd = "SELECT FIO as [ФИО],TypePost as [Должность],TypeCor as [Тип корочки], Expiration_date as [Дата окончания] FROM Person;";
             cmd = "SELECT FIO,TypePost,TypeCor,Department, Expiration_date FROM Person;";
             //cmd = "SELECT * FROM Employee";
            //// Вывод изначальный
            // SQLiteDataAdapter connect4 = new SQLiteDataAdapter(cmd, connection);
            // DataSet ds4 = new DataSet();
            // connect4.Fill(ds4);
            // Employeer.ItemsSource = ds4.Tables[0].DefaultView;
            //Вывод по новому
             command = new SQLiteCommand(cmd, connection);
             reader = command.ExecuteReader();
             //create business data
             itemList = new List<Empl>();//var 

             foreach (DbDataRecord record in reader)
             {
                 itemList.Add(new Empl { FIO = (string)record["FIO"], TypeCor = (string)record["TypeCor"], TypePost = (string)record["TypePost"], Department = (string)record["Department"], Expiration_date = String.Format("{0:dd/MM/yyyy }", record["Expiration_date"]) });
             }
             Employeer.ItemsSource = itemList;
             //link business data to CollectionViewSource
             // CollectionViewSource itemCollectionViewSource;
             // itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
             // itemCollectionViewSource.Source = itemList;//</stockitem>
             connection.Close();
             update_clik();
        }

        private void FiltrCor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( FiltrCor.SelectedItem != null)
            {
                connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                connection.Open();
                cmd = String.Format("SELECT FIO ,TypePost ,TypeCor ,Department, Expiration_date  FROM Person WHERE TypeCor={0};", "'" + FiltrCor.SelectedItem + "'");
                if (FiltrCor.SelectedIndex == 0)
                {
                    //cmd = "SELECT FIO as [ФИО],TypePost as [Должность],TypeCor as [Тип корочки], Expiration_date as [Дата окончания] FROM Person;";
                    cmd = "SELECT FIO ,TypePost, Department, TypeCor , Expiration_date FROM Person;";
                }
                command = new SQLiteCommand(cmd, connection);// SQLiteCommand
                reader = command.ExecuteReader();
                itemList = new List<Empl>();
                foreach (DbDataRecord record in reader)
                {
                    itemList.Add(new Empl { FIO = (string)record["FIO"], TypeCor = (string)record["TypeCor"], TypePost = (string)record["TypePost"], Department = (string)record["Department"], Expiration_date = String.Format("{0:dd/MM/yyyy }", record["Expiration_date"]) });
                }
                Employeer.ItemsSource = itemList;
                connection.Close();
            }
            
        }

        private void FiltrPers_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        //private void FiltrPers_TextInput(object sender, TextCompositionEventArgs e)
        //{
        //    for (int i = 0; i < this.FiltrPers.Items.Count; i++)
        //        if (!this.FiltrPers.Items[i].ToString().Contains(this.FiltrPers.Text))
        //        {

        //            this.FiltrPers.Items.RemoveAt(i);
        //            i--;
        //        }
        //}



    }
}
