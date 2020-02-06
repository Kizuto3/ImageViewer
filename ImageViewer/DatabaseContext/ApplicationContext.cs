using Dapper;
using ImageViewer.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace ImageViewer.DatabaseContext
{
    public class ApplicationContext
    {
        #region Private Members

        /// <summary>
        /// Connection string
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Connection to database
        /// </summary>
        private SQLiteConnection _SQLiteConnection = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Default costructor
        /// </summary>
        public ApplicationContext()
        {
            _connectionString = "Data Source=master.db";
            CreateDatabase();
        }

        #endregion

        #region Connection Methods

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

        #endregion

        /// <summary>
        /// Create database file if it`s not exist
        /// </summary>
        public void CreateDatabase()
        {
            if (File.Exists("master.db")) return;

            SQLiteConnection.CreateFile("master.db");
            OpenConnection();
            using (_SQLiteConnection)
            {
                _SQLiteConnection.Open();

                var createImageModelsTable = "CREATE TABLE \"ImageModels\" (" +
                                             "\"ID\"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
	                                         "\"Fullpath\"  TEXT," +
	                                         "\"ScaleX\"    FLOAT," +
	                                         "\"ScaleY\"    FLOAT," +
	                                         "\"Angle\" FLOAT)";

                var command = new SQLiteCommand(createImageModelsTable, _SQLiteConnection);
                command.ExecuteNonQuery();

                var createPageModelsTable = "CREATE TABLE \"PageModels\" (" +
                                            "\"IsListVisible\" BOOLEAN," +
	                                        "\"IsEditBarVisible\"  BOOLEAN," +
	                                        "\"ID\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
	                                        "\"ImageModelID\"  INTEGER," +
	                                        "FOREIGN KEY(\"ImageModelID\") REFERENCES \"ImageModels\"(\"ID\"))";

                command = new SQLiteCommand(createPageModelsTable, _SQLiteConnection);
                command.ExecuteNonQuery();

                var createWindowModelsTable = "CREATE TABLE \"WindowModels\" (" +
                                              "\"Left\"  FLOAT," +
	                                          "\"Top\"   FLOAT," +
	                                          "\"Width\" FLOAT," +
	                                          "\"Height\" FLOAT," +
	                                          "\"State\" INTEGER," +
	                                          "\"ID\"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE)";

                command = new SQLiteCommand(createWindowModelsTable, _SQLiteConnection);
                command.ExecuteNonQuery();

                var createEditModelsTable = "CREATE TABLE \"EditModels\" (" +
                                            "\"ID\"    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                                            "\"Path\"  TEXT," +
                                            "\"ImageModelID\"  INTEGER," +
                                            "FOREIGN KEY(\"ImageModelID\") REFERENCES \"ImageModels\"(\"ID\"))";

                command = new SQLiteCommand(createEditModelsTable, _SQLiteConnection);
                command.ExecuteNonQuery();
            }
        }

        #region 'Select' Methods

        /// <summary>
        /// Get all <see cref="ImageModel"/> from database
        /// </summary>
        /// <returns></returns>
        public async Task<List<ImageModel>> GetImageModels()
        {
            OpenConnection();

            List<ImageModel> imageModels = new List<ImageModel>();

            const string query = "SELECT * FROM [ImageModels]";

            using (_SQLiteConnection)
            { 
                var result = await _SQLiteConnection.QueryAsync<ImageModel>(query);

                imageModels = result.AsList();
            }

            return imageModels;
        }

        /// <summary>
        /// Get <see cref="ImageModel"/> with <paramref name="imageModelID"/> from database
        /// </summary>
        /// <returns></returns>
        public ImageModel GetImageModel(int imageModelID)
        {
            OpenConnection();

            ImageModel imageModels = new ImageModel();

            string query = $"SELECT * FROM [ImageModels] WHERE ID = {imageModelID}";

            using (_SQLiteConnection)
            {
                imageModels = _SQLiteConnection.QuerySingleOrDefault<ImageModel>(query);
            }

            return imageModels ?? new ImageModel();
        }

        /// <summary>
        /// Get all <see cref="PageModel"/> from database
        /// </summary>
        /// <returns></returns>
        public PageModel GetPageModel()
        {
            OpenConnection();

            PageModel pageModels = new PageModel();

            const string query = "SELECT * FROM [PageModels]";

            using (_SQLiteConnection)
            {
                pageModels = _SQLiteConnection.QuerySingleOrDefault<PageModel>(query);
            }

            return pageModels;
        }

        /// <summary>
        /// Get all <see cref="WindowModel"/> from database
        /// </summary>
        /// <returns></returns>
        public WindowModel GetWindowModel()
        {
            OpenConnection();

            WindowModel windowModels = new WindowModel();

            const string query = "SELECT * FROM [WindowModels]";

            using (_SQLiteConnection)
            {
                windowModels = _SQLiteConnection.QuerySingleOrDefault<WindowModel>(query);
            }

            return windowModels;
        }

        /// <summary>
        /// Get all <see cref="EditModel"/> from database
        /// </summary>
        /// <returns></returns>
        public List<EditModel> GetEditModels(int imageModelID)
        {
            OpenConnection();

            List<EditModel> editModels = new List<EditModel>();

            string query = $"SELECT * FROM [EditModels] WHERE [ImageModelID] = {imageModelID}";

            using (_SQLiteConnection)
            {
                editModels = _SQLiteConnection.Query<EditModel>(query).AsList();
            }

            return editModels;
        }

        #endregion

        #region 'Insert' Methods

        /// <summary>
        /// Insert <see cref="ImageModel"/> into database using parametrized query
        /// </summary>
        /// <param name="imageModel"></param>
        public async void InsertImageModel(ImageModel imageModel)
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

                await command.ExecuteNonQueryAsync();
            }

            CloseConnection();
        }

        /// <summary>
        /// Insert <see cref="WindowModel"/> into database using parametrized query
        /// </summary>
        /// <param name="windowModel"></param>
        public async void InsertWindowModel(WindowModel windowModel)
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

                await command.ExecuteNonQueryAsync();
            }

            CloseConnection();
        }

        /// <summary>
        /// Insert <see cref="PageModel"/> into database using parametrized query
        /// </summary>
        /// <param name="pageModel"></param>
        public async void InsertPageModel(PageModel pageModel)
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

                await command.ExecuteNonQueryAsync();
            }

            CloseConnection();
        }

        /// <summary>
        /// Insert <see cref="EditModel"/> into database using parametrized query
        /// </summary>
        /// <param name="editModel"></param>
        public async void InsertEditModel(EditModel editModel)
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

                await command.ExecuteNonQueryAsync();
            }

            CloseConnection();
        }

        #endregion

        #region 'Update' Methods

        /// <summary>
        /// Update <see cref="ImageModel"/> by it`s ID
        /// </summary>
        /// <param name="imageModel"></param>
        public async void UpdateImageModel(ImageModel imageModel)
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

                await command.ExecuteNonQueryAsync();
            }

            CloseConnection();
        }

        /// <summary>
        /// Update <see cref="PageModel"/> by it`s ID
        /// </summary>
        /// <param name="pageModel"></param>
        public async void UpdatePageModel(PageModel pageModel)
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

                await command.ExecuteNonQueryAsync();
            }

            CloseConnection();
        }

        /// <summary>
        /// Update <see cref="WindowModel"/> by it`s ID
        /// </summary>
        /// <param name="windowModel"></param>
        public async void UpdateWindowModel(WindowModel windowModel)
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

                await command.ExecuteNonQueryAsync();
            }

            CloseConnection();
        }

        #endregion

        #region 'Remove' Methods

        /// <summary>
        /// Remove <see cref="ImageModel"/> from database by ID
        /// </summary>
        /// <param name="imageModelID"></param>
        public async void RemoveImageModel(int imageModelID)
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

                await command.ExecuteNonQueryAsync();
            }

            CloseConnection();
        }

        #endregion
    }
}
