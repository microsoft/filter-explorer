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

            VersionTextBlock.Text = String.Format(AppResources.AboutPage_TextBlock_Version,
                XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value);
        }
    }
}