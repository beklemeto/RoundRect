using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Drawing
{
    /// <summary>
    /// A simple helper class to draw pixel-perfect symmetric rectangles with rounded corners.
    /// Adapted from https://www.codeproject.com/Articles/27228/A-class-for-creating-round-rectangles-in-GDI-with
    /// and ported from C++ to C#.NET
    /// by Jonas Kohl [http://jonaskohl.de/]
    /// </summary>

    static class RoundRect
    {
        /// <summary>
        /// Generate the path of a rounded rectangle
        /// </summary>
        /// <param name="r">The rectangle to use</param>
        /// <param name="dia">The diameter</param>
        /// <returns>The GraphicsPath which resembles a rounded rectangle</returns>
        public static void AddRoundRect(this GraphicsPath pPath, Rectangle r, int radius)
        {
            int dia = 2 * radius;

            // diameter can't exceed width or height
            if (dia > r.Width) dia = r.Width;
            if (dia > r.Height) dia = r.Height;

            // define a corner 
            Rectangle Corner = new Rectangle(r.X, r.Y, dia, dia);

            //// begin path
            //pPath.Reset();

            // top left
            pPath.AddArc(Corner, 180, 90);

            // tweak needed for radius of 10 (dia of 20)
            if (dia == 20)
            {
                Corner.Width += 1;
                Corner.Height += 1;
                r.Width -= 1; r.Height -= 1;
            }

            // top right
            Corner.X += (r.Width - dia - 1);
            pPath.AddArc(Corner, 270, 90);

            // bottom right
            Corner.Y += (r.Height - dia - 1);
            pPath.AddArc(Corner, 0, 90);

            // bottom left
            Corner.X -= (r.Width - dia - 1);
            pPath.AddArc(Corner, 90, 90);

            // end path
            pPath.CloseFigure();
        }
        
        /// <summary>
        /// Draw (outline) a rounded rectangle to a graphics context.
        /// </summary>
        /// <param name="pGraphics">The graphics context</param>
        /// <param name="r">The bounding rectangle</param>
        /// <param name="color">The color of the outline</param>
        /// <param name="radius">The corner radius</param>
        /// <param name="width">The width of the outline</param>
        public static void DrawRoundRect(this System.Drawing.Graphics pGraphics, Pen pen, Rectangle r, int radius)
        {
            int dia = 2 * radius;
            int width = (int)pen.Width;

            // store old page unit
            GraphicsUnit oldPageUnit = pGraphics.PageUnit;

            // set to pixel mode
            pGraphics.PageUnit = GraphicsUnit.Pixel;
            
            // set pen alignment
            var penAlignment = pen.Alignment;
            pen.Alignment = PenAlignment.Center;

            // get the corner path
            using (GraphicsPath path = new GraphicsPath())
            {
                // draw the round rect
                pGraphics.DrawPath(pen, path);

                // if width > 1
                for (int i = 1; i < width; i++)
                {
                    // left stroke
                    r.Inflate(-1, 0);

                    // get the path
                    path.Reset();
                    path.AddRoundRect(r, dia);

                    // draw the round rect
                    pGraphics.DrawPath(pen, path);

                    // up stroke
                    r.Inflate(0, -1);

                    // get the path
                    path.Reset();
                    path.AddRoundRect(r, dia);

                    // draw the round rect
                    pGraphics.DrawPath(pen, path);
                }
            }

            // restore
            pGraphics.PageUnit = oldPageUnit;
            pen.Alignment = penAlignment;
        }

        /// <summary>
        /// Fill a rounded rectangle to a graphics context.
        /// </summary>
        /// <param name="pGraphics">The graphics context</param>
        /// <param name="pBrush">The brush to fill the rectangle with</param>
        /// <param name="r">The bounding rectangle</param>
        /// <param name="border">The outline (border) color</param>
        /// <param name="radius">The corner radius</param>
        public static void FillRoundRect(this System.Drawing.Graphics pGraphics, Brush pBrush, Rectangle r, int radius)
        {
            int dia = 2 * radius;

            // store old page unit
            GraphicsUnit oldPageUnit = pGraphics.PageUnit;

            // set to pixel mode
            pGraphics.PageUnit = GraphicsUnit.Pixel;


            // get the corner path
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddRoundRect(r, dia);

                // fill
                pGraphics.FillPath(pBrush, path);
            }

            // restore page unit
            pGraphics.PageUnit = oldPageUnit;
        }
    }
}
