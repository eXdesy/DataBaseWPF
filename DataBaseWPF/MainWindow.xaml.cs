using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataBaseWPF.database;
using MySql.Data.MySqlClient;
using DataBaseWPF;


namespace DataBaseWPF
{
    public partial class MainWindow : Window
    {
        // Conexión a la base de datos
        private readonly MySqlConnection? connection;

        // Constructor de la clase MainWindow que recibe una conexión a la base de datos
        public MainWindow(MySqlConnection con)
        {
            InitializeComponent();
            // Asigna la conexión recibida al campo de conexión de la clase
            connection = con;
            // Llama al método para cargar las tablas en el menú
            LoadTablesToMenu();
        }

        // Método para cargar dinámicamente los elementos del menú con nombres de tablas
        private void LoadTablesToMenu()
        {
            // Obtiene la lista de nombres de tablas disponibles en la base de datos
            List<string> tableNames = GetTableNames();

            // Crea dinámicamente elementos del menú con los nombres de las tablas
            foreach (string tableName in tableNames)
            {
                MenuItem menuItem = new()
                {
                    Header = tableName,
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI Black"),
                    FontSize = 10,
                    Foreground = System.Windows.Media.Brushes.White,
                    Height = 25
                };
                // Asigna el evento ChangeContentButton al hacer clic en el elemento del menú
                menuItem.Click += ChangeContentButton;
                // Agrega el nuevo elemento al menú
                MainMenu.Items.Add(menuItem);
            }

            // Agrega el elemento "EXIT" al final del menú
            MenuItem exitMenuItem = new MenuItem
            {
                Header = "EXIT",
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI Black"),
                FontSize = 10,
                Foreground = System.Windows.Media.Brushes.White,
                Height = 25
            };
            // Asigna el evento ExitButton al hacer clic en el elemento "EXIT"
            exitMenuItem.Click += ExitButton;
            // Agrega el nuevo elemento al final del menú
            MainMenu.Items.Add(exitMenuItem);
        }

        // Método para obtener la lista de nombres de tablas desde la base de datos
        private List<string> GetTableNames()
        {
            List<string> tableNames = new List<string>();
            try
            {
                // Obtiene la lista de tablas disponibles en la base de datos
                DataTable schema = connection.GetSchema("Tables");
                foreach (DataRow row in schema.Rows)
                {
                    tableNames.Add(row["TABLE_NAME"].ToString());
                }
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si hay un problema al obtener los nombres de las tablas
                MessageBox.Show("Error getting table names: " + ex.Message);
            }
            // Devuelve la lista de nombres de tablas
            return tableNames;
        }

        // Método para cambiar el contenido al hacer clic en un elemento del menú
        private void ChangeContentButton(object sender, RoutedEventArgs e)
        {
            // Verifica si el remitente es un MenuItem
            if (sender is MenuItem menuItem)
            {
                // Obtiene el nombre de la tabla desde el Header del MenuItem
                string tableName = menuItem.Header.ToString();
                // Crea un nuevo UserControl para mostrar el contenido de la tabla seleccionada
                UserControl newContent = new TableWindow(connection, tableName);
                // Limpia el contenido actual y agrega el nuevo UserControl
                Content.Children.Clear();
                Content.Children.Add(newContent);
            }
        }

        // Método para cerrar la conexión a la base de datos y cerrar la ventana actual al hacer clic en "EXIT"
        private void ExitButton(object sender, RoutedEventArgs e)
        {
            // Cierra la conexión a la base de datos
            Sql.Close();
            // Crea una nueva instancia de LoginWindow
            LoginWindow loginWindow = new LoginWindow();
            // Muestra la ventana de inicio de sesión y cierra la ventana actual
            loginWindow.Show();
            this.Close();
        }
    }
}