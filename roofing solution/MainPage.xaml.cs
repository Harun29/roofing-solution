using Microsoft.Maui.Controls;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace roofing_solution
{
    public partial class MainPage : ContentPage, IDrawable
    {
        public List<double> Heights { get; set; }
        
        private List<double> firstList;
        private List<double> secondList;
        private string lossOne;
        private string lossTwo;
        private double widthOne;
        private double widthTwo;

        

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(firstNumberEntry.Text) || string.IsNullOrWhiteSpace(secondNumberEntry.Text))
            {
                DisplayAlert("Error", "Unesi vrijednost u svoja polja.", "OK");
                return;
            }

            double sk = double.Parse(firstNumberEntry.Text);
            double vk = double.Parse(secondNumberEntry.Text);

            firstList = new List<double>(getHeights(sk, vk, ""));
            secondList = new List<double>(getHeights(sk, vk, "two"));

            UpdateColumns(firstList, "One");
            UpdateColumns(secondList, "Two");
            firstLoss.Text = $"Otpad: {lossOne} m2";
            secondLoss.Text = $"Otpad: {lossTwo} m2";

            sirinaZadnjeJedan.Text = $"Širina zadnje: {widthOne}";
            sirinaZadnjeDva.Text = $"Širina zadnje: {widthTwo}";
        }

        private List<double> getHeights(double sk, double vk, string type)
        {
            double st = 1.1;
            if (type == "two") { st = st / 2; }
            double i = (sk / 2) / st;
            if (type == "two") { i = ((sk / 2) - st) / (st * 2);}
            Console.WriteLine(i);
            double k = Math.Floor(i);
            if (type == "two") { k += 1; };
            List<double> h = new List<double> { Math.Round(vk, 2) };
            for (int n = 0; n < k; n++)
            {
                double sv = ((2 * vk) * ((sk / 2) - st)) / sk;
                if(sv != 0)
                {
                    h.Add(Math.Round(sv, 2));
                }
                vk = sv;
                st = 1.1;
                if (type == "" || (type == "two" && n != 0)) { sk -= st * 2; }
                else if(type == "two" && n == 0)
                {
                    sk -= st;
                }
            }
            List<double> hr = new List<double>(h);
            hr.Reverse();
            if (type == "two") { hr.Remove(hr.Last()); }
            List<double> fl = new List<double>(hr.Concat(h));

            double loss = calculateLoss(st, fl[0], (i - Math.Floor(i)) * st);
            double width = Math.Round(((i - Math.Floor(i)) * st), 2);
            if (width == 0)
            {
                width = st;
            }
            foreach (double v in hr)
            {
                int index = hr.IndexOf(v);
                if(index == 0)
                {
                    continue;
                }
                loss += ((v - hr[index - 1]) * st) / 2;
            }

            if(type == "two")
            {
                loss += ((h[0] - h[1]) * (st / 2)) / 2;
            }

            loss *= 2;
            loss = Math.Round(loss, 2);

            if (type == "two") { lossTwo = loss.ToString(); widthTwo = width; }
            else
            {
                lossOne = loss.ToString();
                widthOne = width;
            }

            return fl;
        }

        private static double calculateLoss(double st, double a, double b)
        {
            double loss = (st * a) - (a * b)/2;
            loss = Math.Round(loss, 3);
            return loss;
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
                    BackgroundColor = Colors.BlueViolet,
                    HeightRequest = height * 20,
                    WidthRequest = 30,
                    Margin = new Thickness(2)
                };

                var heightLabel = new Label
                {
                    Text = height.ToString() + " ",
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

        public void draw(icanvas canvas, rectf dirtyrect)
        {
            canvas.strokecolor = colors.red;
            canvas.strokesize = 6;
            canvas.drawline(10, 10, 90, 100);
        }

    }
}
