using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterExplorer.Filters
{
    public abstract class Filter
    {
        public string Id
        {
            get
            {
                return GetType().ToString();
            }
        }

        public string Name { get; protected set; }

        public abstract Nokia.Graphics.Imaging.IFilter GetFilter();
    }

    public class AntiqueFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.AntiqueFilter();
        }
    }

    //public class BlurFilter : Filter
    //{
    //    public override Nokia.Graphics.Imaging.IFilter GetFilter()
    //    {
    //        return new Nokia.Graphics.Imaging.BlurFilter(5);
    //    }
    //}

    public class BrightnessFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.BrightnessFilter(0.35f);
        }
    }

    public class CartoonFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.CartoonFilter(true);
        }
    }

    public class RedColorAdjustFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.ColorAdjustFilter(1.0f, 0.0f, 0.0f);
        }
    }

    public class GreenColorAdjustFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.ColorAdjustFilter(0.0f, 1.0f, 0.0f);
        }
    }

    public class BlueColorAdjustFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.ColorAdjustFilter(0.0f, 0.0f, 1.0f);
        }
    }

    public class ContrastFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.ContrastFilter(0.8f);
        }
    }

    public class DespeckleFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.DespeckleFilter(DespeckleLevel.High);
        }
    }

    public class EmbossFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.EmbossFilter(1.0f);
        }
    }

    public class HorizontalFlipFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.FlipFilter(FlipMode.Horizontal);
        }
    }

    public class VerticalFlipFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.FlipFilter(FlipMode.Vertical);
        }
    }

    public class GrayscaleFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.GrayscaleFilter();
        }
    }

    public class HueSaturationFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.HueSaturationFilter(0.9, 0.9);
        }
    }

    public class LomoFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.LomoFilter(0.5f, 0.75f, LomoVignetting.Medium, LomoStyle.Neutral);
        }
    }

    public class MagicPenFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.MagicPenFilter();
        }
    }

    public class MilkyFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.MilkyFilter();
        }
    }

    public class MirrorFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.MirrorFilter();
        }
    }

    public class RedMonoColorFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), 0.3);
        }
    }

    public class GreenMonoColorFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.3);
        }
    }

    public class BlueMonoColorFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), 0.3);
        }
    }

    public class MoonlightFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.MoonlightFilter(2);
        }
    }

    public class NegativeFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.NegativeFilter();
        }
    }

    public class OilyFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.OilyFilter();
        }
    }

    public class PaintFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.PaintFilter(1);
        }
    }

    public class PosterizeFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.PosterizeFilter(8);
        }
    }

    public class SepiaFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.SepiaFilter();
        }
    }

    public class SharpnessFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.SharpnessFilter(4);
        }
    }

    public class SketchFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.SketchFilter(SketchMode.Color);
        }
    }

    public class SolarizeFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.SolarizeFilter(0.5f);
        }
    }

    public class StampFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.StampFilter(3, 0.6);
        }
    }

    public class LeftRotationFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.RotationFilter(270);
        }
    }

    public class RightRotationFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.RotationFilter(90);
        }
    }

    public class WarpFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.WarpFilter(WarpEffect.Twister, 0.3f);
        }
    }

    public class WatercolorFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.WatercolorFilter(0.8f, 0.5f);
        }
    }

    public class VignettingFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.VignettingFilter(0.4f, Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0x00));
        }
    }

    #region Enhancement filters

    public class AutoEnhanceFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.AutoEnhanceFilter(true, true);
        }
    }

    public class AutoLevelsFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.AutoLevelsFilter();
        }
    }

    public class ColorBoostFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.ColorBoostFilter(3.0f);
        }
    }

    public class ExposureFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.ExposureFilter(ExposureMode.Natural, 0.9f);
        }
    }

    public class FoundationFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.FoundationFilter();
        }
    }

    public class LevelsFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.LevelsFilter(0.7f, 0.2f, 0.4f);
        }
    }

    public class LocalBoostAutomaticFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.LocalBoostAutomaticFilter();
        }
    }

    public class TemperatureAndTintFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.TemperatureAndTintFilter(0.8, -0.4);
        }
    }

    public class WhiteboardEnhancementFilter : Filter
    {
        public override Nokia.Graphics.Imaging.IFilter GetFilter()
        {
            return new Nokia.Graphics.Imaging.WhiteboardEnhancementFilter(WhiteboardEnhancementMode.Soft);
        }
    }
    
    #endregion
}
