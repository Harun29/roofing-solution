using Microsoft.Maui.Controls;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;
using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Platform.Compatibility;

namespace roofing_solution
{

    public partial class MainPage : ContentPage
    {
        // Non-nullable fields
        private List<double> firstList = [];
        private List<double> secondList = [];
        private string lossOne = string.Empty;
        private string lossTwo = string.Empty;
        private double widthOne;
        private double widthTwo;
        private string roofType = "Standardno";
        private double area;
        private double st = 1.1;

        private void CheckSelectedPickerItem()
        {
            string? selectedValue = picker.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedValue))
            {
                roofType = selectedValue;
                if (selectedValue == "Ravan vrh")
                {
                    additionalEntryField.IsVisible = true;
                }
                else
                {
                    additionalEntryField.IsVisible = false;
                }
            }
            else
            {
                DisplayAlert("Error", "No item is selected.", "OK");
            }
        }

        private void DrawOnCanvas(GraphicsView canvas, double Height, double Width, double LastWidth, double PanelWidth, List<double> List, double cutOutWidth)
        {
            canvas.Drawable = new CustomDrawable(Height, Width, LastWidth, PanelWidth, List, cutOutWidth);
        }

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(firstNumberEntry.Text) || string.IsNullOrWhiteSpace(secondNumberEntry.Text) || (roofType == "Ravan vrh" && string.IsNullOrWhiteSpace(thirdNumberEntry.Text)))
            {
                DisplayAlert("Error", "Unesi vrijednost u svoja polja.", "OK");
                return;
            }

            double sk = double.Parse(firstNumberEntry.Text);
            double vk = double.Parse(secondNumberEntry.Text);
            double si = 0.0;
            if(roofType == "Ravan vrh")
            {
                si = double.Parse(thirdNumberEntry.Text);
            }
            area = ((sk * (vk + ((vk * si) / (sk - si)))) / 2) - (si*((vk * si) / (sk - si)))/2;
            area = Math.Round(area,2);
            povrsina.Text = $"Površina krova: {area} m2";

            firstList = new List<double>(GetHeights(sk, vk, "", si));
            secondList = new List<double>(GetHeights(sk, vk, "two", si));

            UpdateColumns(firstList, "One");
            UpdateColumns(secondList, "Two");
            firstLoss.Text = $"Otpad: {lossOne} m2";
            secondLoss.Text = $"Otpad: {lossTwo} m2";

            saSredine.Text = "Sa sredine";
            naStrane.Text = "Na strane";

            sirinaZadnjeJedan.Text = $"Širina zadnje: {widthOne}m";
            sirinaZadnjeDva.Text = $"Širina zadnje: {widthTwo}m";

            CanvasTwo.HeightRequest = vk*40;
            CanvasOne.HeightRequest = vk*40;
            CanvasTwo.WidthRequest = sk*40;
            CanvasOne.WidthRequest = sk*40;

            DrawOnCanvas(CanvasOne, vk, sk, widthOne, st, firstList, si);
            DrawOnCanvas(CanvasTwo, vk, sk, widthTwo, st, secondList, si);

            if(double.Parse(lossOne) > double.Parse(lossTwo))
            {
                firstLoss.TextColor = Colors.Red;
                secondLoss.TextColor = Colors.Green;
            }
            else
            {
                secondLoss.TextColor = Colors.Red;
                firstLoss.TextColor = Colors.Green;
            }

        }


        private List<double> GetHeights(double sk, double vk, string type, double si)
        {
            double originalHeigh = vk;
            double originalWidth = sk;
            if (roofType == "Ravan vrh")
            {
                vk += (vk * si) / (sk - si);
            }

            if (type == "two") { st /= 2; }
            double i = (sk / 2) / st;
            if (type == "two") { i = ((sk / 2) - st) / (st * 2);}
            double k = Math.Floor(i);
            if (type == "two") { k += 1; };
            List<double> h = [Math.Round(vk, 2)];
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
            if (roofType == "Ravan vrh")
            {
                h[0] -= Math.Round(((originalHeigh * si) / (originalWidth - si)), 2);
                h[0] = Math.Round(h[0], 2);
                for (int index = 1; index <= h.Count - 1 ; index++)
                {
                    if (h[index] > h[index - 1])
                    {
                        h[index] = h[index - 1];
                    }
                }
            }
            List<double> hr = new(h);
            hr.Reverse();
            if (type == "two") { hr.Remove(hr.Last()); }
            List<double> fl = new(hr.Concat(h));

            double width = Math.Round(((i - Math.Floor(i)) * st), 2);
            if (width == 0)
            {
                width = st;
            }


            double loss = CalculateLoss(fl, st, area);
            loss = Math.Round(loss, 2);

            if (type == "two") { lossTwo = loss.ToString(); widthTwo = width; }
            else
            {
                lossOne = loss.ToString();
                widthOne = width;
            }

            return fl;
        }

        private static double CalculateLoss(List<double> list, double panelWidth, double roofArea)
        {
            double areaOfPanels = 0;
            foreach(double height in list)
            {
                areaOfPanels += height * panelWidth;
            }
            return areaOfPanels - roofArea;
        }

        public MainPage()
        {
            InitializeComponent();

            this.BindingContext = this;

            picker.SelectedIndexChanged += (s, e) => CheckSelectedPickerItem();
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
                    HeightRequest = height * 40,
                    WidthRequest = 20,
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

    }
}
