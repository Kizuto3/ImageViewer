using ImageViewer.Models;
using System.Data.Entity;

namespace ImageViewer.DatabaseContext
{
    class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("DefaultConnection")
        {
        }

        public DbSet<ImageModel> ImageModels { get; set; }
        public DbSet<WindowModel> WindowModels { get; set; }
        public DbSet<PageModel> PageModels { get; set; }
    }
}
