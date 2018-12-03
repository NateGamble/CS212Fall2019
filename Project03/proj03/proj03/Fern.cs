using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FernNamespace
{
    /*
     * this class draws a fractal fern when the constructor is called.
     * Written as sample C# code for a CS 212 assignment -- October 2011.
     * 
     * Bugs: WPF and shape objects are the wrong tool for the task 
     */   
    class Fern
    {
        private static int BERRYMIN = 20;
        private static int TENDRILS = 3;
        private static int TENDRILMIN = 10;
        private static double DELTATHETA = 0.025;
        private static double SEGLENGTH = 2.0;

        /* 
         * Fern constructor erases screen and draws a fern
         * 
         * Size: number of 3-pixel segments of tendrils
         * Redux: how much smaller children clusters are compared to parents
         * Turnbias: how likely to turn right vs. left (0=always left, 0.5 = 50/50, 1.0 = always right)
         * canvas: the canvas that the fern will be drawn on
         */
        public Fern(double size, double redux, double turnbias, double background, Canvas canvas)
        {
            canvas.Children.Clear();                                // delete old canvas contents
            // draw a new fern at the center of the canvas with given parameters
            ImageBrush ib = new ImageBrush();
            if (background > 0.5)
            {
                Random random = new Random();
                int choice = random.Next(0, 3);
                if (choice == 0)
                    ib.ImageSource = new BitmapImage(new Uri(".../.../forestBackground1.jpeg", UriKind.Relative));
                else if (choice == 1)
                    ib.ImageSource = new BitmapImage(new Uri(".../.../forestBackground2.jpg", UriKind.Relative));
                else
                    ib.ImageSource = new BitmapImage(new Uri(".../.../forestBackground3.png", UriKind.Relative));
            }
            else
            {
                ib.ImageSource = new BitmapImage(new Uri(".../.../white.jpg", UriKind.Relative));
            }
            canvas.Background = ib;
            cluster((int) (canvas.Width / 2), (int)(canvas.Height * 3 / 4), size * 1.5, redux, turnbias + .075, canvas, 0);       
        }

        /*
         * cluster draws a cluster at the given location and then draws a bunch of tendrils out in 
         * regularly-spaced directions out of the cluster.
         */
        private void cluster(double x, double y, double size, double redux, double turnbias, Canvas canvas, double orientation)
        {
            for (int i = 0; i < TENDRILS; i++)
            {
                // compute the angle of the outgoing tendril
                double theta = ((i * Math.PI / TENDRILS) + orientation) + (Math.PI * 3 / 4);
                tendril(x, y, size, redux, turnbias, theta, canvas, orientation + (i - 1) * (Math.PI / 3) );
                if (size > BERRYMIN)
                    berry(x, y, 5, canvas);
            }
        }

        /*
         * tendril draws a tendril (a randomly-wavy line) in the given direction, for the given length, 
         * and draws a cluster at the other end if the line is big enough.
         */
        private void tendril(double x1, double y1, double size, double redux, double turnbias, double direction, Canvas canvas, double orientation)
        {
            double x2 =x1, y2=y1;
            Random random = new Random();

            for (int i = 0; i < size; i++)
            {
                direction += (random.NextDouble() > turnbias) ? -1 * DELTATHETA : DELTATHETA;
                x1 = x2; y1 = y2;
                x2 = x1 + (double)(SEGLENGTH * Math.Sin(direction));
                y2 = y1 + (double)(SEGLENGTH * Math.Cos(direction));
                byte red = (byte) (50+size);
                byte green = (byte)(250 - size);
                line(x1, y1, x2, y2, red, green, 0, 1+size/80, canvas);
                if ((i %  Math.Ceiling(size/5)) == Math.Floor(size/5) - 1 & i != Math.Floor(size / 5))
                    cluster(x2, y2, size / redux - (i / 5), redux, turnbias, canvas, orientation);
            }
        }

        /*
         * draw a circle centered at (x,y), radius 7 * radius, with a black edge, and filled with a picture of Prof. Plantinga onto canvas
         */
        private void berry(double x, double y, double radius, Canvas canvas)
        {
            Ellipse myEllipse = new Ellipse();
            myEllipse.StrokeThickness = 1;
            myEllipse.Stroke = Brushes.Black;
            myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            myEllipse.VerticalAlignment = VerticalAlignment.Center;
            myEllipse.Width = 7 * radius;
            myEllipse.Height = 7 * radius;
            myEllipse.SetCenter(x, y);

            ImageBrush ib = new ImageBrush();
            Random random = new Random();
            int choice = random.Next(0, 3);
            if (choice == 0)
                ib.ImageSource = new BitmapImage(new Uri(".../.../PlantingaNormal.png", UriKind.Relative));
            else if (choice == 1)
                ib.ImageSource = new BitmapImage(new Uri(".../.../PlantingaRiver.png", UriKind.Relative));
            else if (choice == 2)
                ib.ImageSource = new BitmapImage(new Uri(".../.../PlantingaYoung.png", UriKind.Relative));
            myEllipse.Fill = ib;
            canvas.Children.Add(myEllipse);

        }

        /*
         * draw a line segment (x1,y1) to (x2,y2) with given color, thickness on canvas
         */
            private void line(double x1, double y1, double x2, double y2, byte r, byte g, byte b, double thickness, Canvas canvas)
        {
            Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, r, g, b);
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y2;
            myLine.Stroke = mySolidColorBrush;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.StrokeThickness = thickness;
            canvas.Children.Add(myLine);
        }
    }
}

/*
 * this class is needed to enable us to set the center for an ellipse (not built in?!)
 */
public static class EllipseX
{
    public static void SetCenter(this Ellipse ellipse, double X, double Y)
    {
        Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
        Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
    }
}

