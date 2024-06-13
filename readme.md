# WPF Interface BaseDate

Este proyecto consiste en una aplicación de escritorio desarrollada en C# con WPF (Windows Presentation Foundation). La aplicación proporciona una interfaz gráfica para interactuar con una base de datos MySQL. Permite realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) en las tablas de la base de datos, así como ejecutar consultas personalizadas.

## 1. **LoginWindow**
Representan la ventana de inicio de sesión de la aplicación. Al ingresar las credenciales correctas, se establece una conexión a la base de datos y se abre la ventana principal (`MainWindow`).

![LoginWindow](https://github.com/eXdesy/DataBaseWPF/blob/master/img/LoginWindow.png)

## 2. **MainWindow**
Representan la ventana principal de la aplicación. Muestra un menú con las tablas disponibles en la base de datos y permite navegar entre ellas. También proporciona la funcionalidad para cerrar la conexión a la base de datos.

![MainWindow](https://github.com/eXdesy/DataBaseWPF/blob/master/img/MainWindow.png)

## 3. **TableWindow**
Representan la ventana que muestra los datos de una tabla específica de la base de datos. Permite realizar operaciones CRUD en los datos y ejecutar consultas personalizadas.

![TableWindow](https://github.com/eXdesy/DataBaseWPF/blob/master/img/TableWindow.png)
![TableWindow2](https://github.com/eXdesy/DataBaseWPF/blob/master/img/TableWindow2.png)

## Uso de la Aplicación
1. Al iniciar la aplicación, se muestra la ventana de inicio de sesión (`LoginWindow`). Ingresa las credenciales de la base de datos y presiona el botón "Enter" para establecer la conexión.
2. Una vez conectado, se abrirá la ventana principal (`MainWindow`), que muestra un menú con las tablas disponibles. Selecciona una tabla para ver y manipular sus datos.
3. En la ventana de tabla (`TableWindow`), puedes realizar operaciones CRUD en los datos de la tabla seleccionada. También hay botones predefinidos para ejecutar consultas específicas, como mostrar los productos más vendidos.
4. Para cerrar la sesión y volver a la ventana de inicio de sesión, puedes presionar el botón "EXIT" en la ventana principal (`MainWindow`).

## Requisitos
- MySQL Server instalado y en ejecución.
- Conexión a una base de datos válida con las credenciales adecuadas.

## Instalacion y Ejecucion
- Descargar repositorio.
- Configurar el servidor MySQL de la siguiente manera:
    - Método de conexión: `Estándar (TCP/IP)`
    - Nombre de host: `127.0.0.1`
    - Puerto: `3306`
    - Nombre de usuario: A su discreción.
    - Contraseña: A su discreción.
- Iniciar la aplicación en Visual Studio.
- Iniciar sesión.

## Dependencias
- La aplicación utiliza la biblioteca `MySql.Data` para la conexión y manipulación de datos en la base de datos MySQL.

## Notas
- Asegúrate de tener una conexión activa a la base de datos antes de realizar operaciones CRUD o consultas.
- La aplicación proporciona una interfaz gráfica simple para interactuar con la base de datos, pero es necesario entender la estructura de la base de datos subyacente para utilizarla de manera efectiva.

## Autores
Creado y desarrollado por DILYORBEK MUKHIDDINOV y ERIC QUILES SILGADO para el proyecto de la segunda evaluación del curso Desarrollo de Aplicaciones Multiplataformas en el Instituto Santa Joaquin de Vedruna durante el curso 2023/2024. Todos los derechos reservados.

**¡Disfruta usando la aplicación de gestión de base de datos MySQL con interfaz WPF!**
