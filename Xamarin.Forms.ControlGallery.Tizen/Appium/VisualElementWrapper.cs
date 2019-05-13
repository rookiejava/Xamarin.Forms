using System;
using ElmSharp;
using Tizen.Appium;
using TPlatform = Xamarin.Forms.Platform.Tizen.Platform;

namespace Xamarin.Forms.ControlGallery.Tizen
{
    public class VisualElementWrapper : BaseObjectWrapper<VisualElement>
    {
        WeakReference<VisualElement> _ref;
        string _id;

        public override string[] TextProperties => new string[] { "Text", "FormattedText" };
        public override string[] DisplayedTextProperies => new string[] { "Text", "Name", "FormattedText", "Title", "Placeholder", "Detail" };
        public override string Id => _id;

        public override event EventHandler Deleted;

        public override bool IsShown
        {
            get
            {
                return (Control != null)? (Control.GetIsShownProperty() && Control.IsVisible) : false;
            }
        }

        public override VisualElement Control
        {
            get
            {
                VisualElement ve;
                if (_ref.TryGetTarget(out ve))
                {
                    return ve;
                }

                Deleted?.Invoke(this, EventArgs.Empty);
                return null;
            }
        }

        public override Geometry Geometry
        {
            get
            {
                return TizenAppium.RunOnMainThread<Geometry>(() =>
                {
                    if (NativeControl != null)
                    {
                        return new Geometry(NativeControl.Geometry.X, NativeControl.Geometry.Y, NativeControl.Geometry.Width, NativeControl.Geometry.Height);
                    }
                    else
                    {
                        return new Geometry();
                    }
                });
            }
        }

        public override bool IsFocused
        {
            get
            {
                if(Control is VisualElement ve)
                {
                    return ve.IsFocused;
                }

                return false;
            }
        }

        EvasObject NativeControl
        {
            get
            {
                if (Control != null)
                {
                    return TPlatform.GetRenderer((VisualElement)Control).NativeView;
                }

                return null;
            }
        }

        public VisualElementWrapper(VisualElement ve)
        {
            _ref = new WeakReference<VisualElement>(ve);
            _id = ve.GetId();

            if (NativeControl != null)
            {
                NativeControl.Deleted += (sender, arg) =>
                {
                    Deleted?.Invoke(this, EventArgs.Empty);
                };
            }
        }
    }
}
