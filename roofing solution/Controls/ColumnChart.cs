using Microsoft.Maui.Controls;

namespace roofing_solution.Controls
{
    public class ColumnChart : ContentView
    {
        private int[] _data;

        public ColumnChart()
        {
            _data = new int[0];
            UpdateChart();
        }

        public ColumnChart(int[] data)
        {
            _data = data;
            UpdateChart();
        }

        private void UpdateChart()
        {
            Content = null;

            var grid = new Grid();

            // Add rows to the grid based on the maximum value in the data array
            int maxValue = _data.Length > 0 ? _data.Max() : 0;
            for (int i = 0; i < maxValue; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            }

            // Add columns to the grid
            for (int i = 0; i < _data.Length; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Populate the grid with BoxViews
            for (int i = 0; i < _data.Length; i++)
            {
                int value = _data[i];
                for (int j = 0; j < value; j++)
                {
                    var boxView = new BoxView { Color = Colors.Blue };
                    Grid.SetRow(boxView, maxValue - j - 1); // Set the row index
                    Grid.SetColumn(boxView, i); // Set the column index
                    grid.Children.Add(boxView); // Add the BoxView to the grid
                }
            }

            Content = grid;
        }

        public int[] Data
        {
            get => _data;
            set
            {
                _data = value;
                UpdateChart();
            }
        }
    }
}
