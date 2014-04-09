using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CustomMarkerProject.Resources;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using System.Windows.Resources;
using System.Windows.Media.Imaging;

namespace CustomMarkerProject
{
    public partial class MainPage : PhoneApplicationPage
    {

        // just some fake markers 
        private IList<Marker> fakeMarkers = new List<Marker>
        {
              new Marker {Latitude = 49.0, Longitude = 24.1, Direction = 100}, 
              new Marker {Latitude = 49.1, Longitude = 24.2, Direction = 0},
              new Marker {Latitude = 49.2, Longitude = 24.3, Direction = 40},
              new Marker {Latitude = 49.3, Longitude = 24.4, Direction = 90},
              new Marker {Latitude = 49.4, Longitude = 24.5, Direction = 270}
            };

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            MyMap.Center = new GeoCoordinate(49.0, 24.0);
            MyMap.ZoomLevel = 10;
            
            DisplayMarkers(fakeMarkers);
        }

		// current layer to display all markers
        MapLayer current = null;

		// size of the image size
        public const int RECT_SIZE = 48;

        private void DisplayMarkers(IList<Marker> markers)
        {
		// invoke on UI thread. This should be done if you invoke this method from another thread
            Dispatcher.BeginInvoke(() =>
            {
			// make sure that current layer is not null, and remove it from the Map.
			// this should be invoked only if you want to delete all markers previously displayed.
                if (current != null && MyMap.Layers.Contains(current))
                { 
                    MyMap.Layers.Remove(current);
					
					// delete all MapOverlays from MapLayer. Not quite sure should it be placed here or just wasting
					// time( We've already deleted MapLayer from the Map).
                    current.Clear();
                }
				
                // sounds weird, but MapLayer should be recreated - app will  throw Exception when second time
				// displaying on this layer if wouldn't be.
                current = new MapLayer();

                foreach (Marker marker in markers)
                {
				// get the Uri of the image should be displayed
                    Uri imgUri = new Uri("image.png", UriKind.Relative);

					// if image doesn't exist
                    if (imgUri == null)
                    {
                        throw new Exception("Image can't be find");
                    }
                    try
                    {

					
                        StreamResourceInfo resourceInfo = Application.GetResourceStream(imgUri);
                        BitmapImage imgSourceR = new BitmapImage();
                    
						if (resourceInfo == null)
                        {
						// if there is error when opening resource, skip this marker and go further
                            continue;
                        }
						// get the image from the stream
                        imgSourceR.SetSource(resourceInfo.Stream);

                        Image image = new Image();
                        image.Height = imgSourceR.PixelHeight;
                        image.Width = imgSourceR.PixelWidth;
                        image.Source = imgSourceR;

                        ImageBrush imgBrush = new ImageBrush() { ImageSource = image.Source };
						// If you need to rotate your marker, use this code:
                            RotateTransform rt = new RotateTransform();
                            rt.CenterX = 0.5;
                            rt.CenterY = 0.5;
                            rt.Angle = (double)marker.Direction;
                            imgBrush.RelativeTransform = rt;
						
                        System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle()
                        {
                            Fill = imgBrush,
                            Height = RECT_SIZE,
                            Width = RECT_SIZE
                        };
						
                        // move rectangle to make the center of the image corresponds the coordinate.
                        TranslateTransform tt = new TranslateTransform();
                        tt.X = -RECT_SIZE / 2;
                        tt.Y = -RECT_SIZE / 2;
                        rect.RenderTransform = tt;
						
						//actually creating of the overlay
                        MapOverlay overlay = new MapOverlay
                        {
                            GeoCoordinate = new GeoCoordinate(marker.Latitude, marker.Longitude),
                            Content = rect
                        };

                        current.Add(overlay);

                    }
                    catch (Exception e)
                    {
                       // handle exception
                    }
                }
				// add the MapLayer to the Map
                if (current != null && current.Count > 0)
                    MyMap.Layers.Add(current);

            });
        }
    }
}