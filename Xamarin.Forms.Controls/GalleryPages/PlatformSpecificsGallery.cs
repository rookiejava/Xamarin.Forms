using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;

namespace Xamarin.Forms.Controls
{
	public class PlatformSpecificsGallery : ContentPage
	{
		Page _originalRoot;

		public PlatformSpecificsGallery()
		{
			var mdpButton = new Button { Text = "Master Detail Page" };

			mdpButton.Clicked += (sender, args) => { SetRoot(CreateMdpPage()); };

			Content = new StackLayout
			{
				Children = { mdpButton }
			};
		}

		void SetRoot(Page page)
		{
			var app = Application.Current as App;
			if (app == null)
			{
				return;
			}

			_originalRoot = app.MainPage;
			app.SetMainPage (page);
		}

		void RestoreOriginal()
		{
			if (_originalRoot == null)
			{
				return;
			}

			var app = Application.Current as App;
			app?.SetMainPage (_originalRoot);
		}

		public class NavItem
		{
			public NavItem(string text, string icon, ICommand command)
			{
				Text = text;
				Icon = icon;
				Command = command;
			}

			public string Text { get; set; }
			public string Icon { get; set; }
			public ICommand Command { get; set; }
		}

		public class NavList : ListView
		{
			public NavList(IEnumerable<NavItem> items)
			{
				ItemsSource = items;
				ItemTapped += (sender, args) => (args.Item as NavItem)?.Command.Execute(null);

				ItemTemplate = new DataTemplate(() =>
				{
					var grid = new Grid();
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 48 });
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = 200 });

					grid.Margin = new Thickness(0, 10, 0, 10);

					var text = new Label
					{
						VerticalOptions = LayoutOptions.Fill
					};
					text.SetBinding(Label.TextProperty, "Text");
					
					var glyph = new Label
					{
						FontFamily = "Segoe MDL2 Assets",
						FontSize = 24,
						HorizontalTextAlignment = TextAlignment.Center,
					};

					glyph.SetBinding(Label.TextProperty, "Icon");
					
					grid.Children.Add(glyph);
					grid.Children.Add(text);

					Grid.SetColumn(glyph, 0);
					Grid.SetColumn(text, 1);

					grid.WidthRequest = 48;

					var cell = new ViewCell
					{
						View = grid
					};

					return cell;
				});
			}
		}

		static Layout CreateCollapseStyleChanger(MasterDetailPage page)
		{
			var collapseStylePicker = new Picker();
			string[] collapseStyles = Enum.GetNames(typeof(CollapseStyle));
			foreach (string collapseStyle in collapseStyles)
			{
				collapseStylePicker.Items.Add(collapseStyle);
			}

			collapseStylePicker.SelectedIndex =
				collapseStyles.IndexOf(Enum.GetName(typeof(CollapseStyle), page.On<Windows>().GetCollapseStyle()));

			collapseStylePicker.SelectedIndexChanged += (sender, args) =>
			{
				page.On<Windows>()
					.SetCollapseStyle(
						(CollapseStyle)Enum.Parse(typeof(CollapseStyle), collapseStylePicker.Items[collapseStylePicker.SelectedIndex]));
			};

			var layout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Center,
				Orientation = StackOrientation.Horizontal,
				Children = { new Label { Text = "Select Collapse Style", VerticalOptions = LayoutOptions.Center }, collapseStylePicker }
			};

			return layout;
		}

		static Layout CreateCollapseWidthAdjuster(MasterDetailPage page)
		{
			var adjustCollapseWidthLabel = new Label() { Text = "Adjust Collapsed Width", VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center};
			var adjustCollapseWidthEntry = new Entry { Text = page.On<Windows>().CollapsedPaneWidth().ToString() }; 
			var adjustCollapseWidthButton = new Button { Text = "Change" };
			adjustCollapseWidthButton.Clicked += (sender, args) =>
			{
				double newWidth;
				if (double.TryParse(adjustCollapseWidthEntry.Text, out newWidth))
				{
					page.On<Windows>().CollapsedPaneWidth(newWidth);
				}
			};
			
			var adjustCollapsedWidthSection = new StackLayout()
			{
				HorizontalOptions = LayoutOptions.Center,
				Orientation = StackOrientation.Horizontal,
				Children = { adjustCollapseWidthLabel, adjustCollapseWidthEntry, adjustCollapseWidthButton}
			};

			return adjustCollapsedWidthSection;
		}

		MasterDetailPage CreateMdpPage()
		{
			var page = new MasterDetailPage();

			page.On<Windows>()
				.SetCollapseStyle(CollapseStyle.Partial);
			page.MasterBehavior = MasterBehavior.Popover;

			var master = new ContentPage { Title = "Master Detail Page" };
			var masterContent = new StackLayout { Spacing = 10, Margin = new Thickness(0, 10, 5, 0)};

			// Build the navigation pane items
			var navItems = new List<NavItem>
			{
				new NavItem("Display Alert", "\uE171", new Command(() => DisplayAlert("Alert", "This is an alert", "OK"))),
				new NavItem("Return To Gallery", "\uE106", new Command(RestoreOriginal)),
				new NavItem("Save", "\uE105", new Command(() => DisplayAlert("Save", "Fake save dialog", "OK"))),
				new NavItem("Audio", "\uE189", new Command(() => DisplayAlert("Audio", "Never gonna give you up...", "OK")))
			};

			var navList = new NavList(navItems);

			// And add them to the navigation pane's content
			masterContent.Children.Add(navList);
			master.Content = masterContent;

			var detail = new ContentPage { Title = "Detail" };
			var detailContent = new StackLayout { VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill };
			detailContent.Children.Add(new Label
			{ 
				HeightRequest = 200,
				Text = "Features",
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			});

			detailContent.Children.Add(CreateCollapseStyleChanger(page));
			detailContent.Children.Add(CreateCollapseWidthAdjuster(page));

			detail.Content = detailContent;

			page.Master = master;
			
			AddToolBarItems(page);

			page.Detail = detail;

			return page;
		}

		void AddToolBarItems(Page page)
		{
			Action action = () => DisplayAlert("Hey!", "Command Bar Item Clicked", "OK");

			var tb1 = new ToolbarItem("Primary 1", "coffee.png", action, ToolbarItemOrder.Primary)
			{
				IsEnabled = true,
				AutomationId = "toolbaritem_primary1"
			};

			var tb2 = new ToolbarItem("Primary 2", "coffee.png", action, ToolbarItemOrder.Primary)
			{
				IsEnabled = true,
				AutomationId = "toolbaritem_primary2"
			};

			var tb3 = new ToolbarItem("Seconday 1", "coffee.png", action, ToolbarItemOrder.Secondary)
			{
				IsEnabled = true,
				AutomationId = "toolbaritem_secondary3"
			};

			var tb4 = new ToolbarItem("Secondary 2", "coffee.png", action, ToolbarItemOrder.Secondary)
			{
				IsEnabled = true,
				AutomationId = "toolbaritem_secondary4"
			};

			page.ToolbarItems.Add(tb1);
			page.ToolbarItems.Add(tb2);
			page.ToolbarItems.Add(tb3);
			page.ToolbarItems.Add(tb4);
		}
	}
}