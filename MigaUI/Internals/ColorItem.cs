
namespace Acorisoft.Miga.UI.Internals
{
    public class ColorItem : RadioButton
    {
        static ColorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorItem), new FrameworkPropertyMetadata(typeof(ColorItem)));
        }

        public Color Color { get; set; }

        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register(
            "Brush",
            typeof(Brush),
            typeof(ColorItem),
            new PropertyMetadata(default(Brush)));

        public Brush Brush
        {
            get => (Brush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }
    }

    public class ColorPickerItem : Button
    {
        static ColorPickerItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerItem), new FrameworkPropertyMetadata(typeof(ColorPickerItem)));
        }
    }
}