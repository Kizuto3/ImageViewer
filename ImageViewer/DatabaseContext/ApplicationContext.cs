using ImageViewer.Models;
using System.Data.Entity;

namespace ImageViewer.DatabaseContext
{
    /// <summary>
    /// Database context to manage database using EF6 and SQLite
    /// </summary>
    class ApplicationContext : DbContext
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationContext() : base("DefaultConnection")
        {
        }

        #endregion

        #region Entities

        /// <summary>
        /// <see cref="ImageModel"/> entities
        /// </summary>
        public DbSet<ImageModel> ImageModels { get; set; }

        /// <summary>
        /// <see cref="WindowModel"/> entities
        /// </summary>
        public DbSet<WindowModel> WindowModels { get; set; }

        /// <summary>
        /// <see cref="PageModel"/> entities
        /// </summary>
        public DbSet<PageModel> PageModels { get; set; }

        #endregion
    }
}
