using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace roofing_solution
{
    public partial class MainPage : ContentPage
    {
        public List<double> Heights { get; set; }

        public MainPage()
        {
            InitializeComponent();

            // Sample data
            Heights = new List<double> {};

            // Set the data context of the page to itself
            this.BindingContext = this;

            // Call method to update columns
            UpdateColumns();
        }

        // Method to update the columns in the FlexLayout
        private void UpdateColumns()
        {
            // Clear existing columns
            flexLayout.Children.Clear();

            // Add new columns based on Heights
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
                    VerticalOptions = LayoutOptions.End,
                    HorizontalOptions = LayoutOptions.Center
                };

                flexLayout.Children.Add(columnWithLabel);
            }
        }
    }
}
