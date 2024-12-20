namespace Kashapov;

public partial class ItemMain : ContentPage
{
    string login_page;
	public ItemMain(string login)
    {
		InitializeComponent();
        Login_user.Text = login;
        login_page = login;
        var viewModel = new ItemMainViewModel(() => Login_user.Text);
        BindingContext = viewModel;
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Cart_Page(login_page));
    }

    private void ImageButton_Clicked_1(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Lk_Page(login_page));
    }
}