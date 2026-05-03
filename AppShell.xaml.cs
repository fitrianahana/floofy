using floofy.Views;
namespace floofy
{
  public partial class AppShell : Shell
  {
    public AppShell()
    {
      InitializeComponent();
      Routing.RegisterRoute("petDetail", typeof(PetDetail));
    }
  }
}
