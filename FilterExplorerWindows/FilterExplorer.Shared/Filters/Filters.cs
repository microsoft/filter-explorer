/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using Nokia.Graphics.Imaging;
using System.Linq;

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

        public string Name
        {
            get
            {
                return new Windows.ApplicationModel.Resources.ResourceLoader().GetString(Id.Split('.').Last() + "Name");
            }
        }

        public override string ToString()
        {
            return Name;
        }
        
        public abstract Nokia.Graphics.Imaging.IFilter GetFilter();
    }

    public class AdjustColorBlueFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorAdjustFilter(0.0f, 0.0f, 0.5f); } }
    public class AdjustColorGreenFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorAdjustFilter(0.0f, 0.5f, 0.0f); } }
    public class AdjustColorRedFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorAdjustFilter(0.5f, 0.0f, 0.0f); } }
    //public class AlphaToGrayscaleFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.AlphaToGrayscaleFilter(); } }
    public class AntiqueFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.AntiqueFilter(); } }
    public class AutoEnhanceFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.AutoEnhanceFilter(true, true); } }
    //public class AutoLevelsFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.AutoLevelsFilter(); } }
    //public class BlendFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.BlendFilter(); } }
    public class BlurFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.BlurFilter(5); } }
    public class BrightnessFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.BrightnessFilter(0.35f); } }
    public class CartoonFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.CartoonFilter(true); } }
    //public class ChromaKeyFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ChromaKeyFilter(); } }
    public class ColorBoostFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorBoostFilter(1.0f); } }
    public class ColorizationBlueFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorizationFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), 0.25, 0.25); } }
    public class ColorizationGreenFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorizationFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.25, 0.25); } }
    public class ColorizationRedFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorizationFilter(Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), 0.25, 0.25); } }
    public class ContrastFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ContrastFilter(0.8f); } }
    //public class CropFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.CropFilter(); } }
    //public class CurvesFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.CurvesFilter(); } }
    public class DespeckleFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.DespeckleFilter(DespeckleLevel.High); } }
    public class EmbossFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.EmbossFilter(1.0f); } }
    public class ExposureFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ExposureFilter(ExposureMode.Natural, 0.5f); } }
    public class FlipHorizontalFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.FlipFilter(FlipMode.Horizontal); } }
    public class FlipVerticalFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.FlipFilter(FlipMode.Vertical); } }
    public class FogFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.FogFilter(); } }
    public class FoundationFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.FoundationFilter(); } }
    public class GrayscaleFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.GrayscaleFilter(); } }
    public class GrayscaleNegativeFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.GrayscaleNegativeFilter(); } }
    public class HueSaturationFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.HueSaturationFilter(0.9, 0.9); } }
    //public class ImageFusionFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ImageFusionFilter(); } }
    //public class LevelsFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.LevelsFilter(0.3f, 0.5f, 0.1f); } }
    //public class LocalBoostFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.LocalBoostFilter(); } }
    public class LocalBoostAutomaticFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.LocalBoostAutomaticFilter(); } }
    public class LomoFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.LomoFilter(0.5f, 0.75f, LomoVignetting.Medium, LomoStyle.Neutral); } }
    public class MagicPenFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.MagicPenFilter(); } }
    public class MilkyFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.MilkyFilter(); } }
    public class MirrorFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.MirrorFilter(); } }
    public class MonoColorBlueFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), 0.3); } }
    public class MonoColorGreenFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.3); } }
    public class MonoColorRedFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), 0.3); } }
    public class MoonlightFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.MoonlightFilter(2); } }
    public class NegativeFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.NegativeFilter(); } }
    public class NoiseFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.NoiseFilter(); } }
    public class OilyFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.OilyFilter(); } }
    public class PaintFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.PaintFilter(1); } }
    public class PosterizeFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.PosterizeFilter(4); } }
    //public class ReframingFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ReframingFilter(); } }
    public class RotationLeftFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.RotationFilter(270); } }
    public class RotationRightFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.RotationFilter(90); } }
    public class SepiaFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.SepiaFilter(); } }
    public class SharpnessFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.SharpnessFilter(3); } }
    public class SketchFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.SketchFilter(SketchMode.Color); } }
    public class SolarizeFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.SolarizeFilter(0.2f); } }
    //public class SplitToneFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.SplitToneFilter(); } }
    //public class SpotlightFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.SpotlightFilter(); } }
    public class StampFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.StampFilter(1, 0.6f); } }
    public class SwapBlueGreenFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.25, false, false); } }
    public class SwapBlueRedFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), 0.25, false, false); } }
    public class SwapGreenBlueFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), 0.25, false, false); } }
    public class SwapGreenRedFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), 0.25, false, false); } }
    public class SwapRedBlueFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), 0.25, false, false); } }
    public class SwapRedGreenFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.25, false, false); } }
    public class TemperatureAndTintFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.TemperatureAndTintFilter(0.2, -0.4); } }
    public class WarpFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.WarpFilter(WarpEffect.Twister, 0.3f); } }
    public class WatercolorFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.WatercolorFilter(0.3f, 0.3f); } }
    public class WhiteBalanceFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.WhiteBalanceFilter(WhitePointCalculationMode.Mean); } }
    public class WhiteboardEnhancementFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.WhiteboardEnhancementFilter(WhiteboardEnhancementMode.Hard); } }
    public class VignettingFilter : Filter { public override Nokia.Graphics.Imaging.IFilter GetFilter() { return new Nokia.Graphics.Imaging.VignettingFilter(0.5f, Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0x00)); } }
}
