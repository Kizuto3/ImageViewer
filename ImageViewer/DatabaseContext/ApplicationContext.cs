using ImageViewer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace ImageViewer.DatabaseContext
{
    public class ApplicationContext
    {
        /// <summary>
        /// Connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Connection to database
        /// </summary>
        private SQLiteConnection _SQLiteConnection = null;

        /// <summary>
        /// Default costructor
        /// </summary>
        public ApplicationContext()
        {
            _connectionString = "Data Source=master.db";
            CreateDatabase();
        }

        /// <summary>
        /// Open database connection
        /// </summary>
        private void OpenConnection()
        {
            _SQLiteConnection = new SQLiteConnection
            {
                ConnectionString = _connectionString
            };
            _SQLiteConnection.Open();
        }

        /// <summary>
        /// Close database connection
        /// </summary>
        private void CloseConnection()
        {
            if(_SQLiteConnection?.State != ConnectionState.Closed)
            {
                _SQLiteConnection?.Close();
            }
        }

        /// <summary>
        /// Create database file if it`s not exist
        /// </summary>
        public void CreateDatabase()
        {
            if (File.Exists("master.db")) return;

            SQLiteConnection.CreateFile("master.db");
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                var createImageModelsTable = "CREATE TABLE \"ImageModels\" (" +
                                             "\"ID\"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
	                                         "\"Fullpath\"  TEXT," +
	                                         "\"ScaleX\"    FLOAT," +
	                                         "\"ScaleY\"    FLOAT," +
	                                         "\"Angle\" FLOAT)";

                var command = new SQLiteCommand(createImageModelsTable, connection);
                command.ExecuteNonQuery();

                var createPageModelsTable = "CREATE TABLE \"PageModels\" (" +
                                            "\"IsListVisible\" BOOLEAN," +
	                                        "\"IsEditBarVisible\"  BOOLEAN," +
	                                        "\"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
	                                        "\"ImageModelID\"  INTEGER," +
	                                        "FOREIGN KEY(\"ImageModelID\") REFERENCES \"ImageModels\"(\"ID\"))";

                command = new SQLiteCommand(createPageModelsTable, connection);
                command.ExecuteNonQuery();

                var createWindowModelsTable = "CREATE TABLE \"WindowModels\" (" +
                                              "\"Left\"  FLOAT," +
	                                          "\"Top\"   FLOAT," +
	                                          "\"Width\" FLOAT," +
	                                          "\"Height\"    FLOAT," +
	                                          "\"State\" INTEGER," +
	                                          "\"ID\"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE)";

                command = new SQLiteCommand(createWindowModelsTable, connection);
                command.ExecuteNonQuery();

                var createEditModelsTable = "CREATE TABLE \"EditModels\" (" +
                                            "\"ID\"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                                            "\"Path\"  TEXT," +
                                            "\"ImageModelID\"  INTEGER," +
                                            "FOREIGN KEY(\"ImageModelID\") REFERENCES \"ImageModels\"(\"ID\"))";

                command = new SQLiteCommand(createEditModelsTable, connection);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Get all <see cref="ImageModel"/> from database
        /// </summary>
        /// <returns></returns>
        public List<ImageModel> GetImageModels()
        {
            OpenConnection();

            List<ImageModel> imageModels = new List<ImageModel>();

            const string query = "SELECT * FROM [ImageModels]";

            using(var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                command.CommandType = CommandType.Text;
                SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    imageModels.Add(new ImageModel
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        FullPath = Convert.ToString(reader["Fullpath"]),
                        ScaleX = Convert.ToDouble(reader["ScaleX"]),
                        ScaleY = Convert.ToDouble(reader["ScaleY"]),
                        Angle = Convert.ToDouble(reader["Angle"])
                    });
                }
                reader.Close();
            }

            return imageModels;
        }

        /// <summary>
        /// Get all <see cref="PageModel"/> from database
        /// </summary>
        /// <returns></returns>
        public List<PageModel> GetPageModels()
        {
            OpenConnection();

            List<PageModel> pageModels = new List<PageModel>();

            const string query = "SELECT * FROM [PageModels]";

            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                command.CommandType = CommandType.Text;
                SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    pageModels.Add(new PageModel
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        ImageModelID = Convert.ToInt32(reader["ImageModelID"]),
                        IsListVisible = Convert.ToBoolean(reader["IsListVisible"]),
                        IsEditBarVisible = Convert.ToBoolean(reader["IsEditBarVisible"])
                    });
                }
                reader.Close();
            }

            return pageModels;
        }

        /// <summary>
        /// Get all <see cref="WindowModel"/> from database
        /// </summary>
        /// <returns></returns>
        public List<WindowModel> GetWindowModels()
        {
            OpenConnection();

            List<WindowModel> windowModels = new List<WindowModel>();

            const string query = "SELECT * FROM [WindowModels]";

            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    windowModels.Add(new WindowModel
                    {
                        Left = Convert.ToDouble(reader["Left"]),
                        Top = Convert.ToDouble(reader["Top"]),
                        Width = Convert.ToDouble(reader["Width"]),
                        Height = Convert.ToDouble(reader["Height"]),
                        State = Convert.ToInt32(reader["State"]),
                        ID = Convert.ToInt32(reader["ID"])
                    });
                }
                reader.Close();
            }

            return windowModels;
        }

        /// <summary>
        /// Get all <see cref="EditModel"/> from database
        /// </summary>
        /// <returns></returns>
        public List<EditModel> GetEditModels()
        {
            OpenConnection();

            List<EditModel> editModels = new List<EditModel>();

            const string query = "SELECT * FROM [EditModels]";

            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                command.CommandType = CommandType.Text;
                SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    editModels.Add(new EditModel
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Path = Convert.ToString(reader["Path"]),
                        ImageModelID = Convert.ToInt32(reader["ImageModelID"]),
                    });
                }
                reader.Close();
            }

            return editModels;
        }

        /// <summary>
        /// Insert <see cref="ImageModel"/> into database using parametrized query
        /// </summary>
        /// <param name="imageModel"></param>
        public void InsertImageModel(ImageModel imageModel)
        {
            OpenConnection();

            var query = "INSERT INTO [ImageModels] ([Fullpath], [ScaleX], [ScaleY], [Angle])" + 
                                " VALUES (@Fullpath, @ScaleX, @ScaleY, @Angle)";
            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                var parameter = new SQLiteParameter
                {
                    ParameterName = "@Fullpath",
                    Value = imageModel.FullPath,
                    DbType = DbType.String
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@ScaleX",
                    Value = imageModel.ScaleX,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@ScaleY",
                    Value = imageModel.ScaleY,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@Angle",
                    Value = imageModel.Angle,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        /// <summary>
        /// Insert <see cref="WindowModel"/> into database using parametrized query
        /// </summary>
        /// <param name="windowModel"></param>
        public void InsertWindowModel(WindowModel windowModel)
        {
            OpenConnection();

            var query = "INSERT INTO [WindowModels] ([Left], [Top], [Width], [Height], [State])" +
                                " VALUES (@Left, @Top, @Width, @Height, @State)";
            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                var parameter = new SQLiteParameter
                {
                    ParameterName = "@Left",
                    Value = windowModel.ID,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@Top",
                    Value = windowModel.Top,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@Width",
                    Value = windowModel.Width,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@Height",
                    Value = windowModel.Height,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@State",
                    Value = windowModel.State,
                    DbType = DbType.Int16
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        /// <summary>
        /// Insert <see cref="PageModel"/> into database using parametrized query
        /// </summary>
        /// <param name="pageModel"></param>
        public void InsertPageModel(PageModel pageModel)
        {
            OpenConnection();

            var query = "INSERT INTO [PageModels] ([ImageModelID], [IsListVisible], [IsEditBarVisible])" +
                                " VALUES (@ImageModelID, @IsListVisible, @IsEditBarVisible)";
            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                var parameter = new SQLiteParameter
                {
                    ParameterName = "@ImageModelID",
                    Value = pageModel.ImageModelID,
                    DbType = DbType.Int32
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@IsListVisible",
                    Value = pageModel.IsListVisible,
                    DbType = DbType.Boolean
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@IsEditBarVisible",
                    Value = pageModel.IsEditBarVisible,
                    DbType = DbType.Boolean
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        /// <summary>
        /// Insert <see cref="EditModel"/> into database using parametrized query
        /// </summary>
        /// <param name="editModel"></param>
        public void InsertEditModel(EditModel editModel)
        {
            OpenConnection();

            var query = "INSERT INTO [EditModels] ([ImageModelID], [Path])" +
                                " VALUES (@ImageModelID, @Path)";
            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                var parameter = new SQLiteParameter
                {
                    ParameterName = "@ImageModelID",
                    Value = editModel.ImageModelID,
                    DbType = DbType.Int32
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@Path",
                    Value = editModel.Path,
                    DbType = DbType.String
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        /// <summary>
        /// Update <see cref="ImageModel"/> by it`s ID
        /// </summary>
        /// <param name="imageModel"></param>
        public void UpdateImageModel(ImageModel imageModel)
        {
            OpenConnection();

            var query = "UPDATE [imageModels] " +
                        "SET [ScaleX] = @ScaleX, [ScaleY] = @ScaleY, [Angle] = @Angle " +
                        "WHERE [ID] = @ID";
            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                var parameter = new SQLiteParameter
                {
                    ParameterName = "@ScaleX",
                    Value = imageModel.ScaleX,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@ScaleY",
                    Value = imageModel.ScaleY,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@Angle",
                    Value = imageModel.Angle,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@ID",
                    Value = imageModel.ID,
                    DbType = DbType.Int32
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        /// <summary>
        /// Update <see cref="PageModel"/> by it`s ID
        /// </summary>
        /// <param name="pageModel"></param>
        public void UpdatePageModel(PageModel pageModel)
        {
            OpenConnection();

            var query = "UPDATE [PageModels] " +
                        "SET [IsListVisible] = @IsListVisible, [IsEditBarVisible] = @IsEditBarVisible, [ImageModelID] = @ImageModelID " +
                        "WHERE [ID] = @ID";
            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                var parameter = new SQLiteParameter
                {
                    ParameterName = "@ImageModelID",
                    Value = pageModel.ImageModelID,
                    DbType = DbType.Int32
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@IsListVisible",
                    Value = pageModel.IsListVisible,
                    DbType = DbType.Boolean
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@IsEditBarVisible",
                    Value = pageModel.IsEditBarVisible,
                    DbType = DbType.Boolean
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@ID",
                    Value = pageModel.ID,
                    DbType = DbType.Int32
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        /// <summary>
        /// Update <see cref="WindowModel"/> by it`s ID
        /// </summary>
        /// <param name="windowModel"></param>
        public void UpdateWindowModel(WindowModel windowModel)
        {
            OpenConnection();

            var query = "UPDATE [WindowModels] " +
                        "SET [Left] = @Left, [Top] = @Top, [Width] = @Width, [Height] = @Height, [State] = @State " +
                        "WHERE [ID] = @ID";
            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                var parameter = new SQLiteParameter
                {
                    ParameterName = "@Left",
                    Value = windowModel.Left,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@Top",
                    Value = windowModel.Top,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@Width",
                    Value = windowModel.Width,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@Height",
                    Value = windowModel.Height,
                    DbType = DbType.Double
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@State",
                    Value = windowModel.State,
                    DbType = DbType.Int16
                };
                command.Parameters.Add(parameter);

                parameter = new SQLiteParameter
                {
                    ParameterName = "@ID",
                    Value = windowModel.ID,
                    DbType = DbType.Int32
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        /// <summary>
        /// Remove <see cref="ImageModel"/> from database by ID
        /// </summary>
        /// <param name="imageModelID"></param>
        public void RemoveImageModel(int imageModelID)
        {
            OpenConnection();

            var query = "DELETE FROM [ImageModels] " +
                        "WHERE [ID] = @ID";
            using (var command = new SQLiteCommand(query, _SQLiteConnection))
            {
                var parameter = new SQLiteParameter
                {
                    ParameterName = "@ID",
                    Value = imageModelID,
                    DbType = DbType.Int32
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

            CloseConnection();
        }
    }
}
