using System.ComponentModel;

namespace KCActorsTheatre.Resources
{
    public enum ResourceType
    {
        [Description("Audio")]
        Audio,
        [Description("Document")]
        Document,
        [Description("CD-ROM/DVD")]
        Media,
        [Description("Product")]
        Product,
        [Description("Publication")]
        Publication,
        [Description("Slideshow")]
        Slideshow,
        [Description("Video")]
        Video,
        [Description("Presentation")]
        Presentation,
    }
}