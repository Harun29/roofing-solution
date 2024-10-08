﻿using Microsoft.Maui.Controls;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;
using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Platform.Compatibility;
using System.Xml.Linq;

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
        private double scale = 1.0;
        private double si = 0.0;
        private double roofHeight;
        private double roofWidth; 

        private void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            scale = e.NewValue;
            UpdateColumns(firstList, "One");
            UpdateColumns(secondList, "Two");
            CanvasTwo.HeightRequest = roofHeight * 40 * scale;
            CanvasOne.HeightRequest = roofHeight * 40 * scale;
            CanvasTwo.WidthRequest = roofWidth * 40 * scale;
            CanvasOne.WidthRequest = roofWidth * 40 * scale;
            DrawOnCanvas(CanvasOne, roofHeight, roofWidth, widthOne, st, firstList, si, scale);
            DrawOnCanvas(CanvasTwo, roofHeight, roofWidth, widthTwo, st, secondList, si, scale);
        }

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
                    si = 0.0;
                    additionalEntryField.IsVisible = false;
                }
            }
            else
            {
                DisplayAlert("Error", "No item is selected.", "OK");
            }
        }

        private static void DrawOnCanvas(GraphicsView canvas, double Height, double Width, double LastWidth, double PanelWidth, List<double> List, double cutOutWidth, double scale)
        {
            canvas.Drawable = new CustomDrawable(Height, Width, LastWidth, PanelWidth, List, cutOutWidth, scale);
        }

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            scaleStepperBox.IsVisible = true;

            if (string.IsNullOrWhiteSpace(firstNumberEntry.Text) || string.IsNullOrWhiteSpace(secondNumberEntry.Text) || (roofType == "Ravan vrh" && string.IsNullOrWhiteSpace(thirdNumberEntry.Text)))
            {
                DisplayAlert("Error", "Unesi vrijednost u svoja polja.", "OK");
                return;
            }

            double sk = double.Parse(firstNumberEntry.Text);
            double vk = double.Parse(secondNumberEntry.Text);
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

            CanvasTwo.HeightRequest = vk*40*scale;
            CanvasOne.HeightRequest = vk*40*scale;
            CanvasTwo.WidthRequest = sk*40*scale;
            CanvasOne.WidthRequest = sk*40*scale;

            roofHeight = vk;
            roofWidth = sk;
            DrawOnCanvas(CanvasOne, roofHeight, roofWidth, widthOne, st, firstList, si, scale);
            DrawOnCanvas(CanvasTwo, roofHeight, roofWidth, widthTwo, st, secondList, si, scale);

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
                h[0] -= Math.Round((originalHeigh * si) / (originalWidth - si), 2);
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

            scaleStepper.ValueChanged += Stepper_ValueChanged;
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
                    WidthRequest = scale * 40,
                    Margin = new Thickness(2),
                };

                var heightLabel = new Label
                {
                    Text = height.ToString() + " ",
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = 18 * scale,
                };

                var columnWithLabel = new StackLayout
                {
                    Children = { column, heightLabel },
                    VerticalOptions = LayoutOptions.End,
                    HorizontalOptions = LayoutOptions.Center,
                };

                if(Table == "One") { flexLayoutOne.Children.Add(columnWithLabel);} else
                {
                    flexLayoutTwo.Children.Add(columnWithLabel);
                }
            }
        }

    }
}
