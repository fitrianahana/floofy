using floofy.Views;
namespace floofy
{
  public partial class AppShell : Shell
  {
    public AppShell()
    {
      InitializeComponent();
      Routing.RegisterRoute("petDetail", typeof(PetDetail));
      Routing.RegisterRoute("productDetail", typeof(ProductDetail));
      Routing.RegisterRoute("cart", typeof(Cart));
      Routing.RegisterRoute("sellPet", typeof(SellPet));
      Routing.RegisterRoute("rehomingAgreement", typeof(RehomingAgreement));
      Routing.RegisterRoute("rehomingPolicy", typeof(RehomingPolicy));
    }
  }
}
