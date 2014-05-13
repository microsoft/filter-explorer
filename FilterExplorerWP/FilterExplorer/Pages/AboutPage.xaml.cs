using ImageProcessingApp.Resources;
using Microsoft.Phone.Controls;
using System;
using System.Xml.Linq;

namespace ImageProcessingApp.Pages
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            var xElement = XDocument.Load("WMAppManifest.xml").Root;
            if (xElement != null)
            {
                var element = xElement.Element("App");
                if (element != null)
                    VersionTextBlock.Text = String.Format(AppResources.AboutPage_TextBlock_Version,
                        element.Attribute("Version").Value);
            }
        }
    }
}