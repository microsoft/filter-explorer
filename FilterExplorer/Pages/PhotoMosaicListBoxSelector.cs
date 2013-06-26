using ImageProcessingApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ImageProcessingApp.Pages
{
    public class PhotoMosaicListBoxSelector : ContentControl
    {
        public DataTemplate PhotoMosaicTemplate { get; set; }
        public DataTemplate PhotoMosaicButtonTemplate { get; set; }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent);
        }

        public DataTemplate SelectTemplate(object item)
        {
            if (item as PhotoMosaicViewModel != null)
            {
                return PhotoMosaicTemplate;
            }
            else if (item as PhotoMosaicButtonViewModel != null)
            {
                return PhotoMosaicButtonTemplate;
            }
            else
            {
                return null;
            }
        }
    }
}
