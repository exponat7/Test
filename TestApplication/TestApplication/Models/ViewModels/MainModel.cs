using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplication.Models.ViewModels
{
    public class MainModel
    {
        public TabModel StartTab { get; set; }

        public SettingsModel Settings { get; set; }

        public MainModel()
        {
            StartTab = new TabModel();
            Settings = new SettingsModel();
        }
    }

    public class TabModel
    {
        public IncomingRequests Requests { get; set; }
    }
}
