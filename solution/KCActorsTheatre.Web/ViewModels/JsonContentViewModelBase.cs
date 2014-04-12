using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;

namespace KCActorsTheatre.Web.ViewModels
{
    public abstract class JsonContentViewModelBase : BaseViewModel
    {
        public string Template { get; private set; }

        protected JsonContentViewModelBase(RequestContent cmsRequestContent, string template)
        {
            RequestContent = cmsRequestContent;
            Template = template;
        }
    }
}
