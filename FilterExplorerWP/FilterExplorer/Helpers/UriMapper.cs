using System;
using System.Windows.Navigation;

namespace ImageProcessingApp.Helpers
{
    class UriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            Uri mappedUri = uri;

            if (uri.ToString().Contains("ViewfinderLaunch"))
            {
                mappedUri = new Uri("/Pages/PhotoPage.xaml?ViewfinderLaunch", UriKind.Relative);
            }

            return mappedUri;
        }
    }
}