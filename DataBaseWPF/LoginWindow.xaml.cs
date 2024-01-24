using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataBaseWPF;
using DataBaseWPF.database;

namespace DataBaseWPF
{
    // Declaración de la clase LoginWindow que hereda de la clase Window
    public partial class LoginWindow : Window
    {
        // Constructor de la clase LoginWindow
        public LoginWindow()
        {
            // Inicializa los componentes visuales definidos en XAML (Interfaz de usuario)
            InitializeComponent();
        }

        // Método que maneja el evento Click del botón de inicio de sesión
        private void LoginButton(object sender, RoutedEventArgs e)
        {
            // Verifica la conexión a la base de datos usando la clase Sql y su método Connect
            if (Sql.Connect(DataBase.Text, Username.Text, Password.Password))
            {
                // Si la conexión es exitosa, crea una nueva instancia de la clase MainWindow pasando la conexión como parámetro
                MainWindow mainWindow = new(Sql.con);
                // Muestra la ventana principal (MainWindow)
                mainWindow.Show();
                // Cierra la ventana de inicio de sesión (LoginWindow)
                this.Close();
            }
            else
            {
                // Si la conexión falla, muestra un mensaje de error con un cuadro de diálogo MessageBox
                MessageBox.Show("Username or password is incorrect.");
            }
        }
    }
}