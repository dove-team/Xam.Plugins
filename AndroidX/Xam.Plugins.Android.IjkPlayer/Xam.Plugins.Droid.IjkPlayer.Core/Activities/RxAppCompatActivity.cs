using Android.OS;
using AndroidX.AppCompat.App;
using Com.Trello.Rxlifecycle2;
using Com.Trello.Rxlifecycle2.Android;
using IO.Reactivex;
using IO.Reactivex.Subjects;

namespace Xam.Plugins.Droid.IjkPlayer.Core.Activities
{
    public abstract class RxAppCompatActivity : AppCompatActivity, ILifecycleProvider
    {
        private BehaviorSubject lifecycleSubject = BehaviorSubject.Create();
        private ActivityEvent activityEvent;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            activityEvent = ActivityEvent.Create;
            lifecycleSubject.OnNext(activityEvent);
        }
        protected override void OnStart()
        {
            base.OnStart();
            activityEvent = ActivityEvent.Start;
            lifecycleSubject.OnNext(activityEvent);
        }
        protected override void OnResume()
        {
            base.OnResume();
            activityEvent = ActivityEvent.Resume;
            lifecycleSubject.OnNext(activityEvent);
        }
        protected override void OnPause()
        {
            activityEvent = ActivityEvent.Pause;
            lifecycleSubject.OnNext(activityEvent);
            base.OnPause();
        }
        protected override void OnStop()
        {
            activityEvent = ActivityEvent.Stop;
            lifecycleSubject.OnNext(activityEvent);
            base.OnStop();
        }
        protected override void OnDestroy()
        {
            activityEvent = ActivityEvent.Destroy;
            lifecycleSubject.OnNext(activityEvent);
            base.OnDestroy();
        }
        public LifecycleTransformer BindToLifecycle()
        {
            return RxLifecycleAndroid.BindActivity(lifecycleSubject);
        }
        public LifecycleTransformer BindUntilEvent(Java.Lang.Object p0)
        {
            return RxLifecycle.BindUntilEvent(lifecycleSubject, p0);
        }
        Observable ILifecycleProvider.Lifecycle()
        {
            return lifecycleSubject.Hide();
        }
    }
}