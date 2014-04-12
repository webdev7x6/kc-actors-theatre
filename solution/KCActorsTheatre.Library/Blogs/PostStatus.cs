using System.ComponentModel;

namespace KCActorsTheatre.Blogs
{
    public enum PostStatus : byte
    {
        [Description("Draft")]
        Draft,
        [Description("Published")]
        Published
    }
}
