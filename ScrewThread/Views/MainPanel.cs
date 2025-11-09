using Eto.Drawing;
using Eto.Forms;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrewThread.Views
{
    [Guid("D4E8F1A2-3B6C-4F9B-8A7B-5C3D2E1F0A9B")]
    public class MainPanel : Panel
    {
        public static int Spacing => 4;
        public MainViewModel ViewModel { get; }
        /// <summary>
        /// Standard spacing used by all wizard pages
        /// </summary>
        public static Size SpacingSize = new Size(Spacing, Spacing);
        public MainPanel(uint documentRuntimeSerialNumber)
        {
            Padding = 6;
            var veiwModel = new MainViewModel(documentRuntimeSerialNumber);
            DataContext = veiwModel;
            ViewModel = veiwModel;
            this.Bind(c => c.Content, veiwModel, m => m.Content);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // This method can be used to initialize the panel's components if needed.
            // For example, you can add controls to the panel here.
            TextBox textbox = new TextBox();
            Content = new TableLayout
            {
                Spacing = SpacingSize,
                Rows =
                {
                    new TableRow(textbox) { ScaleHeight = true },
                    new StackLayout
                    {
                     Padding = 0,
                     HorizontalContentAlignment = HorizontalAlignment.Stretch,
                     Spacing = Spacing,
                     Items =
                     {
                        new Button {Text = Rhino.UI.LOC.STR("Next >"), Command = ViewModel.FinishCommand},
                     }
                }
            }
            };
        }
    }
}
