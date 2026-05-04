using System.Globalization;
using floofy.Services;

namespace floofy.Converters;

public class StringBoolConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return !string.IsNullOrWhiteSpace(value?.ToString());
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class InvertedBoolConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return value is bool b ? !b : true;
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class RatingToStarsConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    var rating = value is int i ? i : 0;
    if (rating < 0) rating = 0;
    if (rating > 5) rating = 5;
    return new string('★', rating) + new string('☆', 5 - rating);
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class StockToLabelConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    var qty = value is int i ? i : 0;
    if (qty <= 0) return "Out of stock";
    if (qty < 10) return $"Only {qty} left";
    return "In stock";
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class StockToColorConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    var qty = value is int i ? i : 0;
    if (qty <= 0) return Color.FromArgb("#FF6B6B");
    if (qty < 10) return Color.FromArgb("#E89B3C");
    return Color.FromArgb("#4CAF93");
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class ImageOrPlaceholderConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    var url = value?.ToString();
    if (string.IsNullOrWhiteSpace(url))
    {
      return "no_image.png";
    }
    return url;
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class CurrentUserPetConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is not Guid sellerId)
      return false;

    var sessionService = App.Services.GetRequiredService<SessionService>();
    var currentUser = sessionService.CurrentUser;

    if (currentUser is null)
      return false;

    return sellerId == currentUser.Id;
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}