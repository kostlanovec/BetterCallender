using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Diagnostics;

namespace BetterCallender
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel ViewModel { get; }

        public ImageSource IconSource { get; set; }
        private ScrollView masterStackLayout;
        private ScrollView labelScrollView;
        private ScrollView scrollView;
        private StackLayout stackLayout;
        private bool isScrollable = false;

        public class MainPageViewModel : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;

            public ObservableCollection<string> Items { get; set; }

            public ObservableCollection<DayViewModel> Days { get; set; }

            public ICommand LayoutWhiteCommand { get; set; }
            public ICommand OrangePlusCommand { get; set; }
            public ICommand OnButtonTappedCommand { get; set; }

            public ICommand LupaOrangeAgainCommand { get; set; }

            private bool _isWhiteLayout = true;
            public bool IsWhiteLayout
            {
                get { return _isWhiteLayout; }
                set
                {
                    _isWhiteLayout = value;
                    OnPropertyChanged();
                    UpdateIcon();
                }
            }

            private string _iconSource;
            public string IconSource
            {
                get { return _iconSource; }
                set
                {
                    _iconSource = value;
                    OnPropertyChanged();
                }
            }

            private string _currentMonthYear;
            public string CurrentMonthYear
            {
                get { return _currentMonthYear; }
                set
                {
                    _currentMonthYear = value;
                    OnPropertyChanged();
                }
            }

            private string _currentYear;
            public string CurrentYear
            {
                get { return _currentYear; }
                set
                {
                    _currentYear = value;
                    OnPropertyChanged();
                }
            }
            private MainPage _mainPage;
            public MainPageViewModel(MainPage mainPage, int year, int selectedMonth)
            {
                _mainPage = mainPage;
                CurrentMonthYear = new DateTime(year, selectedMonth, 1).ToString("MMMM yyyy");
                CurrentYear = year.ToString();
                Items = new ObservableCollection<string>();

                Days = new ObservableCollection<DayViewModel>();
                for (int i = 1; i <= DateTime.DaysInMonth(year, selectedMonth); i++)
                {
                    var day = new DateTime(year, selectedMonth, i);
                    var dayViewModel = new DayViewModel(day);
                    Days.Add(dayViewModel);
                }

                LayoutWhiteCommand = new Command(ExecuteLayoutWhiteCommand);
                OrangePlusCommand = new Command(ExecuteOrangePlusCommand);
                OnButtonTappedCommand = new Command(ExecuteOnButtonTappedCommand);
                LupaOrangeAgainCommand = new Command(ExecuteLupaOrangeAgainCommand);

                IconSource = "layoutwhite.png";
            }

            public void UpdateIcon()
            {
                IconSource = IsWhiteLayout ? "layoutwhite.png" : "layoutblack.png";
            }

            public void ExecuteLayoutWhiteCommand()
            {
                _mainPage.ChangeDisplayMode();
            }

            private async void ExecuteLupaOrangeAgainCommand()
            {
            }

            public void ExecuteOrangePlusCommand()
            {
                OnPropertyChanged(nameof(OrangePlusCommand));
                Items.Add("New Item");
            }

            private void ExecuteOnButtonTappedCommand()
            {
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class DayViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public DateTime Date { get; }

            private bool _isSelected;
            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(BackgroundColor));
                    OnPropertyChanged(nameof(TextColor));
                }
            }

            public Color BackgroundColor
            {
                get { return IsSelected ? Colors.Orange : Colors.Transparent; }
            }

            public Color TextColor
            {
                get { return IsSelected ? Colors.White : Colors.Black; }
            }

            public DayViewModel(DateTime date)
            {
                Date = date;
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public MainPage(int year, int selectedMonth)
        {

            InitializeComponent();

            ViewModel = new MainPageViewModel(this, year, selectedMonth);
            var existingContent = Content;

            stackLayout = new StackLayout(); // Initialize the private field

            stackLayout.Children.Add(existingContent);

            var autoGeneratedGrid = new AutoGeneratedGrid(new DateTime(year, selectedMonth, 1), ViewModel);
            stackLayout.Children.Add(autoGeneratedGrid);

            masterStackLayout = CreateLabelScrollView();
            stackLayout.Children.Add(masterStackLayout);

            Content = stackLayout;

            var buttonGrid = CreateLayout();
            stackLayout.Children.Add(buttonGrid);

            labelScrollView = CreateLabelScrollView();

            BindingContext = ViewModel;
            ViewModel.Days.CollectionChanged += ViewModel_CollectionChanged;

        }

        private void ViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var labelStackLayout = CreateLabelStackLayout(e.NewItems[0] as DayViewModel);
                var existingContent = masterStackLayout.Content as StackLayout;

                if (existingContent != null)
                {
                    existingContent.Children.Add(labelStackLayout);
                }
                else
                {
                    var newContent = new StackLayout();
                    newContent.Children.Add(labelStackLayout);
                    masterStackLayout.Content = newContent;
                }
            }
        }

        public class AutoGeneratedGrid : Grid
        {
            public event EventHandler ScrollToRequested;

            public DateTime Date { get; }

            private MainPageViewModel ViewModel { get; }

            public AutoGeneratedGrid(DateTime date, MainPageViewModel viewModel)
            {
                Date = date;
                ViewModel = viewModel;
                var currentDate = date;

                var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var startingDayOfWeek = (int)(firstDayOfMonth.DayOfWeek + 6) % 7;
                var rowDefinitions = new RowDefinitionCollection();
                var weeksInMonth = (int)Math.Ceiling((daysInMonth + startingDayOfWeek) / 7.0);
                rowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                for (int i = 0; i < daysInMonth; i++)
                {
                    rowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }

                RowDefinitions = rowDefinitions;

                var columnDefinitions = new ColumnDefinitionCollection();
                for (int i = 0; i < 7; i++)
                {
                    columnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }

                ColumnDefinitions = columnDefinitions;

                for (int i = 0; i < daysInMonth; i++)
                {
                    Debug.WriteLine($"Index: {i}, DayViewModel: {ViewModel.Days[i].ToString()}");
                    var day = i + 1;
                    var dayLabel = new Label
                    {
                        Text = day.ToString(),

                        //stará technika
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Padding = new Thickness(0, 20, 0, 20)
                    };
                    var column = (i + startingDayOfWeek) % 7;

                    if (column >= 5)
                    {
                        dayLabel.TextColor = Colors.Gray;
                    }
                    else
                    {

                        dayLabel.TextColor = Color.FromArgb("#080808");
                    }

                    dayLabel.SetValue(Grid.RowProperty, (i + startingDayOfWeek) / 7 + 1);
                    dayLabel.SetValue(Grid.ColumnProperty, (i + startingDayOfWeek) % 7);
                    Debug.WriteLine(i);
                    var dayViewModel = ViewModel.Days[i];
                    dayLabel.SetBinding(Label.BackgroundColorProperty, new Binding("BackgroundColor", source: dayViewModel));
                    dayLabel.SetBinding(Label.TextColorProperty, new Binding("TextColor", source: dayViewModel));
                    dayLabel.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(() =>
                        {
                            foreach (var d in ViewModel.Days)
                            {
                                d.IsSelected = false;
                            }
                            dayViewModel.IsSelected = true;
                        })
                    });

                    Children.Add(dayLabel);
                }

                if (currentDate.Month == DateTime.Now.Month && currentDate.Year == DateTime.Now.Year)
                {
                    var currentDay = DateTime.Now.Day;
                    var frame = new Frame
                    {
                        BackgroundColor = Colors.Black,
                        CornerRadius = 12,
                        WidthRequest = 25,
                        HeightRequest = 25,
                        Padding = 0,
                        Margin = 0,
                        HasShadow = false
                    };
                    frame.SetValue(Grid.RowProperty, (currentDay + startingDayOfWeek - 1) / 7 + 1);
                    frame.SetValue(Grid.ColumnProperty, (currentDay + startingDayOfWeek - 1) % 7);

                    var label = new Label
                    {
                        Text = currentDay.ToString(),
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };

                    frame.Content = label;
                    Children.Add(frame);
                }

                for (int i = 0; i < weeksInMonth; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        var boxView = new BoxView
                        {
                            BackgroundColor = Colors.Black,
                            HeightRequest = 1,
                            Margin = new Thickness(0, 0, 0, -50)
                        };

                        boxView.SetValue(Grid.RowProperty, i + 1);
                        boxView.SetValue(Grid.ColumnProperty, 0);
                        boxView.SetValue(Grid.ColumnSpanProperty, 7);

                        Children.Add(boxView);
                    }
                }

                this.SizeChanged += (s, e) =>
                {
                    if (this.Height > 0)
                    {
                        ScrollToRequested?.Invoke(this, EventArgs.Empty);
                    }
                };
            }
        }

        private StackLayout CreateLabelStackLayout(DayViewModel dayViewModel)
        {
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(0, 0, 0, 0)
            };

            var boxView = new BoxView
            {
                WidthRequest = 5,
                HeightRequest = 20,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 10, 10, 10),
                Color = Colors.Purple,
                CornerRadius = 2
            };
            stackLayout.Children.Add(boxView);

            var label = new Label
            {
                Text = "Velikonoční pondělí",
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.StartAndExpand
            };

            var subStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 10
            };

            var allDayLabel = new Label
            {
                Text = "all-day",
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.End
            };
            subStackLayout.Children.Add(allDayLabel);

            stackLayout.Children.Add(label);
            stackLayout.Children.Add(subStackLayout);
            stackLayout.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    Console.WriteLine("Label byl klepnut");
                })
            });

            return stackLayout;
        }


        private ScrollView CreateLabelScrollView()
        {
            var masterStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical
            };

            var stackLayout = new StackLayout
            {
                Padding = new Thickness(0, 0, 0, 0),
                Orientation = StackOrientation.Horizontal
            };

            var boxView = new BoxView
            {
                WidthRequest = 5,
                HeightRequest = 20,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 10, 10, 10),
                Color = Colors.Purple,
                CornerRadius = 2
            };
            stackLayout.Children.Add(boxView);

            var label = new Label
            {
                Text = "Velikonoční pondělí",
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            stackLayout.Children.Add(label);

            var subStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 10
            };

            var allDayLabel = new Label
            {
                Text = "all-day",
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.End
            };
            subStackLayout.Children.Add(allDayLabel);

            stackLayout.Children.Add(subStackLayout);
            stackLayout.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    Console.WriteLine("Label byl klepnut");
                })
            });

            masterStackLayout.Children.Add(stackLayout);

            var scrollView = new ScrollView
            {
                Content = masterStackLayout,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            return scrollView;
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
                    if (ViewModel.CurrentYear != visibleYear)
                    {
                        ViewModel.CurrentYear = visibleYear;
                        break;
                    }
                }
            }
        }
        private void GenerateCalendarGrid(DateTime date, bool prepend = false)
        {

            if (date.Month <= 0 || date.Month > 12)
                return;

            var calendarGrid = new AutoGeneratedGrid(date, ViewModel);
            calendarGrid.ScrollToRequested += (s, e) =>
            {
                ViewModel.CurrentYear = calendarGrid.Date.Year.ToString();
            };

            var monthLabel = new Label
            {
                Text = date.ToString("MMM"),
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            ViewModel.CurrentYear = date.Year.ToString();

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



        private void GenerateInitialCalendarGrids()
        {
            var currentDate = DateTime.Now;
            var initialDate = currentDate.AddMonths(-1);

            GenerateCalendarGrid(initialDate, true);
            GenerateCalendarGrid(currentDate);
        }

        public void ChangeDisplayMode()
        {
            if (isScrollable)
            {
                isScrollable = false;
                stackLayout.Children.Clear();
                var buttonGrid = CreateLayout();
                stackLayout.Children.Add(buttonGrid);
                GenerateInitialCalendarGrids();
            }
            else
            {
                isScrollable = true;
                stackLayout.Children.Clear();
                GenerateScrollableCalendarGrids();
            }
        }
        private void GenerateScrollableCalendarGrids()
        {
            var currentDate = DateTime.Now;
            var initialDate = currentDate.AddMonths(-1);

            for (int i = -1; i <= 1; i++)
            {
                var date = initialDate.AddMonths(i);
                GenerateCalendarGrid(DateTime.Now, true);
            }
        }
    }
}