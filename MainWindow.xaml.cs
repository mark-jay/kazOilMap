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

namespace TestShapeFile
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
            string msg = TestShapeFile.Properties.Resources.MainWindow_ExitQuestion;
            string appName = TestShapeFile.Properties.Resources.MainWindow_Title;
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
            string msg = TestShapeFile.Properties.Resources.MainWindow_ResetQuestion;
            string appName = TestShapeFile.Properties.Resources.MainWindow_Title;
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

    }
}

// END

/*
<Window x:Class="TestShapeFile.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    Title="TestShapeFile" Height="409" Width="576" xmlns:my="clr-namespace:System;assembly=mscorlib"
    Closing="mainWindow_Closing" KeyDown="mainWindow_KeyDown">
    <Grid>
    <Menu Height="22" Margin="0,0,0,0" Name="menu1" VerticalAlignment="Top" >
      <MenuItem Name="fileMI" Header="_File" SubmenuOpened="fileMI_SubmenuOpened">
        <MenuItem Name="openMI" Header="_Open..." Click="openMI_Click"/>        
        <Separator/>
        <MenuItem Name="resetMI" Header="_Reset" Click="resetMI_Click"/>
        <Separator/>
        <MenuItem Name="exitMI" Header="E_xit" Click="exitMI_Click"/>
      </MenuItem>
      <MenuItem Name="viewMI" Header="_View" SubmenuOpened="viewMI_SubmenuOpened">
        <MenuItem Name="displayLonLatMI" Header="Display _Lon/Lat" IsCheckable="True" IsChecked="True" Click="displayLonLatMI_Click"/>
        <Separator/>
        <MenuItem Name="enablePanningMI" Header="Enable _Panning" IsCheckable="True" IsChecked="True" Click="enablePanningMI_Click"/>
        <MenuItem Name="zoomMI" Header="_Zoom" SubmenuOpened="zoomMI_SubmenuOpened">
          <MenuItem Name="zoom50MI" Header="Zoom 50%" Click="zoom50_Click"/>
          <MenuItem Name="zoom100MI" Header="Zoom 100%" Click="zoom100_Click"/>
          <MenuItem Name="zoom200MI" Header="Zoom 200%" Click="zoom200_Click"/>
          <MenuItem Name="zoom400MI" Header="Zoom 400%" Click="zoom400_Click"/>
          <MenuItem Name="zoom800MI" Header="Zoom 800%" Click="zoom800_Click"/>
          <MenuItem Name="zoom1600MI" Header="Zoom 1600%" Click="zoom1600_Click"/>
          <MenuItem Name="zoom3200MI" Header="Zoom 3200%" Click="zoom3200_Click"/>
        </MenuItem>
      </MenuItem>
      <MenuItem Header="Optio_ns">
        <MenuItem Name="geometryTypeMI" Header="_Geometry Type" SubmenuOpened="geometryTypeMI_SubmenuOpened">
          <MenuItem Name="pathGeometryMI" Header="_Path Geometry" IsCheckable="True" Click="pathGeometryMI_Click"/>
          <MenuItem Name="streamGeometryMI" Header="_Stream Geometry" IsCheckable="True" Click="streamGeometryMI_Click"/>
          <MenuItem Name="streamGeometryUnstrokedMI" Header="Stream Geometry _Unstroked" IsCheckable="True" Click="streamGeometryUnstrokedMI_Click"/>
        </MenuItem>
      </MenuItem>
    </Menu>
    <Menu Height="22" Margin="0,22,0,0" Name="menu2" VerticalAlignment="Top" HorizontalAlignment="Right">
            <MenuItem>
                <MenuItem.Header>
                    <StackPanel>
                        <Image Width="20" Height="20" Source="./resources/images/zoom_in.ico" />
                        <ContentPresenter Content="Reports" />
                    </StackPanel>
                </MenuItem.Header>
            </MenuItem>
            <MenuItem>
                <MenuItem.Header>
                    <StackPanel>
                        <Image Width="20" Height="20" Source="./resources/images/zoom_out.ico" />
                        <ContentPresenter Content="Reports" />
                    </StackPanel>
                </MenuItem.Header>
            </MenuItem>
            <MenuItem>
                <MenuItem.Header>
                    <StackPanel>
                        <Image Width="20" Height="20" Source="./resources/images/hand.png" />
                        <ContentPresenter Content="Reports" />
                    </StackPanel>
                </MenuItem.Header>
            </MenuItem>
    </Menu>
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="9*"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="2*"/>
            </Grid.RowDefinitions>
            <Canvas Margin="0,44,0,0" MinHeight="50" MinWidth="50" Name="canvas1" ClipToBounds="True" RenderOptions.BitmapScalingMode="Unspecified">
            </Canvas>
            <GridSplitter HorizontalAlignment="Right" 
                  VerticalAlignment="Stretch" 
                  Grid.Row="1" ResizeBehavior="PreviousAndNext"
                  Width="5" Background="#FFBCBCBC"/>
            <Label Content="Right" Grid.Row="2" />
        </Grid> 
    </Grid>
</Window>
 * 
         <GridSplitter HorizontalAlignment="Right" 
                      VerticalAlignment="Stretch" 
                      Grid.Row="1" ResizeBehavior="PreviousAndNext"
                      Width="5" Background="#FFBCBCBC"/>
 * 
 * <Label Content="Bottom" Grid.Row="4" />
 
 <Menu  Margin="0,22,0,0" Height="20" Background="#FFA9D1F4">
            <Menu.ItemsPanel>
                <ItemsPanelTemplate>
                    <DockPanel HorizontalAlignment="Stretch"/>
                </ItemsPanelTemplate>
            </Menu.ItemsPanel>
            <MenuItem Header="File">
                <MenuItem Header="Exit"/>
            </MenuItem>
            <MenuItem Header="Edit">
                <MenuItem Header="Cut"/>
            </MenuItem>
            <MenuItem Header="Help" HorizontalAlignment="Right">
                <MenuItem Header="About"/>
            </MenuItem>
        </Menu>
*/