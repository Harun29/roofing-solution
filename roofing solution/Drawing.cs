using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Platform.Compatibility;


namespace roofing_solution
{
    public class CustomDrawable : IDrawable
    {
        private float height;
        private float width;
        private float lastWidth;
        private float panelWidth;
        private List<double> forCount;

        public CustomDrawable(double height, double width, double lastWidth, double panelWidth, List<double> forCount)
        {
            this.height = Convert.ToSingle(height);
            this.width = Convert.ToSingle(width);
            this.lastWidth = Convert.ToSingle(lastWidth);
            this.panelWidth = Convert.ToSingle(panelWidth);
            this.forCount = forCount;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (canvas == null || forCount == null || forCount.Count == 0)
                return;

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 1;

            width *= 40;
            height *= 40;
            panelWidth *= 40;

            PathF path = new PathF();
            path.MoveTo(0, height);
            path.LineTo(width, height);
            path.LineTo(width / 2, 0);
            path.Close();
            canvas.DrawPath(path);

            float followingWidth = lastWidth * 40;
            int count = 0;
            foreach (double item in forCount)
            {
                canvas.StrokeDashPattern = new float[] { 2, 2 };
                float lineHeight = Convert.ToSingle(item) * 40;
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
