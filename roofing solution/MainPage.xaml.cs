using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace roofing_solution
{
    public partial class MainPage : ContentPage
    {
        public List<double> Heights { get; set; }
        
        private List<double> firstList;
        private List<double> secondList;
        private string lossOne;
        private string lossTwo;

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            double sk = double.Parse(firstNumberEntry.Text);
            double vk = double.Parse(secondNumberEntry.Text);

            firstList = new List<double>(getHeights(sk, vk, ""));
            secondList = new List<double>(getHeights(sk, vk, "two"));

            UpdateColumns(firstList, "One");
            UpdateColumns(secondList, "Two");
            firstLoss.Text = $"otpad na zadnjoj tabli: {lossOne} m2";
            secondLoss.Text = $"otpad na zadnjoj tabli: {lossTwo} m2";
        }


        private List<double> getHeights(double sk, double vk, string type)
        {
            double st = 1.5;
            if (type == "two") { st = st / 2; }
            double i = (sk / 2) / st;
            if (type == "two") { i = ((sk / 2) - st) / (st * 2) + 1; }
            i = Math.Floor(i);
            List<double> h = new List<double> { Math.Round(vk, 3) };
            for (int n = 0; n < i; n++)
            {
                double sv = ((2 * vk) * ((sk / 2) - st)) / sk;
                h.Add(Math.Round(sv, 3));
                vk = sv;
                st = 1.5;
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

            double loss = calculateLoss(st, fl[0], (i - Math.Floor(i)) * st);
            if (type == "two") { lossTwo = loss.ToString(); }
            else
            {
                lossOne = loss.ToString();
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
