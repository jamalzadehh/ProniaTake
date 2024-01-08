namespace WebApplication1.Areas.Admin.ViewModels.SlideVM
{
    public class UpdateSlideVM
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string ImgeUrl { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
