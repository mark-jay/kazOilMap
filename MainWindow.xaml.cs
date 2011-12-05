// Filename:    MainWindow.xaml.cs
// Description: MainWindow is the main application window for the
//              TestShapeFile application.
// 2007-01-22 nschan Initial revision.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using Ionic.Zip;

namespace kazOilMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public sealed partial class MainWindow : System.Windows.Window
    {
        #region Private fields
        // Open file dialog for choosing a shapefile.
        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

        // Helper class for reading shapefile and displaying WPF shapes on canvas.
        ShapeDisplay shapeDisplay;

        private static readonly GradientBrush MENU_BACKGROUND = new LinearGradientBrush(Color.FromArgb(255, 158, 190, 245), Color.FromArgb(255, 196, 218, 250), 45);

        private KomProject currentProject = null;

        #endregion Private fields

        #region Constructor
        /// <summary>
        /// Constructor for MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            // Create the shape display instance.
            this.shapeDisplay = new ShapeDisplay(this, this.canvas1);

            // Colorize the menus.
            this.menu1.Background = MENU_BACKGROUND;
            this.menu2.Background = MENU_BACKGROUND;

            // Colorize the canvas.
            this.canvas1.Background = new LinearGradientBrush(Colors.WhiteSmoke, Colors.LightSteelBlue, 45);
        }
        #endregion Constructor

        #region Window closing
        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            // Ask the user to confirm exit.
            string msg = kazOilMap.Properties.Resources.MainWindow_ExitQuestion;
            string appName = kazOilMap.Properties.Resources.MainWindow_Title;
            MessageBoxResult result = MessageBox.Show(msg, appName, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if ( result == MessageBoxResult.Yes )
            {
                // Proceed with closing of the window.
                this.shapeDisplay.CancelReadShapeFile();
                e.Cancel = false;
                return;
            }

            // Cancel the closing of the window.
            e.Cancel = true;
        }
        #endregion Window closing

        #region File Menu
        /// <summary>
        /// Handle when the File menu is opened, so that we can update
        /// the enabled state of the menu items appropriately.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void fileMI_SubmenuOpened(object sender, EventArgs e)
        {
            // Disable menu items if we are already reading a shapefile.
            this.openMI.IsEnabled  = !this.shapeDisplay.IsReadingShapeFile;
            this.resetMI.IsEnabled = this.openMI.IsEnabled;           
        } 

        /// <summary>
        /// Handle File|Open menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void openMI_Click(object sender, EventArgs e)
        {
            // Show the Open File dialog.
            this.openFileDialog.Filter = "Shapefiles (*.shp)|*.shp";
            this.openFileDialog.Title = "Open Shapefile for Import";
            Nullable<bool> result = this.openFileDialog.ShowDialog();
            if ( !result.HasValue || !result.Value )
                return;
            
            // Read the shapefile.
            this.shapeDisplay.ReadShapeFile(this.openFileDialog.FileName);
        }      

        /// <summary>
        /// Handle File|Reset menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void resetMI_Click(object sender, EventArgs e)
        {
            // Ask user to confirm reset of canvas.
            string msg = kazOilMap.Properties.Resources.MainWindow_ResetQuestion;
            string appName = kazOilMap.Properties.Resources.MainWindow_Title;
            MessageBoxResult result = MessageBox.Show(msg, appName, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if ( result == MessageBoxResult.Yes )
            {
                // Reset the canvas.
                this.shapeDisplay.ResetCanvas();
            }
        }

        /// <summary>
        /// Handle File|Exit menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void exitMI_Click(object sender, EventArgs e)
        {
            // When we call Close(), this will trigger the Window.Closing event.
            this.Close();
        }
        #endregion File Menu

        #region View Menu
        /// <summary>
        /// Handle when the View Menu is opened.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void viewMI_SubmenuOpened(object sender, EventArgs e)
        {
            // Update enabled and checked states of menu items.
            this.displayLonLatMI.IsEnabled = (this.shapeDisplay.CanZoom && !this.shapeDisplay.IsReadingShapeFile);
            this.displayLonLatMI.IsChecked = this.shapeDisplay.IsDisplayLonLatEnabled;

            this.enablePanningMI.IsEnabled = this.displayLonLatMI.IsEnabled;
            this.enablePanningMI.IsChecked = this.shapeDisplay.IsPanningEnabled;

            this.zoomMI.IsEnabled = this.displayLonLatMI.IsEnabled;
        }

        /// <summary>
        /// Handle View|Display Lon/Lat menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void displayLonLatMI_Click(object sender, EventArgs e)
        {
            // Toggle the display of the lon/lat coordinates on the canvas.
            this.shapeDisplay.IsDisplayLonLatEnabled = !this.shapeDisplay.IsDisplayLonLatEnabled;
        }

        /// <summary>
        /// Handle View|Enable Panning menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void enablePanningMI_Click(object sender, EventArgs e)
        {
            // Toggle the panning feature on or off.
            this.shapeDisplay.IsPanningEnabled = !this.shapeDisplay.IsPanningEnabled;
        }
        #endregion View Menu

        #region Zoom Menu
        /// <summary>
        /// Handle when the View|Zoom menu is opened, so that we can update
        /// the enabled state of the menu items appropriately.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void zoomMI_SubmenuOpened(object sender, EventArgs e)
        {           
            this.zoom50MI.IsEnabled   = (this.shapeDisplay.CanZoom && !this.shapeDisplay.IsReadingShapeFile);
            this.zoom100MI.IsEnabled  = this.zoom50MI.IsEnabled;
            this.zoom200MI.IsEnabled  = this.zoom50MI.IsEnabled;
            this.zoom400MI.IsEnabled  = this.zoom50MI.IsEnabled;
            this.zoom800MI.IsEnabled  = this.zoom50MI.IsEnabled;
            this.zoom1600MI.IsEnabled = this.zoom50MI.IsEnabled;
            this.zoom3200MI.IsEnabled = this.zoom50MI.IsEnabled;           
        }

        /// <summary>
        /// Handle View|Zoom 50% menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void zoom50_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.Zoom(0.5);
        }

        /// <summary>
        /// Handle View|Zoom 100% menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void zoom100_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.Zoom(1);
        }

        /// <summary>
        /// Handle View|Zoom 200% menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void zoom200_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.Zoom(2);
        }

        /// <summary>
        /// Handle View|Zoom 400% menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void zoom400_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.Zoom(4);
        }

        /// <summary>
        /// Handle View|Zoom 800% menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void zoom800_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.Zoom(8);
        }

        /// <summary>
        /// Handle View|Zoom 1600% menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void zoom1600_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.Zoom(16);
        }

        /// <summary>
        /// Handle View|Zoom 3200% menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void zoom3200_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.Zoom(32);
        }
        #endregion Zoom Menu

        #region Geometry Type Menu
        /// <summary>
        /// Handle when the Options|Geometry Type menu is opened, so that we can
        /// update the enabled and checked states of the menu items appropriately.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void geometryTypeMI_SubmenuOpened(object sender, EventArgs e)
        {
            // Update enabled states.
            this.pathGeometryMI.IsEnabled = !this.shapeDisplay.IsReadingShapeFile;
            this.streamGeometryMI.IsEnabled = this.pathGeometryMI.IsEnabled;
            this.streamGeometryUnstrokedMI.IsEnabled = this.pathGeometryMI.IsEnabled;

            // Update checked states.
            switch( this.shapeDisplay.GeometryType )
            {
                case GeometryType.UsePathGeometry:
                    this.pathGeometryMI.IsChecked = true;
                    this.streamGeometryMI.IsChecked = false;
                    this.streamGeometryUnstrokedMI.IsChecked = false;
                    break;
                case GeometryType.UseStreamGeometry:
                    this.pathGeometryMI.IsChecked = false;
                    this.streamGeometryMI.IsChecked = true;
                    this.streamGeometryUnstrokedMI.IsChecked = false;
                    break;
                case GeometryType.UseStreamGeometryNotStroked:
                    this.pathGeometryMI.IsChecked = false;
                    this.streamGeometryMI.IsChecked = false;
                    this.streamGeometryUnstrokedMI.IsChecked = true;
                    break;
            }                
        }

        /// <summary>
        /// Handle Geometry Type|Path Geometry menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void pathGeometryMI_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.GeometryType = GeometryType.UsePathGeometry;
        }

        /// <summary>
        /// Handle Geometry|Stream Geometry menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void streamGeometryMI_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.GeometryType = GeometryType.UseStreamGeometry;
        }

        /// <summary>
        /// Handle Geometry Type|Stream Geometry Unstroked menu item click.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void streamGeometryUnstrokedMI_Click(object sender, EventArgs e)
        {
            this.shapeDisplay.GeometryType = GeometryType.UseStreamGeometryNotStroked;
        }
        #endregion Geometry Type Menu

        #region Keyboard handling
        /// <summary>
        /// Handle the KeyDown event for the main window.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void mainWindow_KeyDown(object sender, KeyboardEventArgs e)
        {
            // Pan 10% of the canvas width or height.
            if ( e.KeyboardDevice.IsKeyDown(Key.Left) )
                this.shapeDisplay.Pan(0.10, 0);
            else if ( e.KeyboardDevice.IsKeyDown(Key.Right) )
                this.shapeDisplay.Pan(-0.10, 0);
            else if ( e.KeyboardDevice.IsKeyDown(Key.Up) )
                this.shapeDisplay.Pan(0, 0.10);
            else if ( e.KeyboardDevice.IsKeyDown(Key.Down) )
                this.shapeDisplay.Pan(0, -0.10);
        }
        #endregion Keyboard handling

        #region My menu option handlers
        private void RunTests(object sender, EventArgs e)
        {
            // Params.ParamsTest();
            Debug(Utils.GetTempDirectory("asd"));
        }

        private void NewProject(object sender, EventArgs e)
        {
            currentProject = MakeNewProject();
            ShowMessage(Messages.projectCreated);
            saveProjectAsMI.IsEnabled = true;
        }

        private void SaveProjectAs(object sender, EventArgs e) 
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = Messages.kazOilMapProject; // Default file name
            dlg.DefaultExt = "";// Messages.kazOilMapExtension; // Default file extension
            dlg.Filter = Messages.kazOilMapFilter; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                string actualProjectName = System.IO.Path.GetFileNameWithoutExtension(filename);
                string filenameParentDir = System.IO.Path.GetDirectoryName(filename);

                // actual zipping
                using (ZipFile zip = new ZipFile())
                {
                    string dir = Utils.GetTempDirectory();

                    currentProject.paramz.Serialize(dir + System.IO.Path.DirectorySeparatorChar + Messages.kazOilMapMainXml);

                    // Debug(dir + System.IO.Path.DirectorySeparatorChar + Messages.kazOilMapMainXml);

                    zip.AddDirectory(dir);
                    zip.AddDirectoryByName("output");
                    zip.Save(filename);
                }
            }
        }

        private void OpenProject(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = Messages.kazOilMapExtension; // Default file extension
            dlg.Filter = Messages.kazOilMapFilter; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                string actualProjectName = System.IO.Path.GetFileNameWithoutExtension(filename);
                string filenameParentDir = System.IO.Path.GetDirectoryName(filename);
                
                // actual zipping
                using (ZipFile zip = new ZipFile(filename))
                {
                    string dir = Utils.GetTempDirectory() + System.IO.Path.DirectorySeparatorChar + "1";

                    Debug(dir);

                    zip.ExtractAll(dir + System.IO.Path.DirectorySeparatorChar + "1");

                    // Debug(dir + System.IO.Path.DirectorySeparatorChar + Messages.kazOilMapMainXml);

                    /*
                    zip.AddDirectory(dir);
                    zip.AddDirectoryByName("output");
                    zip.Save(filename);
                     */
                }
            }
        }
        #endregion

        #region projects managing, tests
        private KomProject MakeNewProject()
        {
            return new KomProject(Params.MakeDefaultProject());
        }
        #endregion 

        #region Utils
        /// <summary>
        /// shows message and then fades out 
        /// </summary>
        /// <param name="message"></param>
        private void ShowMessage(string message)
        {
            Messager.Content = message;

            Messager.Visibility = System.Windows.Visibility.Visible;

            DoubleAnimation a = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(2),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            Storyboard storyboard = new Storyboard();

            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, Messager);
            Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
            storyboard.Completed += delegate { Messager.Visibility = System.Windows.Visibility.Hidden; };
            storyboard.Begin();
        }

        private void Debug(string message)
        {
            DebugMessager.Content = message;
        }
        #endregion

    }
}

// END
