using ImageProcessingApp.ViewModels;
using System;
using System.Windows.Controls;

namespace ImageProcessingApp.Controls
{
    public partial class PhotoMosaic : UserControl
    {
        public PhotoMosaic()
        {
            InitializeComponent();
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var m = DataContext as PhotoMosaicViewModel;

            var i = sender as Image;

            if (m != null && i != null)
            {
                m.RaiseStreamItemTapped(Int32.Parse(i.Name.Replace("Image", "")));
            }
        }
    }
}
