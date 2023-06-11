using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dothttp.Handlers
{
    internal class ColumnChart : Renderable
    {
        //
        // Summary:
        //     Gets the bar chart data.
        public List<double> Data { get; }

        //
        // Summary:
        //     Gets or sets the width of the bar chart.
        public int? Width { get; set; }

        public int Height { get; set; }

        //
        // Summary:
        //     Gets or sets the bar chart label.
        public string? Label { get; set; }

        //
        // Summary:
        //     Gets or sets the bar chart label alignment.
        public Justify? LabelAlignment { get; set; } = Justify.Center;

        public Color? FillColor { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether or not values should be shown next to
        //     each bar.
        public bool ShowValues { get; set; } = true;


        //
        // Summary:
        //     Gets or sets the culture that's used to format values.
        //
        // Remarks:
        //     Defaults to invariant culture.
        public CultureInfo? Culture { get; set; }

        //
        // Summary:
        //     Gets or sets the fixed max value for a bar chart.
        //
        // Remarks:
        //     Defaults to null, which corresponds to largest value in chart.
        public double? MaxValue { get; set; }

        //
        // Summary:
        //     Initializes a new instance of the Spectre.Console.BarChart class.
        public ColumnChart(int height)
        {
            Height = height;
            Data = new List<double>();
        }

        public ColumnChart AddValue(double val)
        {
            Data.Add(val);
            return this;
        }
        public ColumnChart SetValues(IEnumerable<double> values)
        {
            Data.Clear();
            Data.AddRange(values);
            return this;
        }

        public ColumnChart WithFillColor(Color color)
        {
            this.FillColor = color;
            return this;
        }

        protected override Measurement Measure(RenderOptions options, int maxWidth)
        {
            int num = Math.Min(Width ?? maxWidth, maxWidth);
            return new Measurement(num, num);
        }

        public Text CreateRow(int y, double maxValue, int columns)
        {
            var sb = new StringBuilder();
            double limit = maxValue * (Height-y-1) / (double)Height;
            int column = 0;

            var skip = Math.Max(0,Data.Count - columns);

            foreach(var datum in Data.Skip(skip))
            {
                if(datum >= limit)
                {
                    sb.Append('█');
                    //sb.Append(column%10);
                }
                else
                {
                    sb.Append(' ');
                }
                column++;
            }
            return new Text(sb.ToString(), new Style().Foreground(this.FillColor ?? Color.Default));
        }

        protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
        {
            int num = Math.Min(Width ?? maxWidth, maxWidth);
            var maxValue = Data.Any() ? Data.Max() : 0;
            //double maxValue = Math.Max(MaxValue.GetValueOrDefault(), Data.Max());
            Grid grid = new Grid();
            grid.Expand();
            grid.AddColumn(new GridColumn());
            grid.Width = num;
            if (!string.IsNullOrWhiteSpace(Label))
            {
                grid.AddRow(Text.Empty, new Markup(Label).Justify(LabelAlignment));
            }

            for(int y=0; y<Height; y++)
            {
                grid.AddRow(CreateRow(y, maxValue, num));
            }

            return ((IRenderable)grid).Render(options, num);
        }
    }
}
