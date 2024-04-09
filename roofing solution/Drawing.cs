using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Platform.Compatibility;


namespace roofing_solution
{
    public class CustomDrawable(double height, double width, double lastWidth, double panelWidth, List<double> forCount, double cutOutWidth, double scaleForBigRoofs) : IDrawable
    {
        private float height = Convert.ToSingle(height);
        private float width = Convert.ToSingle(width);
        private readonly float lastWidth = Convert.ToSingle(lastWidth);
        private float panelWidth = Convert.ToSingle(panelWidth);
        private float cutOutWidth = Convert.ToSingle(cutOutWidth);
        private readonly List<double> forCount = forCount;
        private readonly float scale = Convert.ToSingle(scaleForBigRoofs);

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {

            if (canvas == null || forCount == null || forCount.Count == 0)
                return;

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;

            width *= 40 * scale;
            height *= 40 * scale;
            panelWidth *= 40 * scale;
            cutOutWidth *= 40 * scale;

            PathF path = new();
                path.MoveTo(0, height);
                path.LineTo(width, height);
            if(cutOutWidth == 0)
            {
                path.LineTo(width / 2, 0);
                path.Close();
            }
            else
            {
                path.LineTo((width / 2) + (cutOutWidth / 2), 0);
                path.LineTo((width / 2) - (cutOutWidth/2), 0);
                path.Close();
            }
            canvas.DrawPath(path);

            float followingWidth = lastWidth * 40 * scale;
            int count = 0;

            foreach (double item in forCount)
            {
                canvas.StrokeDashPattern = [2, 2];
                float lineHeight = Convert.ToSingle(item) * 40 * scale;
                if (lineHeight == height && count == 0)
                {
                    count++;
                    continue;
                }
                else if (lineHeight == height && count == 1)
                {
                    canvas.DrawLine(followingWidth, height, followingWidth, 0);
                    followingWidth += panelWidth;
                    continue;
                }
                canvas.DrawLine(followingWidth, height, followingWidth, height - lineHeight);
                followingWidth += panelWidth;
            }
        }
    }
}
