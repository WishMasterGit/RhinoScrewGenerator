using Eto.Forms;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ScrewThread.Views
{
    public class MainViewModel : ViewModel
    {
        private Control m_content;
        public MainViewModel(uint documentSerialNumber)
        {
            FinishCommand = new RelayCommand<object>(obj => { FinishButtonCommand(); });
            DocumentRuntimeSerialNumber = documentSerialNumber;
        }

        public uint DocumentRuntimeSerialNumber { get; }
        /// <summary>
        /// Bind this to the main panel container contents, will get set by the
        /// next and back buttons
        /// </summary>
        public Control Content
        {
            get => m_content;
            set
            {
                if (m_content == value)
                    return;
                m_content = value;
                RaisePropertyChanged(nameof(Content));
            }
        }
        public ICommand FinishCommand { get; }
        private void FinishButtonCommand()
        {
            Panels.ClosePanel(typeof(MainPanel).GUID);
        }
    }
}
