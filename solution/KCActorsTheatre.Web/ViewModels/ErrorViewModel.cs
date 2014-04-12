using System;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.Cms.Core;

namespace KCActorsTheatre.Web.ViewModels
{
    public class ErrorViewModel : JsonContentViewModelBase
    {
        public string Message { get; private set; }

        public ErrorViewModel(RequestContent cmsRequestContent, Exception exception) : this(cmsRequestContent, exception.GetInnermostException().Message) { }

        public ErrorViewModel(RequestContent cmsRequestContent, string message)
            : base(cmsRequestContent, "Error")
        {
            Message = message;
        }
    }
}
