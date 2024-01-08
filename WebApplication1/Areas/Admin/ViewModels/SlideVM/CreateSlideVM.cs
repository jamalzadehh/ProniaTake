namespace WebApplication1.Areas.Admin.ViewModels.SlideVM
{
    public class CreateSlideVM
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public IFormFile Image { get; set; }
    }
}
