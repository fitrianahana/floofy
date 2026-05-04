namespace floofy.Controls;

public partial class BottomNav : ContentView
{
  public static readonly BindableProperty ActiveTabProperty = BindableProperty.Create(
    nameof(ActiveTab),
    typeof(string),
    typeof(BottomNav),
    string.Empty,
    propertyChanged: (b, _, _) => ((BottomNav)b).ApplyActiveState());

  public string ActiveTab
  {
    get => (string)GetValue(ActiveTabProperty);
    set => SetValue(ActiveTabProperty, value);
  }

  private static readonly Color ActiveBg = Color.FromArgb("#F0EAFB");
  private static readonly Color ActiveText = Color.FromArgb("#6B4FAA");
  private static readonly Color InactiveText = Color.FromArgb("#A89BBB");
  private static readonly Color Transparent = Colors.Transparent;

  public BottomNav()
  {
    InitializeComponent();
    ApplyActiveState();
  }

  private void ApplyActiveState()
  {
    Reset(HomeChip, HomeLabel);
    Reset(ShopChip, ShopLabel);
    Reset(CommunityChip, CommunityLabel);
    Reset(PlusChip, PlusLabel);
    Reset(ProfileChip, ProfileLabel);

    switch ((ActiveTab ?? string.Empty).ToLowerInvariant())
    {
      case "home":
        Highlight(HomeChip, HomeLabel);
        break;
      case "shop":
        Highlight(ShopChip, ShopLabel);
        break;
      case "community":
        Highlight(CommunityChip, CommunityLabel);
        break;
      case "plus":
        Highlight(PlusChip, PlusLabel);
        break;
      case "profile":
        Highlight(ProfileChip, ProfileLabel);
        break;
    }
  }

  private static void Reset(Border chip, Label label)
  {
    chip.BackgroundColor = Transparent;
    label.TextColor = InactiveText;
  }

  private static void Highlight(Border chip, Label label)
  {
    chip.BackgroundColor = ActiveBg;
    label.TextColor = ActiveText;
  }

  private async void OnHomeTapped(object? sender, EventArgs e) => await GoTo("home");
  private async void OnShopTapped(object? sender, EventArgs e) => await GoTo("shop");
  private async void OnCommunityTapped(object? sender, EventArgs e) => await GoTo("community");
  private async void OnPlusTapped(object? sender, EventArgs e) => await GoTo("plus");
  private async void OnProfileTapped(object? sender, EventArgs e) => await GoTo("profile");

  private async Task GoTo(string route)
  {
    if (string.Equals(ActiveTab, route, StringComparison.OrdinalIgnoreCase)) return;
    if (Shell.Current is null) return;
    await Shell.Current.GoToAsync($"//{route}");
  }
}
