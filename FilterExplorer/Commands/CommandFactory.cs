using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FilterExplorer.Commands
{
    public static class CommandFactory
    {
        public static IDelegateCommand CreateGoBackCommand()
        {
            return new DelegateCommand((parameter) =>
            {
                var frame = (Frame)Window.Current.Content;

                if (frame != null && frame.CanGoBack)
                {
                    frame.GoBack();
                }
            });
        }
    }
}
