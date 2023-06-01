using Microsoft.Maui.Layouts;
using System.ComponentModel;

namespace BetterCallender
{
    public partial class ScrollableCalendarPage : ContentPage, INotifyPropertyChanged
    {
        public MainPage.MainPageViewModel ViewModel { get; }
        private ScrollView scrollView;
        private StackLayout stackLayout;

        private string _currentYear;

        public string CurrentYear
        {
            get => _currentYear;
            set
            {
                if (_currentYear != value)
                {
                    _currentYear = value;
                    OnPropertyChanged(nameof(CurrentYear));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ScrollableCalendarPage()
        {
            InitializeComponent();

            BindingContext = ViewModel;

            stackLayout = new StackLayout();
            var buttonGrid = CreateLayout();
            stackLayout.Children.Add(buttonGrid);
            scrollView = new ScrollView();
            scrollView.Scrolled += ScrollView_Scrolled;

            GenerateInitialCalendarGrids();

            scrollView.Content = stackLayout;

            Content = scrollView;
        }

        public AbsoluteLayout CreateLayout()
        {
            Button btnToday = new Button { Text = "Today", TextColor = Color.FromArgb("#fc3e34"), BackgroundColor = Colors.Transparent };
            Button btnCalendars = new Button { Text = "Calendars", TextColor = Color.FromArgb("#fc3e34"), BackgroundColor = Colors.Transparent };
            Button btnInbox = new Button { Text = "Inbox", TextColor = Color.FromArgb("#fc3e34"), BackgroundColor = Colors.Transparent };
            ContentView spacer1 = new ContentView { BackgroundColor = Colors.Transparent };
            ContentView spacer2 = new ContentView { BackgroundColor = Colors.Transparent };
            Grid gridovina = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 1 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 1 },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star }
                },
                BackgroundColor = Colors.WhiteSmoke
            };
            gridovina.Children.Add(btnToday);
            Grid.SetColumn(btnToday, 0);

            gridovina.Children.Add(spacer1);
            Grid.SetColumn(spacer1, 1);

            gridovina.Children.Add(btnCalendars);
            Grid.SetColumn(btnCalendars, 2);

            gridovina.Children.Add(spacer2);
            Grid.SetColumn(spacer2, 3);

            gridovina.Children.Add(btnInbox);
            Grid.SetColumn(btnInbox, 4);

            Grid grid = new Grid { };
            AbsoluteLayout absoluteLayout = new AbsoluteLayout { };
            absoluteLayout.Children.Add(grid);
            absoluteLayout.Children.Add(gridovina);
            AbsoluteLayout.SetLayoutBounds(gridovina, new Rect(0, 1, 1, -1));
            AbsoluteLayout.SetLayoutFlags(gridovina, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
            return absoluteLayout;
        }

        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            var scrollViewHeight = scrollView.Height;
            var scrollViewContentHeight = scrollView.ContentSize.Height;
            var scrollY = scrollView.ScrollY;
            AutoGeneratedGrid currentGrid = null;

            if (scrollY <= 0)
            {
                currentGrid = stackLayout.Children.FirstOrDefault(c => c is AutoGeneratedGrid) as AutoGeneratedGrid;
                if (currentGrid != null)
                {
                    var newDate = currentGrid.Date.AddMonths(-1);
                    GenerateCalendarGrid(newDate, true);
                }
            }
            else if (scrollY + scrollViewHeight >= scrollViewContentHeight)
            {
                currentGrid = stackLayout.Children.LastOrDefault(c => c is AutoGeneratedGrid) as AutoGeneratedGrid;
                if (currentGrid != null)
                {
                    var newDate = currentGrid.Date.AddMonths(1);
                    GenerateCalendarGrid(newDate);
                }
            }

            var visibleGrids = stackLayout.Children.Where(c => c is AutoGeneratedGrid).Cast<AutoGeneratedGrid>();
            foreach (var grid in visibleGrids)
            {
                var gridPosition = grid.Y;
                var gridHeight = grid.Height;
                if (scrollY < gridPosition + gridHeight && scrollY + scrollViewHeight > gridPosition)
                {
                    var visibleYear = grid.Date.Year.ToString();
                    if (CurrentYear != visibleYear)
                    {
                        CurrentYear = visibleYear;
                        break;
                    }
                }
            }
        }

        private void GenerateInitialCalendarGrids()
        {
            var currentDate = DateTime.Now;
            var initialDate = currentDate.AddMonths(-1);

            GenerateCalendarGrid(initialDate, true);
            GenerateCalendarGrid(currentDate);
        }

        private void GenerateCalendarGrid(DateTime date, bool prepend = false)
        {
            var calendarGrid = new AutoGeneratedGrid(date, ViewModel);
            calendarGrid.ScrollToRequested += (s, e) =>
            {
                CurrentYear = calendarGrid.Date.Year.ToString();
            };

            var monthLabel = new Label
            {
                Text = date.ToString("MMM"),
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            CurrentYear = date.Year.ToString();

            if (prepend)
            {
                stackLayout.Children.Insert(0, calendarGrid);
                stackLayout.Children.Insert(0, monthLabel);
            }
            else
            {
                stackLayout.Children.Add(monthLabel);
                stackLayout.Children.Add(calendarGrid);
            }
        }

        public class AutoGeneratedGrid : MainPage.AutoGeneratedGrid
        {
            public DateTime Date { get; }

            public AutoGeneratedGrid(DateTime date, MainPage.MainPageViewModel viewModel) : base(date, viewModel)
            {
                Date = date;
            }
        }

    }
}