using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Data.SqlTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BetterCallender.View
{
    public partial class CalllendarList : ContentPage
    {
        ViewModel viewModel;
        StackLayout masterStackLayout;
        ScrollView scrollView;

        int currentYear;

        public CalllendarList()
        {
            InitializeComponent();

            viewModel = new ViewModel();
            viewModel.Months.CollectionChanged += ViewModel_CollectionChanged;

            masterStackLayout = new StackLayout();
            masterStackLayout.Spacing = 20;

            currentYear = DateTime.Now.Year;
            masterStackLayout.Children.Add(CreateCalendarLayoutByYear(currentYear));

            var buttonGrid = CreateLayout();

            var stackLayout = new StackLayout();
            stackLayout.Children.Add(masterStackLayout);

            scrollView = new ScrollView();
            scrollView.Content = stackLayout;
            scrollView.Scrolled += ScrollView_Scrolled;

            stackLayout.Children.Add(buttonGrid);

            Content = scrollView;
        }

        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            double scrollOffset = scrollView.ContentSize.Height - scrollView.Height;
            if (scrollOffset > 0 && e.ScrollY >= scrollOffset)
            {
                currentYear++;
                masterStackLayout.Children.Add(CreateCalendarLayoutByYear(currentYear));
            }
            else if (e.ScrollY <= 0)
            {
                currentYear--;
                masterStackLayout.Children.Insert(0, CreateCalendarLayoutByYear(currentYear));
                scrollView.ScrollToAsync(0, 200, false);
            }
        }

        private void ViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var newMonth = (Month)e.NewItems[0];
                var calendarGrid = CreateCalendarLayoutByYear(newMonth.Year);
                masterStackLayout.Children.Add(calendarGrid);
            }
        }

        public Grid CreateCalendarLayoutByYear(int year)
        {
            var calendarGrid = new Grid();
            calendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            calendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            calendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            calendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            calendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            calendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            calendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            calendarGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var yearLabel = new Label
            {
                Text = year.ToString(),
                TextColor = Colors.Black,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(20, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                FontSize = 24
            };

            calendarGrid.Children.Add(yearLabel);
            Grid.SetRow(yearLabel, 0);
            Grid.SetColumnSpan(yearLabel, 3);

            int rowNumber = 1;
            int colNumber = 0;

            for (int i = 0; i < 12; i++)
            {
                int monthIndex = i;
                var monthGrid = new Grid();
                monthGrid.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        var selectedMonth = monthIndex + 1;
                        Navigation.PushAsync(new MainPage(year, selectedMonth));
                    })
                });

                monthGrid.BackgroundColor = Colors.LightGray;
                monthGrid.Margin = new Thickness(10, 10, 10, 10);

                var monthLabel = new Label
                {
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i + 1),
                    TextColor = Colors.Orange,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    BackgroundColor = Colors.Transparent
                };

                Debug.WriteLine(monthLabel.Text);

                monthGrid.Children.Add(monthLabel);
                Grid.SetRow(monthLabel, 0);
                Grid.SetColumnSpan(monthLabel, 7);
                monthLabel.IsVisible = true;

                var daysGrid = new Grid();
                daysGrid.BackgroundColor = Colors.White;
                daysGrid.ColumnDefinitions = new ColumnDefinitionCollection();

                Debug.WriteLine(year);

                int daysInMonth = DateTime.DaysInMonth(year, i + 1);
                int startDay = (int)new DateTime(year, i + 1, 1).DayOfWeek;

                for (int j = 0; j < 7; j++)
                {
                    daysGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }

                int dayNumber = 1;
                for (int row = 0; row < 5; row++)
                {
                    daysGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

                    for (int col = 0; col < 7; col++)
                    {
                        if (row == 0 && col < startDay)
                            continue;

                        if (dayNumber > daysInMonth)
                            break;

                        var dayLabel = new Label
                        {
                            Text = dayNumber.ToString(),
                            TextColor = Colors.Black,
                            HorizontalOptions = LayoutOptions.Center
                        };

                        daysGrid.Children.Add(dayLabel);
                        Grid.SetRow(dayLabel, row);
                        Grid.SetColumn(dayLabel, col);

                        dayNumber++;
                    }
                }

                monthGrid.Children.Add(daysGrid);
                Grid.SetRow(daysGrid, 1);

                calendarGrid.Children.Add(monthGrid);
                Grid.SetColumn(monthGrid, colNumber);
                Grid.SetRow(monthGrid, rowNumber);

                colNumber++;
                if (colNumber >= 3)
                {
                    colNumber = 0;
                    rowNumber++;
                }
            }

            return calendarGrid;
        }
        public StackLayout CreateLayout()
        {
            var buttonToday = new Button { Text = "Today", TextColor = Color.FromArgb("#fc3e34"), BackgroundColor = Colors.Transparent };
            var buttonCalendars = new Button { Text = "Calendars", TextColor = Color.FromArgb("#fc3e34"), BackgroundColor = Colors.Transparent };
            var buttonInbox = new Button { Text = "Inbox", TextColor = Color.FromArgb("#fc3e34"), BackgroundColor = Colors.Transparent };
            var spacer1 = new ContentView { BackgroundColor = Colors.Transparent };
            var spacer2 = new ContentView { BackgroundColor = Colors.Transparent };

            var buttonGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star }
                },
                BackgroundColor = Colors.WhiteSmoke
            };

            buttonGrid.Children.Add(buttonToday);
            Grid.SetColumn(buttonToday, 0);

            buttonGrid.Children.Add(spacer1);
            Grid.SetColumn(spacer1, 1);

            buttonGrid.Children.Add(buttonCalendars);
            Grid.SetColumn(buttonCalendars, 2);

            buttonGrid.Children.Add(spacer2);
            Grid.SetColumn(spacer2, 3);

            buttonGrid.Children.Add(buttonInbox);
            Grid.SetColumn(buttonInbox, 4);

            var stackLayout = new StackLayout();
            stackLayout.Children.Add(buttonGrid);

            return stackLayout;
        }

        public class ViewModel : INotifyPropertyChanged
        {
            private ObservableCollection<Month> _months;
            public ObservableCollection<Month> Months
            {
                get => _months;
                set
                {
                    if (_months != value)
                    {
                        _months = value;
                        OnPropertyChanged();
                    }
                }
            }

            public ViewModel()
            {
                Months = new ObservableCollection<Month>();
                for (int i = 1; i <= 12; i++)
                {
                    Months.Add(new Month { Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i), Year = DateTime.Now.Year });
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class Month
        {
            public string Name { get; set; }
            public int Year { get; set; }
        }
    }
}