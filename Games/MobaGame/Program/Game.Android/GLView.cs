using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

public class GLView : SurfaceView 
{
    public GLView(Android.Content.Context Context) : base(Context)
    {
        
    }
    public GLView(Android.Content.Context context, Android.Util.IAttributeSet attrs)
        : base(context, attrs)
    {

    }
    public GLView(Android.Content.Context context, Android.Util.IAttributeSet attrs, Int32 defStyle)
        : base(context, attrs, defStyle)
    {

    }
    public GLView(Android.Content.Context context, Android.Util.IAttributeSet attrs, Int32 defStyleAttr, Int32 defStyleRes)
            : base(context, attrs, defStyleRes)
    {

    }
    protected override void OnAttachedToWindow()
    {
        base.OnAttachedToWindow();
    }

    
}



//        //Client.Game.Instance.Tick();
//        CCore.Engine.Instance.Client.Graphics.HelloTriangle();

