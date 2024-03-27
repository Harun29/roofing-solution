using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace roofing_solution.Controls
{
    public class PanelHeightDisplay : ContentView
    {
        public static readonly BindableProperty HeightsProperty =
            BindableProperty.Create(nameof(Heights), typeof(List<double>), typeof(PanelHeightDisplay), new List<double>(), propertyChanged: HeightsChanged);

        public List<double> Heights
        {
            get { return (List<double>)GetValue(HeightsProperty); }
            set { SetValue(HeightsProperty, value); }
        }

        public PanelHeightDisplay()
        {
            // No need to create a new StackLayout here
            // Content will be set in XAML
        }

        private static void HeightsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as PanelHeightDisplay;
            if (control != null)
            {
                control.UpdateColumns();
            }
        }

        public void UpdateColumns()
        {
            // Check if Content is a StackLayout
            if (Content is StackLayout panelStack)
            {
                panelStack.Children.Clear();

                foreach (var height in Heights)
                {
                    var column = new BoxView
                    {
                        BackgroundColor = Color.FromRgb(0, 0, 255), // Customize as needed
                        HeightRequest = height * 20,   // Adjust multiplier as needed for scaling
                        WidthRequest = 40,             // Adjust as needed
                        Margin = new Thickness(2)      // Adjust margin as needed
                    };

                    var heightLabel = new Label
                    {
                        Text = height.ToString(),
                        HorizontalOptions = LayoutOptions.Center
                    };

                    var columnWithLabel = new StackLayout
                    {
                        Children = { column, heightLabel },
                        VerticalOptions = LayoutOptions.EndAndExpand,
                        HorizontalOptions = LayoutOptions.Center
                    };

                    panelStack.Children.Add(columnWithLabel);
                }
            }
        }
    }
}
