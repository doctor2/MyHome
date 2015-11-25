using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite.Linq;
using System.Data.SQLite;
using System.Data.Common;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;



namespace Employees1
{
    public partial class MainWindow : Window
    {
        private void OnDataGridPrinting(object sender, RoutedEventArgs e)
        {
            //System.Windows.Controls.PrintDialog Printdlg = new System.Windows.Controls.PrintDialog();
            //System.Windows.Controls.PrintDialog printdlg = new System.Windows.Controls.PrintDialog();
            //GridPrintDocument doc = new GridPrintDocument(this.dataGridView1,
            var pd = new PrintDialog();

            // calculate page size
            var sz = new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight);

            // create paginator
         //   var paginator = new FlexPaginator(_flex, ScaleMode.PageWidth, sz, new Thickness(96 / 4), 100);
            string tempFileName = System.IO.Path.GetTempFileName();

            File.Delete(tempFileName);

            using (XpsDocument xpsDocument = new XpsDocument(tempFileName, FileAccess.ReadWrite))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                writer.Write(System.IO.Path.GetTempFileName());
                DocumentViewer previewWindow = new DocumentViewer
                {
                    Document = xpsDocument.GetFixedDocumentSequence()
                };

                Window printpriview = new Window();
                printpriview.Content = previewWindow;
                printpriview.Title = "C1FlexGrid: Print Preview";
                printpriview.Show();
            }
            //if ((bool)printdlg.ShowDialog().GetValueOrDefault())
            //{
            //    printdlg.PrintVisual(Employeer, "Печатаем грид");
            //}
            //if ((bool)Printdlg.ShowDialog().GetValueOrDefault())
            //{
            //    Size pageSize = new Size(Printdlg.PrintableAreaWidth, Printdlg.PrintableAreaHeight);
            //    // sizing of the element.
            //    Employeer.Measure(pageSize);
            //    Employeer.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
            //    Printdlg.PrintVisual(Employeer, Title);
            //}
        }
        private void FiltrPers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FiltrPers.SelectedItem != null)
            {
                connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                connection.Open();
                cmd = String.Format("SELECT FIO ,TypePost ,TypeCor ,Department, Expiration_date  FROM Person WHERE FIO={0};", "'" + FiltrPers.SelectedItem + "'");
                if (FiltrPers.SelectedIndex == 0)
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

                //SQLiteDataAdapter connect4 = new SQLiteDataAdapter(cmd, connection);
                //DataSet ds4 = new DataSet();
                //connect4.Fill(ds4);
                //Employeer.ItemsSource = ds4.Tables[0].DefaultView;
                //   Filtr.SelectedValue
                connection.Close();
            }
        }
        public static int DaysDiff(DateTime date1, DateTime date2)
        {
            return date1.Subtract(date2.Date).Days;
        }
        private void Employeer_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            int dat = 0;
            Empl item = e.Row.Item as Empl;
            if (item != null)
            {
                dat = DaysDiff( Convert.ToDateTime(item.Expiration_date), DateTime.Today);
                if (dat <= 10)
                {
                    e.Row.Background = System.Windows.Media.Brushes.Coral;
                }
                else
                {
                  //  e.Row.Background = System.Windows.Media.Brushes.White;
                }
            }
        }
 
        private void DeleteCor_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы дествительно хотите удалить типы корочек и сотрудников, у которых они есть", "Вот в чем вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("DELETE FROM Person WHERE TypeCor={0};", "'" + (string)AddCor.Text + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddPerson.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Удаление прошло успешно");
                }
                catch (Exception)
                {
                    MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
                }
                
            }
        }
        private void EditCorochka_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (ChekCor.SelectedIndex == 0)
                {
                    MessageBox.Show("Редактируемый тип не выбран");
                }
                else
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("UPDATE Person SET TypeCor={0} WHERE TypeCor={1}", "'" + (string)AddCor.Text + "'", "'" + ChekCor.SelectedItem.ToString() + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddPerson.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Успешно");
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
            }
            
            
        }

        private void DeletePost_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы дествительно хотите удалить должность и сотрудников, работающих на ней", "Вот в чем вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("DELETE FROM Person WHERE TypePost={0};", "'" + (string)AddPos.Text + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddPerson.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Удаление прошло успешно");
                }
                catch (Exception)
                {
                    MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
                }
                
            }
        }
        private void EditPost_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (ChekPos.SelectedIndex == 0)
                {
                    MessageBox.Show("Редактируемая должность не выбрана");
                }
                else
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("UPDATE Person SET TypePost={0} WHERE TypePost={1}", "'" + (string)AddPos.Text + "'", "'" + ChekPos.SelectedItem.ToString() + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddPerson.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Успешно");
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
            }
            
            
        }
        private void DeletePerson_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы дествительно хотите удалить сотрудника из базы", "Вот в чем вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("DELETE FROM Person WHERE FIO={0};", "'" + (string)AddPerson.Text + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddPerson.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Удаление прошло успешно");
                }
                catch (Exception)
                {
                    MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
                }
                
            }
        }
        private void EditPerson_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (ChekPerson.SelectedIndex==0)
                {
                    MessageBox.Show("Редактируемое имя не выбрано");
                }
                else
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("UPDATE Person SET FIO={0} WHERE FIO={1}", "'" + (string)AddPerson.Text + "'", "'" + ChekPerson.SelectedItem.ToString() + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddPerson.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Успешно");
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
            }
        }
        private void DeleteDepart_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы дествительно хотите удалить отдел и его сотрудников", "Вот в чем вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("DELETE FROM Person WHERE Department={0};", "'" + (string)AddDepart.Text + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddDepart.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Удаление прошло успешно");
                }
                catch (Exception)
                {
                    MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
                }
            }
        }
        private void EditDepart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ChekDepart.SelectedIndex == 0)
                {
                    MessageBox.Show("Редактируемый отдел не выбран");
                }
                else
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("UPDATE Person SET Department={0} WHERE Department={1}", "'" + (string)AddDepart.Text + "'", "'" + ChekDepart.SelectedItem.ToString() + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddPerson.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Успешно");
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
            }
        }
        private void DeleteDate_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы дествительно хотите удалить записи с выбранной датой", "Вот в чем вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("DELETE FROM Person WHERE Expiration_date={0};", "'" + String.Format("{0:yyyy-MM-dd}", SelectDate.SelectedDate) + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddDepart.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Удаление прошло успешно");
                }
                catch (Exception)
                {
                    MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
                }
            }
        }
        private void EditDate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ChekDate.SelectedIndex == 0)
                {
                    MessageBox.Show("Редактируемая дата не выбрана");
                }
                else
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("UPDATE Person SET Expiration_date={0} WHERE Expiration_date={1}", "'" + String.Format("{0:yyyy-MM-dd}", SelectDate.SelectedDate) + "'", "'" + String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(ChekDate.SelectedItem)) + "'");
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    AddPerson.Text = "";
                    connection.Close();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Успешно");
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Имя введено не правильно или отсутствует в базе!");
            }
        }
        private void AddEmployeer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectDate.SelectedDate!=null)
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    //cmd = String.Format("Select COUNT(*) as [ABC] Person(FIO, TypePost, TypeCor, Expiration_date) VALUES ({0},{1},{2},{3}); ", "'" + (string)AddPerson.Text + "'",
                    //    "'" + (string)AddPos.Text + "'", "'" + (string)AddCor.Text + "'", "'" + String.Format("{0:yyyy-MM-dd}", SelectDate.SelectedDate) + "'");
                    //command = new SQLiteCommand(cmd, connection);
                    //reader = command.ExecuteReader();
                    {
                        cmd = String.Format("INSERT INTO Person(FIO, TypePost,Department, TypeCor, Expiration_date) VALUES ({0},{1},{2},{3},{4}); ", "'" + (string)AddPerson.Text + "'",
                        "'" + (string)AddPos.Text + "'", "'" + AddDepart.Text + "'", "'" + (string)AddCor.Text + "'", "'" + String.Format("{0:yyyy-MM-dd}", SelectDate.SelectedDate) + "'"); //String.Format("{0:dd/MM/yyyy }",//"'" + Calend.SelectedDate.ToString() + "'" ,"'"+ SelectDate.SelectedDate.ToString()+"'"
                        command = new SQLiteCommand(cmd, connection);
                        reader = command.ExecuteReader();
                        connection.Close();
                        //cmd = String.Format("INSERT INTO Employee(Id_Person,Id_Post, Id_Corochka,Expiration_date) VALUES ((SELECT MAX(Id_pers) FROM Person),{0},{1},'2016-09-15');", 2); //ChekPost.SelectedIndex, ChekCorochka.SelectedIndex);
                        //command = new SQLiteCommand(cmd, connection);
                        //reader = command.ExecuteReader();
                        Page_Loaded(sender, e);
                        MessageBox.Show("Сотрудник добавлен");
                    }
                    
                    
                }
                else
                {
                    MessageBox.Show("Дата не выбрана");
                }
                
                
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");
               
            }
            
        }
        private void DeleteEmployeer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectDate.SelectedDate != null)
                {
                    connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
                    connection.Open();
                    cmd = String.Format("DELETE FROM Person WHERE FIO={0} AND TypePost={1} AND TypeCor={2} AND Expiration_date={3} AND Department={4}; ", "'" + (string)AddPerson.Text + "'",
                        "'" + (string)AddPos.Text + "'", "'" + (string)AddCor.Text + "'", "'" + String.Format("{0:yyyy-MM-dd}", SelectDate.SelectedDate) + "'", "'" + AddDepart.Text + "'"); //String.Format("{0:dd/MM/yyyy }",//"'" + Calend.SelectedDate.ToString() + "'" ,"'"+ SelectDate.SelectedDate.ToString()+"'"
                    command = new SQLiteCommand(cmd, connection);
                    reader = command.ExecuteReader();
                    connection.Close();
                    //cmd = String.Format("INSERT INTO Employee(Id_Person,Id_Post, Id_Corochka,Expiration_date) VALUES ((SELECT MAX(Id_pers) FROM Person),{0},{1},'2016-09-15');", 2); //ChekPost.SelectedIndex, ChekCorochka.SelectedIndex);
                    //command = new SQLiteCommand(cmd, connection);
                    //reader = command.ExecuteReader();
                    Page_Loaded(sender, e);
                    MessageBox.Show("Сотрудник Удален");
                }
                else
                {
                    MessageBox.Show("Дата не выбрана");
                }


            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка");

            }
        }
        //private void ChekCor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (ChekCor.SelectedIndex != 0)
        //    {
        //        AddCor.Text = ChekCor.SelectedItem.ToString();
        //    }
        //}
        
        private void ChekCor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ChekCor.SelectedIndex != 0) && (ChekCor.SelectedItem != null))
            {
                AddCor.Text = ChekCor.SelectedItem.ToString();
            }
        }
        private void ChekPerson_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChekPerson.SelectedIndex != 0 && ChekPerson.SelectedItem != null)
            {
                AddPerson.Text = ChekPerson.SelectedItem.ToString();
            }

        }
        private void ChekPos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChekPos.SelectedIndex != 0 && ChekPos.SelectedItem != null)
            {
                AddPos.Text = ChekPos.SelectedItem.ToString();
            }

        }
        private void ChekDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChekDate.SelectedIndex != 0 && ChekDate.SelectedItem != null)
            {
                SelectDate.SelectedDate = Convert.ToDateTime(ChekDate.SelectedItem);
                //AddCor.Text = String.Format("{0:yyyy-MM-dd}", SelectDate.SelectedDate);
            }
        }
        private void ChekDepart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChekDepart.SelectedIndex != 0 && ChekDepart.SelectedItem != null)
            {
                AddDepart.Text = ChekDepart.SelectedItem.ToString();
            }
        }
        //Обноволение выпадающих окон
        private void update_clik()
        {
            connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));//string.Format("Data Source={0}; Version=3;",
            connection.Open();
            //Добавляются данные в выпадающие списки
            cmd = "SELECT distinct TypeCor FROM Person";
            command = new SQLiteCommand(cmd, connection);
            reader = command.ExecuteReader();
            FiltrCor.Items.Clear();
            ChekCor.Items.Clear();
            FiltrCor.Items.Add("Все");
            ChekCor.Items.Add("Не выбраны");
            foreach (DbDataRecord record in reader)
            {
                FiltrCor.Items.Add((string)record["TypeCor"]);
                ChekCor.Items.Add((string)record["TypeCor"]);
            }
            cmd = "SELECT distinct TypePost FROM Person";
            command = new SQLiteCommand(cmd, connection);
            reader = command.ExecuteReader();
            ChekPos.Items.Clear();
            ChekPos.Items.Add("Не выбрана");
            foreach (DbDataRecord record in reader)
            {
                ChekPos.Items.Add((string)record["TypePost"]);
            }
            cmd = "SELECT distinct FIO FROM Person";
            command = new SQLiteCommand(cmd, connection);
            reader = command.ExecuteReader();
            ChekPerson.Items.Clear();
            ChekPerson.Items.Add("Человек не выбран");
            foreach (DbDataRecord record in reader)
            {
                ChekPerson.Items.Add((string)record["FIO"]);
            }
            cmd = "SELECT distinct Expiration_date FROM Person";
            command = new SQLiteCommand(cmd, connection);
            reader = command.ExecuteReader();
            ChekDate.Items.Clear();
            ChekDate.Items.Add("Не выбрана");
            foreach (DbDataRecord record in reader)
            {
                ChekDate.Items.Add(String.Format("{0:dd/MM/yyyy }", record["Expiration_date"]));//record["Expiration_date"].ToString());
            }
            cmd = "SELECT distinct Department FROM Person";
            command = new SQLiteCommand(cmd, connection);
            reader = command.ExecuteReader();
            ChekDepart.Items.Clear();
            ChekDepart.Items.Add("Не выбран");
            foreach (DbDataRecord record in reader)
            {
                ChekDepart.Items.Add((string)record["Department"]);
            }
            cmd = "SELECT distinct FIO FROM Person";
            command = new SQLiteCommand(cmd, connection);
            reader = command.ExecuteReader();
            FiltrPers.Items.Clear();
            FiltrPers.Items.Add("Человек не выбран");
            foreach (DbDataRecord record in reader)
            {
                FiltrPers.Items.Add((string)record["FIO"]);
            }
            connection.Close();
            FiltrCor.SelectedIndex = 0;
            ChekPerson.SelectedIndex = 0;
            ChekPos.SelectedIndex = 0;
            ChekCor.SelectedIndex = 0;
            ChekDate.SelectedIndex = 0;
            ChekDepart.SelectedIndex = 0;
            FiltrPers.SelectedIndex = 0;
        }
    }
}
