using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class DetailVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }

    }
}
