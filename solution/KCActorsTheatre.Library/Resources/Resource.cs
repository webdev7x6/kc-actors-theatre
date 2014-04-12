using KCActorsTheatre.Tags;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace KCActorsTheatre.Resources
{
    public class Resource
    {
        public int ResourceID { get; set; }
        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }
        public DateTime? DateExpired { get; set; }
        public DateTime? DatePublished { get; set; }

        [NotMapped]
        public bool IsPublished { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// generic property used to store associated URL for different resource types
        /// </summary>
        public string URL { get; set; }

        public string ThumbnailImage { get; set; }

        public ResourceType ResourceType { get; set; }
        public Language Language { get; set; }

        private HashSet<Tag> _tags = new HashSet<Tag>();
        public ICollection<Tag> Tags { get { return _tags; } }
    }

    public class AudioResource : Resource
    {
        public AudioResource() { ResourceType = ResourceType.Audio; }
    }

    public class DocumentResource : Resource
    {
        public DocumentResource() { ResourceType = ResourceType.Document; }
    }

    public class MediaResource : Resource
    {
        public MediaResource() { ResourceType = ResourceType.Media; }
    }

    public class ProductResource : Resource
    {
        public ProductResource() { ResourceType = ResourceType.Product; }
    }

    public class PublicationResource : Resource
    {
        public PublicationResource() { ResourceType = ResourceType.Publication; }
        public DateTime? PublicationMonth { get; set; }
    }

    public class SlideshowResource : Resource
    {
        public SlideshowResource() { ResourceType = ResourceType.Slideshow; }
    }

    public class VideoResource : Resource
    {
        public VideoResource() { ResourceType = ResourceType.Video; }
        public VideoType VideoType  { get; set; }
        public string LimelightMediaID { get; set; }
    }
    public class PresentationResource : Resource
    {
        public PresentationResource() { ResourceType = ResourceType.Presentation; }
        public int SlideshareID { get; set; }
    }
}