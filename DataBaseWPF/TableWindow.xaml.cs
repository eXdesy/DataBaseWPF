using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System.Runtime.Intrinsics.X86;
using System.Collections;

namespace DataBaseWPF
{
    public partial class TableWindow : UserControl
    {
        // Conexión a la base de datos
        private readonly MySqlConnection? connection;
        private readonly string tableName;

        public TableWindow(MySqlConnection con, string table)
        {
            InitializeComponent();
            connection = con;
            tableName = table;

            // Método que carga los datos en la tabla al inicializar la ventana
            LoadDataTable();
        }

        private void LoadDataTable()
        {
            try
            {
                // Consulta para obtener todos los datos de la tabla
                string query = $"SELECT * FROM {tableName}";
                MySqlDataAdapter dataAdapter = new(query, connection);

                // Crea un DataSet para almacenar los datos
                DataSet dataSet = new();
                dataAdapter.Fill(dataSet, "DataTable");

                // Enlaza los datos al control DataGrid (o cualquier otro control que prefieras)
                dataGrid.ItemsSource = dataSet.Tables["DataTable"].DefaultView;
            }
            catch (Exception ex)
            {
                // Manejo de errores al cargar datos
                MessageBox.Show($"Error loading data from table {tableName}: {ex.Message}");
            }
        }

        private string[] GetColumnNames()
        {
            // Obtiene los nombres de las columnas de la tabla
            DataTable schemaTable = connection.GetSchema("Columns", new[] { null, null, tableName, null });
            return schemaTable.AsEnumerable().Select(row => row.Field<string>("COLUMN_NAME")).ToArray();
        }


        private void CreateButton(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtén los nombres de las columnas
                string[] columnNames = GetColumnNames();

                // Construye la consulta de inserción
                string insertQuery = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", columnNames.Select(c => $"@{c}"))})";
                MySqlCommand insertCommand = new(insertQuery, connection);

                // Crea una nueva fila en el DataTable
                DataTable dataTable = ((DataView)dataGrid.ItemsSource)?.Table;
                DataRow newRow = dataTable?.NewRow();

                if (dataTable != null && newRow != null)
                {
                    // Obtén la fila seleccionada del DataGrid
                    DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;

                    // Obtén los valores de las celdas del DataGrid
                    foreach (var columnName in columnNames)
                    {
                        // Utiliza el nombre de la columna para obtener el valor de la fila seleccionada
                        if (selectedRow != null && selectedRow.Row.Table.Columns.Contains(columnName))
                        {
                            object cellValue = selectedRow[columnName];
                            newRow[columnName] = cellValue;
                            insertCommand.Parameters.AddWithValue($"@{columnName}", cellValue);
                        }
                    }

                    // Agrega la nueva fila al DataTable
                    dataTable.Rows.Add(newRow);

                    insertCommand.ExecuteNonQuery();

                    // Vuelve a cargar los datos después de la eliminación
                    LoadDataTable();
                }
                else
                {
                    MessageBox.Show("No se pudo acceder al DataTable o la nueva fila.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding basedate: " + ex.Message);
            }
        }
        // Métodos similares para los botones Read, Update, Delete, y otros eventos...
        private void ReadButton(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadDataTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading basedate: " + ex.Message);
            }
        }
        private void UpdateButton(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtén los nombres de las columnas
                string[] columnNames = GetColumnNames();

                // Construye la consulta de actualización
                string updateQuery = $"UPDATE {tableName} SET {string.Join(", ", columnNames.Skip(1).Select(c => $"{c} = @{c}"))} WHERE {columnNames[0]} = @{columnNames[0]}";
                MySqlCommand updateCommand = new(updateQuery, connection);

                // Obtén la fila seleccionada del DataGrid
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;

                if (selectedRow != null)
                {
                    // Configura los parámetros de la consulta de actualización
                    foreach (var columnName in columnNames.Skip(1))
                    {
                        if (selectedRow.Row.Table.Columns.Contains(columnName))
                        {
                            object cellValue = selectedRow[columnName];
                            updateCommand.Parameters.AddWithValue($"@{columnName}", cellValue);
                        }
                    }

                    // Utiliza el valor de la primera columna como identificador único
                    string firstColumnName = columnNames[0];
                    object firstColumnValue = selectedRow[firstColumnName];
                    updateCommand.Parameters.AddWithValue($"@{firstColumnName}", firstColumnValue);

                    // Ejecuta la consulta de actualización
                    updateCommand.ExecuteNonQuery();

                    // Vuelve a cargar los datos después de la eliminación
                    LoadDataTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating basedate: " + ex.Message);
            }
        }
        private void DeleteButton(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtén los nombres de las columnas
                string[] columnNames = GetColumnNames();

                // Construye la consulta de eliminación
                string deleteQuery = $"DELETE FROM {tableName} WHERE {columnNames[0]} = @{columnNames[0]}";
                MySqlCommand deleteCommand = new(deleteQuery, connection);

                // Obtén la fila seleccionada del DataGrid
                DataRowView selectedRow = (DataRowView)dataGrid.SelectedItem;

                if (selectedRow != null)
                {
                    // Utiliza el valor de la primera columna como identificador único
                    string firstColumnName = columnNames[0];
                    object firstColumnValue = selectedRow[firstColumnName];
                    deleteCommand.Parameters.AddWithValue($"@{firstColumnName}", firstColumnValue);

                    // Ejecuta la consulta de eliminación
                    deleteCommand.ExecuteNonQuery();

                    // Vuelve a cargar los datos después de la eliminación
                    LoadDataTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting basedate: " + ex.Message);
            }
        }

        // Método para ejecutar consultas y mostrar resultados en el DataGrid
        private void ExecuteQuery(string CRUD)
        {
            DataTable resultDataTable = new DataTable();
            try
            {
                // Ejecuta la consulta y carga los resultados en el DataTable
                using (MySqlCommand cmd = new MySqlCommand(CRUD, connection))
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    resultDataTable.Load(reader);

                    // Asigna el resultado al DataGrid
                    dataGrid.ItemsSource = resultDataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores al ejecutar consultas
                MessageBox.Show($"Error executing query: {ex.Message}");
            }
        }

        // Evento para el botón "Mostrar 5 productos más vendidos"
        private void FirstButton(object sender, RoutedEventArgs e)
        {
            // Ejecuta una consulta específica al presionar el botón
            ExecuteQuery("SELECT * FROM Products ORDER BY UnitsInStock DESC LIMIT 5");
        }

        // Evento para el botón "Mostrar productos sin stock"
        private void SecondButton(object sender, RoutedEventArgs e)
        {
            // Ejecuta una consulta específica al presionar el botón
            ExecuteQuery("SELECT * FROM Products WHERE UnitsInStock = 0");
        }

        // Evento para el botón "Mostrar 5 productos más caros"
        private void ThirdButton(object sender, RoutedEventArgs e)
        {
            // Ejecuta una consulta específica al presionar el botón
            ExecuteQuery("SELECT * FROM Products ORDER BY UnitPrice DESC LIMIT 5");
        }

        // Evento para hacer consultas
        private void SendButton(object sender, RoutedEventArgs e)
        {
            // Ejecuta la consulta ingresada por el usuario
            ExecuteQuery(query.Text);
        }
    }
}