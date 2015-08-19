﻿using System;
using XamarinCRM.Pages.Base;
using Xamarin.Forms;
using XamarinCRM.Layouts;
using XamarinCRM.Statics;
using XamarinCRM.Converters;
using XamarinCRM.Pages.Products;
using XamarinCRM.ViewModels.Customers;
using XamarinCRM.Views.Base;

namespace XamarinCRM.Pages.Customers
{
    public class EditOrderPage : ModelTypedContentPage<OrderDetailViewModel>
    {
        public EditOrderPage()
        {
            // hide the back button, because we have ToolBarItems to control navigtion on this page
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetBackButtonTitle(this, string.Empty);

            #region header
            StackLayout headerStackLayout = new UnspacedStackLayout();

            Label companyTitleLabel = new Label()
            {
                Text = TextResources.Customers_Orders_EditOrder_CompanyTitle,
                TextColor = Palette._007,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                XAlign = TextAlignment.Start,
                YAlign = TextAlignment.End,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            
            Label companyNameLabel = new Label()
            {
                TextColor = Palette._006,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                XAlign = TextAlignment.Start,
                YAlign = TextAlignment.Start,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            companyNameLabel.SetBinding(Label.TextProperty, "Account.Company");

            RelativeLayout headerLabelsRelativeLayout = new RelativeLayout() { HeightRequest = Sizes.LargeRowHeight };

            headerLabelsRelativeLayout.Children.Add(
                view: companyTitleLabel,
                widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
                heightConstraint: Constraint.RelativeToParent(parent => parent.Height / 2));

            headerLabelsRelativeLayout.Children.Add(
                view: companyNameLabel,
                yConstraint: Constraint.RelativeToParent(parent => parent.Height / 2),
                widthConstraint: Constraint.RelativeToParent(parent => parent.Width),
                heightConstraint: Constraint.RelativeToParent(parent => parent.Height / 2));

            ContentView headerLabelsView = new ContentView() { Padding = new Thickness(20, 0), Content = headerLabelsRelativeLayout };

            headerStackLayout.Children.Add(new ContentViewWithBottomBorder() { Content = headerLabelsView });
            #endregion

            #region fields
            Grid section1Grid = new Grid()
            {
                Padding = new Thickness(20),
                RowSpacing = 15,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                }
            };

            Thickness fieldLabelThickness = new Thickness(0, 0, 5, 0);

            const double rowHeight = 30;

            ContentView productFieldLabelView = new ContentView()
            {
                HeightRequest = rowHeight,
                Padding = fieldLabelThickness,
                Content = new Label()
                {
                    Text = (Device.OS == TargetPlatform.Android) ? TextResources.Customers_Orders_EditOrder_ProductTitleLabel.ToUpper() : TextResources.Customers_Orders_EditOrder_ProductTitleLabel, 
                    XAlign = TextAlignment.End, 
                    YAlign = TextAlignment.Center,
                    TextColor = Device.OnPlatform(Palette._007, Palette._004, Palette._007),
                }
            };

            ContentView priceFieldLabelView = new ContentView()
            {
                HeightRequest = rowHeight,
                Padding = fieldLabelThickness,
                Content = new Label()
                { 
                    Text = (Device.OS == TargetPlatform.Android) ? TextResources.Customers_Orders_EditOrder_PriceTitleLabel.ToUpper() : TextResources.Customers_Orders_EditOrder_PriceTitleLabel, 
                    XAlign = TextAlignment.End, 
                    YAlign = TextAlignment.Center,
                    TextColor = Device.OnPlatform(Palette._007, Palette._004, Palette._007)
                }
            };
            
            ContentView dueDateFieldLabelView = new ContentView()
            {
                HeightRequest = rowHeight,
                Padding = fieldLabelThickness,
                Content = new Label()
                { 
                    Text = (Device.OS == TargetPlatform.Android) ? TextResources.Customers_Orders_EditOrder_DueDateTitleLabel.ToUpper() : TextResources.Customers_Orders_EditOrder_DueDateTitleLabel, 
                    XAlign = TextAlignment.End, 
                    YAlign = TextAlignment.Center,
                    TextColor = Device.OnPlatform(Palette._007, Palette._004, Palette._007)
                }
            };
            
            Entry productEntry = new Entry() { Placeholder = TextResources.Customers_Orders_EditOrder_ProductEntryPlaceholder };
            productEntry.SetBinding(Entry.TextProperty, "Order.Item", BindingMode.TwoWay);
            productEntry.Focused += async (sender, e) =>
            {
                // prevents the keyboard on Android from appearing over the modally presented product category list
                Device.OnPlatform(Android: productEntry.Unfocus);

                NavigationPage navPage = new NavigationPage(new CategoryListPage(null, null, true)
                    { 
                        Title = TextResources.MainTabs_Products
                    });
                navPage.ToolbarItems.Add(new ToolbarItem(TextResources.Cancel, null, () => Navigation.PopModalAsync()));
                await ViewModel.PushModalAsync(navPage);
            };

            Entry priceEntry = new Entry() { Placeholder = TextResources.Customers_Orders_EditOrder_PriceEntryPlaceholder };
            priceEntry.SetBinding(Entry.TextProperty, "Order.Price", BindingMode.TwoWay, new CurrencyDoubleConverter());

            DatePicker dueDateEntry = new DatePicker() { Date = DateTime.Now };
            dueDateEntry.SetBinding(DatePicker.DateProperty, "Order.DueDate", BindingMode.TwoWay);

            section1Grid.Children.Add(productFieldLabelView, 0, 0);
            section1Grid.Children.Add(priceFieldLabelView, 0, 1);
            section1Grid.Children.Add(dueDateFieldLabelView, 0, 2);
            section1Grid.Children.Add(productEntry, 1, 0);
            section1Grid.Children.Add(priceEntry, 1, 1);
            section1Grid.Children.Add(dueDateEntry, 1, 2);

            #endregion

            StackLayout stackLayout = new UnspacedStackLayout();
            stackLayout.Children.Add(headerStackLayout);
            stackLayout.Children.Add(new ContentViewWithBottomBorder() { Content = section1Grid });

            Content = stackLayout;

            ToolbarItems.Add(
                new ToolbarItem(TextResources.Save, null, async () =>
                    {
                        var answer = 
                            await DisplayAlert(
                                title: TextResources.Customers_Orders_EditOrder_SaveConfirmTitle,
                                message: TextResources.Customers_Orders_EditOrder_SaveConfirmDescription,
                                accept: TextResources.Save,
                                cancel: TextResources.Cancel);

                        if (answer)
                        {
                            ViewModel.SaveOrderCommand.Execute(null);

                            await Navigation.PopAsync();
                        }
                    }));

            ToolbarItems.Add(
                new ToolbarItem(TextResources.Exit, null, async () =>
                    {
                        {
                            var answer = 
                                await DisplayAlert(
                                    title: TextResources.Customers_Orders_EditOrder_ExitConfirmTitle,
                                    message: TextResources.Customers_Orders_EditOrder_ExitConfirmDescription,
                                    accept: TextResources.Exit_and_Discard,
                                    cancel: TextResources.Cancel);

                            if (answer)
                            {
                                await Navigation.PopAsync();
                            }
                        }
                    }));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();


        }
    }
}

