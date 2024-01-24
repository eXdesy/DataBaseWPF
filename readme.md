# WPF Interface BaseDate

## Descripción

Este proyecto consiste en una aplicación de escritorio desarrollada en C# con WPF (Windows Presentation Foundation). La aplicación proporciona una interfaz gráfica para interactuar con una base de datos MySQL. Permite realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) en las tablas de la base de datos, así como ejecutar consultas personalizadas.

## Estructura del Proyecto

### 1. **Sql.cs**
Contiene la clase `Sql` que gestiona la conexión y cierre de la conexión a la base de datos MySQL.

```
public static Boolean Connect(string database, string user, string password)
{
    if (!isConnected)
    {
        con = new MySqlConnection("server=localhost;port=3306;database=" + database + ";user=" + user + ";password=" + password);
        try
        {
            // La conexión está abierta
            con.Open();
            isConnected = true;
            Console.WriteLine("Connection established");
        }
        catch (SqlException e)
        {
            throw new ApplicationException("Error establishing connection: " + e.Message);
        }
    }
    return true;
}
```

```
public static void Close()
{
    if (isConnected && con != null)
    {
        try
        {
            // La conexión está cerrada
            con.Close();
            isConnected = false;
            Console.WriteLine("Connection closed");
        }
        catch (SqlException e)
        {
            throw new ApplicationException("Error closing connection: " + e.Message);
        }
    }
}
```

### 2. **LoginWindow.xaml.cs** y **LoginWindow.xaml**
Representan la ventana de inicio de sesión de la aplicación. Al ingresar las credenciales correctas, se establece una conexión a la base de datos y se abre la ventana principal (`MainWindow`).

![LoginWindow](https://github.com/eXdesy/DataBaseWPF/blob/master/img/LoginWindow.png)

Método que maneja el evento Click del botón de inicio de sesión
```
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
```

### 3. **MainWindow.xaml.cs** y **MainWindow.xaml**
Representan la ventana principal de la aplicación. Muestra un menú con las tablas disponibles en la base de datos y permite navegar entre ellas. También proporciona la funcionalidad para cerrar la conexión a la base de datos.

![MainWindow](https://github.com/eXdesy/DataBaseWPF/blob/master/img/MainWindow.png)

// Conexión a la base de datos
```
private readonly MySqlConnection? connection;
```

Método para cargar dinámicamente los elementos del menú con nombres de tablas
```
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
```

Método para obtener la lista de nombres de tablas desde la base de datos
```
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
```

Método para cambiar el contenido al hacer clic en un elemento del menú
```
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
```

Método para cerrar la conexión a la base de datos y cerrar la ventana actual al hacer clic en "EXIT"
```
private void ExitButton(object sender, RoutedEventArgs e)
    {
        // Cierra la conexión a la base de datos
        connection.Close();
        // Crea una nueva instancia de LoginWindow
        LoginWindow loginWindow = new LoginWindow();
        // Muestra la ventana de inicio de sesión y cierra la ventana actual
        loginWindow.Show();
        this.Close();
    }
```

### 4. **TableWindow.xaml.cs** y **TableWindow.xaml**
Representan la ventana que muestra los datos de una tabla específica de la base de datos. Permite realizar operaciones CRUD en los datos y ejecutar consultas personalizadas.

![TableWindow](https://github.com/eXdesy/DataBaseWPF/blob/master/img/TableWindow.png)
![TableWindow2](https://github.com/eXdesy/DataBaseWPF/blob/master/img/TableWindow2.png)

Metodo para cargar los datos de la tabla
```
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
```

Metodo para obeten los nombres de las tablas
```
private string[] GetColumnNames()
{
    // Obtiene los nombres de las columnas de la tabla
    DataTable schemaTable = connection.GetSchema("Columns", new[] { null, null, tableName, null });
    return schemaTable.AsEnumerable().Select(row => row.Field<string>("COLUMN_NAME")).ToArray();
}
```

Metodo para crear nuevo fila en base de datos. CRUD 
```
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
```

Métodos similares para los botones Read, Update, Delete, y otros eventos...
```
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
```
```
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
```
```
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
```

Método para ejecutar consultas y mostrar resultados en el DataGrid
```
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
```

Evento para los botón de hacer consultas
```
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
```

## Uso de la Aplicación
1. Al iniciar la aplicación, se muestra la ventana de inicio de sesión (`LoginWindow`). Ingresa las credenciales de la base de datos y presiona el botón "Enter" para establecer la conexión.
2. Una vez conectado, se abrirá la ventana principal (`MainWindow`), que muestra un menú con las tablas disponibles. Selecciona una tabla para ver y manipular sus datos.
3. En la ventana de tabla (`TableWindow`), puedes realizar operaciones CRUD en los datos de la tabla seleccionada. También hay botones predefinidos para ejecutar consultas específicas, como mostrar los productos más vendidos.
4. Para cerrar la sesión y volver a la ventana de inicio de sesión, puedes presionar el botón "EXIT" en la ventana principal (`MainWindow`).

## Requisitos
- MySQL Server instalado y en ejecución.
- Conexión a una base de datos válida con las credenciales adecuadas.

## Dependencias
- La aplicación utiliza la biblioteca `MySql.Data` para la conexión y manipulación de datos en la base de datos MySQL.

## Notas
- Asegúrate de tener una conexión activa a la base de datos antes de realizar operaciones CRUD o consultas.
- La aplicación proporciona una interfaz gráfica simple para interactuar con la base de datos, pero es necesario entender la estructura de la base de datos subyacente para utilizarla de manera efectiva.

## Autores
Creado y desarrollado por DILYORBEK MUKHIDDINOV y ERIC QUILES SILGADO para el proyecto de la segunda evaluación del curso Desarrollo de Aplicaciones Multiplataformas en el Instituto Santa Joaquin de Vedruna durante el curso 2023/2024. Todos los derechos reservados.

**¡Disfruta usando la aplicación de gestión de base de datos MySQL con interfaz WPF!**
