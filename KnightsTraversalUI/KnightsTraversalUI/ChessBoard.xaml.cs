using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KnightsTraversalUI
{
    public partial class ChessBoard : ContentPage
    {
        private const int RowCount = 8;
        private const int ColCount = 8;
        private List<int[]> selection;
        private int selector;
        private PathFinder traversal;
        private bool FoundPath = false;
        public ChessBoard()
        {
            InitializeComponent();
            CreateBoard();
            selection = new List<int[]>();
            selection.Add(new int[] { -1, -1 });
            selection.Add(new int[] { -1, -1 });
            selector = 0;

            traversal = new PathFinder(selection[0], selection[1], 8);

        }

        private void CreateBoard()
        {
            for (int i = 0; i < RowCount; i++)
            {
                SquaresGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                SquaresGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            SquaresGrid.ColumnSpacing = 0;
            SquaresGrid.RowSpacing = 0;


            for (var row = 0; row < RowCount; row++)
            {
                var isBlack = row % 2 == 1;
                for (int col = 0; col < ColCount; col++)
                {
                    var button = new Button { BackgroundColor = !isBlack ? Color.White : Color.Black };
                    button.Clicked += Cell_Selected;
                    SquaresGrid.Children.Add(button, col, row);
                    isBlack = !isBlack;
                }
            }

            SquaresGrid.IsEnabled = false;
        }


        private void Button_Clicked(object sender, EventArgs e)
        {
            if (FoundPath)
            {
                Clear(this, null);
                FoundPath = false;
            }

            SquaresGrid.IsEnabled = true;
            var button = (Button)sender;
            UserInfo.Text = $"Select {button.Text} Position";
            selector = button.Text == "Start" ? 0 : 1;

        }

        private void Cell_Selected(object sender, EventArgs e)
        {
            SquaresGrid.IsEnabled = false;
            var button = (Button)sender;

            var row = Grid.GetRow(button);
            var col = Grid.GetColumn(button);

            setPosition(row, col, button);
        }

        private void setPosition(int row, int col, Button currElement)
        {
            var current = selection[selector];
            if (current[0] != -1 || current[1] != -1)
            {
                foreach (var view in SquaresGrid.Children)
                {
                    if ((current[1] == Grid.GetColumn(view)) && (current[0] == Grid.GetRow(view)))
                    {
                        var button = view as Button;
                        button.Text = "";
                        break;
                    }
                }
            }

            selection[selector] = new int[] { row, col };
            currElement.Text = selector == 0 ? "S" : "E";
            currElement.TextColor = Color.DarkGray;
        }

        private void ShortestPath(object sender, EventArgs e)
        {
            traversal.start = selection[0];
            traversal.dest = selection[1];

            var path = traversal.BuildPath();

            if (path == null)
            {
                UserInfo.Text = "Please select both start and end position";
                return;
            }

            UserInfo.Text = "Finding Shortest Path For:";
            Start.Text = $"Start: {selection[0][0]}, {selection[0][1]}";
            End.Text = $"End: {selection[1][0]}, {selection[1][1]}";

            SetPath(path);
        }

        private void SetPath(List<int[]> paths)
        {
            FoundPath = true;
            Path.Text = "Path: ";
            int i = 0;
            foreach (var path in paths)
            {
                foreach (var view in SquaresGrid.Children)
                {
                    if ((path[1] == Grid.GetColumn(view)) && (path[0] == Grid.GetRow(view)))
                    {
                        var button = view as Button;
                        button.Text = $"{i}";
                        button.TextColor = Color.OrangeRed;
                        button.BorderColor = Color.Blue;
                        button.BorderWidth = 5;
                        break;
                    }
                }

                Path.Text += $" {path[0]},{ path[1]} ";
                i++;
            }
        }

        private void Clear(Object sender, EventArgs e)
        {
            foreach (var view in SquaresGrid.Children)
            {
                var button = view as Button;
                button.Text = "";
                button.BorderColor = Color.Default;
                button.BorderWidth = 0;
            }

            UserInfo.Text = "Board Cleard";
            Start.Text = "";
            End.Text = "";
            Path.Text = "";

            selection[0] = new int[] { -1, -1 };
            selection[1] = new int[] { -1, -1 };
        }
    }
}
