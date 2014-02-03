using FilterExplorer.Models;
using FilterExplorer.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FilterExplorer.Views
{
    public sealed partial class StreamPage : Page
    {
        public StreamPage()
        {
            DataContext = new StreamPageViewModel();

            this.InitializeComponent();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var viewModel = DataContext as StreamPageViewModel;

            if (viewModel != null)
            {
                viewModel.SelectPhotoCommand.Execute(e.ClickedItem);
            }
        }
    }
}
