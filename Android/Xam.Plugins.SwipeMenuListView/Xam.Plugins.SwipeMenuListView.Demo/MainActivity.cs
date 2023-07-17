using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Xamarin.Essentials;

namespace Xam.Plugins.SwipeList.Demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            RunOnUiThread(() =>
            {
                var listView = FindViewById<SwipeMenuListView>(Resource.Id.listView);
                var array = new[] { "1", "2", "3", "4", "5", "6", "7", "8" };
                var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleExpandableListItem1, array);
                listView.MenuCreator = new SwipeMenuItemCreator(ApplicationContext);
                listView.MenuItemClick += ListView_MenuItemClick;
                listView.Adapter = adapter;
            });
        }
        private void ListView_MenuItemClick(int position, SwipeMenu menu, int index)
        {

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}