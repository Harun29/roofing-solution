﻿using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace roofing_solution
{
    public partial class MainPage : ContentPage
    {
        public List<double> Heights { get; set; }
        
        private List<double> firstList;
        private List<double> secondList;

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            double sk = double.Parse(firstNumberEntry.Text);
            double vk = double.Parse(secondNumberEntry.Text);

            firstList = new List<double>(getHeights(sk, vk, ""));
            secondList = new List<double>(getHeights(sk, vk, "two"));

            

            UpdateColumns(firstList, "One");
            UpdateColumns(secondList, "Two");
        }


        private static List<double> getHeights(double sk, double vk, string type)
        {
            double st = 2;
            if (type == "two") { st = st / 2; }
            double i = (sk / 2) / st;
            if (type == "two") { i = ((sk / 2) - st) / (st * 2); }
            i = Math.Floor(i);
            List<double> h = new List<double> { Math.Round(vk, 3) };
            for (int n = 0; n < i; n++)
            {
                double sv = ((2 * vk) * ((sk / 2) - st)) / sk;
                h.Add(Math.Round(sv, 3));
                vk = sv;
                st = 2;
                if (type == "") { sk -= st * 2; }
                else
                {
                    sk -= st;
                }
            }
            List<double> hr = new List<double>(h);
            hr.Reverse();
            if (type == "two") { hr.Remove(hr.Last()); }
            List<double> fl = new List<double>(hr.Concat(h));

            return fl;
        }

        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the page to itself
            this.BindingContext = this;
        }

        // Method to update the columns in the FlexLayout
        private void UpdateColumns(List<Double> Heights, string Table)
        {
            // Clear existing columns
            if (Table == "One") { flexLayoutOne.Children.Clear(); } else
            {
                flexLayoutTwo.Children.Clear();
            }

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

                if(Table == "One") { flexLayoutOne.Children.Add(columnWithLabel); } else
                {
                    flexLayoutTwo.Children.Add(columnWithLabel);
                }
                
            }
        }
    }
}
