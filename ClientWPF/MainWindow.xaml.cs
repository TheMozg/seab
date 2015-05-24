using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core;
using System.Collections.ObjectModel;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkClient client = new NetworkClient();
        public ObservableCollection<Contact> currentDataList { get; set; }
        public MainWindow()
        {
            DataContext = this;
            currentDataList = new ObservableCollection<Contact>();
            //client.BaseAdress = "http://localhost:8080/";
            var b = new LangSel();
            b.ShowDialog();

            if(b.cb.Text == "Русский")
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");

            var a = new ServerSel();
            a.ShowDialog();
            client.BaseAdress = a.tb.Text;

            InitializeComponent();
        }
        private void RefreshCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<Contact> cl = client.getAllContacts();
            currentDataList.Clear();
            foreach (Contact c in cl)
                currentDataList.Add(c);
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            tbEmail.Clear();
            tbName.Clear();
            tbPhone.Clear();
            tbSurname.Clear();
            tbEmail.IsEnabled = true;
            tbName.IsEnabled = true;
            tbPhone.IsEnabled = true;
            tbSurname.IsEnabled = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Contact contact = new Contact();
            contact.name = tbName.Text;
            contact.mail = tbEmail.Text;
            contact.surname = tbSurname.Text;
            contact.number = tbPhone.Text;
            client.postContact(contact);
            NavigationCommands.Refresh.Execute(null, null);
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tbName.Text != "" && tbSurname.Text != "");
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (tbEmail.Text != "" || tbName.Text != "" || tbSurname.Text != "" || tbPhone.Text != "");
        }
    }
}
