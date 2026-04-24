<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="floofy.Views.Shop"
             Title="Available Pets">
  <VerticalStackLayout Padding="0" Spacing="0">
    <!-- Header Section -->
    <VerticalStackLayout Padding="16" Spacing="12" BackgroundColor="#F5F5F5">
      <Label Text="Find Your Perfect Pet"
             FontSize="24"
             FontAttributes="Bold"/>
      
      <!-- Search Bar -->
      <SearchBar x:Name="SearchBar"
                 Placeholder="Search pets by name..."
                 SearchButtonPressed="OnSearchButtonPressed"
                 IsEnabled="{Binding IsLoading, Converter={StaticResource InvertedBoolConverter}}"/>
      
      <!-- Error/Success Message Display -->
      <Label Text="{Binding ErrorMessage}"
             IsVisible="{Binding ErrorMessage, Converter={StaticResource StringBoolConverter}}"
             TextColor="Red"
             FontAttributes="Bold"
             FontSize="14"/>
    </VerticalStackLayout>
    <!-- Loading Indicator -->
    <ActivityIndicator IsRunning="{Binding IsLoading}"
                       IsVisible="{Binding IsLoading}"
                       Color="{StaticResource Primary}"
                       VerticalOptions="Center"
                       HorizontalOptions="Center"
                       Margin="0,20,0,0"/>
    <!-- Pets Collection View -->
    <CollectionView ItemsSource="{Binding Pets}"
                    SelectionMode="Single"
                    IsVisible="{Binding IsLoading, Converter={StaticResource InvertedBoolConverter}}"
                    SelectionChangedCommand="{Binding SelectPetCommand}"
                    SelectionChangedCommandParameter="{Binding SelectedItem, Source={RelativeSource Self}}"
                    Margin="0">
      <CollectionView.ItemsLayout>
        <LinearItemsLayout Orientation="Vertical"
                           ItemSpacing="12"/>
      </CollectionView.ItemsLayout>
      
      <CollectionView.ItemTemplate>
        <DataTemplate>
          <StackLayout Padding="16" Spacing="8">
            <Frame BorderColor="{StaticResource Primary}"
                   CornerRadius="12"
                   HasShadow="True"
                   Padding="0"
                   Margin="0">
              <VerticalStackLayout Spacing="12" Padding="0">
                <!-- Pet Image -->
                <Image Source="{Binding ImageUrl}"
                       Aspect="AspectFill"
                       HeightRequest="200"
                       WidthRequest="360"/>
                
                <!-- Pet Details -->
                <VerticalStackLayout Padding="12" Spacing="6">
                  <!-- Pet Name and Category -->
                  <HorizontalStackLayout Spacing="12" VerticalOptions="Center">
                    <Label Text="{Binding Name}"
                           FontSize="18"
                           FontAttributes="Bold"
                           VerticalTextAlignment="Center"/>
                    <Label Text="{Binding Category}"
                           FontSize="12"
                           BackgroundColor="{StaticResource Primary}"
                           TextColor="White"
                           Padding="8,4"
                           CornerRadius="4"
                           VerticalTextAlignment="Center"/>
                  </HorizontalStackLayout>
                  
                  <!-- Pet Description -->
                  <Label Text="{Binding Description}"
                         FontSize="13"
                         TextColor="Gray"
                         LineBreakMode="TailTruncation"
                         MaxLines="2"/>
                  
                  <!-- Price -->
                  <Label Text="{Binding Price, StringFormat='${0:F2}'}"
                         FontSize="20"
                         FontAttributes="Bold"
                         TextColor="{StaticResource Primary}"/>
                  
                  <!-- View Details Button -->
                  <Button Text="View Details"
                          BackgroundColor="{StaticResource Primary}"
                          TextColor="White"
                          CornerRadius="8"
                          Padding="12,8"
                          FontSize="14"/>
                </VerticalStackLayout>
              </VerticalStackLayout>
            </Frame>
          </StackLayout>
        </DataTemplate>
      </CollectionView.ItemTemplate>
      <!-- Empty State -->
      <CollectionView.EmptyView>
        <VerticalStackLayout Padding="20"
                             Spacing="12"
                             HorizontalOptions="Center"
                             VerticalOptions="Center">
          <Label Text="No pets found"
                 FontSize="18"
                 FontAttributes="Bold"
                 HorizontalOptions="Center"/>
          <Label Text="Try adjusting your search"
                 FontSize="14"
                 TextColor="Gray"
                 HorizontalOptions="Center"/>
        </VerticalStackLayout>
      </CollectionView.EmptyView>
    </CollectionView>
  </VerticalStackLayout>
</ContentPage>