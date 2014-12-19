/*
 * Copyright (c) 2014 Microsoft Mobile
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using Lumia.Imaging;
using Lumia.Imaging.Adjustments;
using Lumia.Imaging.Artistic;
using Lumia.Imaging.Transforms;
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
        
        public abstract IFilter GetFilter();
    }

    public class AdjustColorBlueFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.ColorAdjustFilter(0.0f, 0.0f, 0.5f); } }
    public class AdjustColorGreenFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.ColorAdjustFilter(0.0f, 0.5f, 0.0f); } }
    public class AdjustColorRedFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.ColorAdjustFilter(0.5f, 0.0f, 0.0f); } }
    //public class AlphaToGrayscaleFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.AlphaToGrayscaleFilter(); } }
    public class AntiqueFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.AntiqueFilter(); } }
    public class AutoEnhanceFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.AutoEnhanceFilter(true, true); } }
    //public class AutoLevelsFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.AutoLevelsFilter(); } }
    //public class BlendFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.BlendFilter(); } }
    public class BlurFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.BlurFilter(5); } }
    public class BrightnessFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.BrightnessFilter(0.35f); } }
    public class CartoonFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.CartoonFilter (true); } }
    //public class ChromaKeyFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.ChromaKeyFilter(); } }
    public class ColorBoostFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.ColorBoostFilter(1.0f); } }
    public class ColorizationBlueFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.ColorizationFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), 0.25, 0.25); } }
    public class ColorizationGreenFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.ColorizationFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.25, 0.25); } }
    public class ColorizationRedFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.ColorizationFilter(Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), 0.25, 0.25); } }
    public class ContrastFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.ContrastFilter(0.8f); } }
    //public class CropFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.CropFilter(); } }
    //public class CurvesFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.CurvesFilter(); } }
    public class DespeckleFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.DespeckleFilter(DespeckleLevel.High); } }
    public class EmbossFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.EmbossFilter(1.0f); } }
    public class ExposureFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.ExposureFilter(ExposureMode.Natural, 0.5f); } }
    public class FlipHorizontalFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Transforms.FlipFilter(FlipMode.Horizontal); } }
    public class FlipVerticalFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Transforms.FlipFilter(FlipMode.Vertical); } }
    public class FogFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.FogFilter(); } }
    public class FoundationFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.FoundationFilter(); } }
    public class GrayscaleFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.GrayscaleFilter(); } }
    public class GrayscaleNegativeFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.GrayscaleNegativeFilter(); } }
    public class HueSaturationFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.HueSaturationFilter(0.9, 0.9); } }
    //public class ImageFusionFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.ImageFusionFilter(); } }
    //public class LevelsFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.LevelsFilter(0.3f, 0.5f, 0.1f); } }
    //public class LocalBoostFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.LocalBoostFilter(); } }
    public class LocalBoostAutomaticFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.LocalBoostAutomaticFilter(); } }
    public class LomoFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.LomoFilter(0.5f, 0.75f, LomoVignetting.Medium, LomoStyle.Neutral); } }
    public class MagicPenFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.MagicPenFilter(); } }
    public class MilkyFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.MilkyFilter(); } }
    public class MirrorFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.MirrorFilter(); } }
    public class MonoColorBlueFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), 0.3); } }
    public class MonoColorGreenFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.3); } }
    public class MonoColorRedFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.MonoColorFilter(Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), 0.3); } }
    public class MoonlightFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.MoonlightFilter(2); } }
    public class NegativeFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.NegativeFilter(); } }
    public class NoiseFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.NoiseFilter(); } }
    public class OilyFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.OilyFilter(); } }
    public class PaintFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.PaintFilter(1); } }
    public class PosterizeFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.PosterizeFilter(4); } }
    //public class ReframingFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.ReframingFilter(); } }
    public class RotationLeftFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Transforms.RotationFilter(270); } }
    public class RotationRightFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Transforms.RotationFilter(90); } }
    public class SepiaFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.SepiaFilter(); } }
    public class SharpnessFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.SharpnessFilter(3); } }
    public class SketchFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.SketchFilter(SketchMode.Color); } }
    public class SolarizeFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.SolarizeFilter(0.2f); } }
    //public class SplitToneFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.SplitToneFilter(); } }
    //public class SpotlightFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.SpotlightFilter(); } }
    public class StampFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.StampFilter(1, 0.6f); } }
    public class SwapBlueGreenFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.25, false, false); } }
    public class SwapBlueRedFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), 0.25, false, false); } }
    public class SwapGreenBlueFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), 0.25, false, false); } }
    public class SwapGreenRedFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), 0.25, false, false); } }
    public class SwapRedBlueFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0xff), 0.25, false, false); } }
    public class SwapRedGreenFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.ColorSwapFilter(Windows.UI.Color.FromArgb(0xff, 0xff, 0x00, 0x00), Windows.UI.Color.FromArgb(0xff, 0x00, 0xff, 0x00), 0.25, false, false); } }
    public class TemperatureAndTintFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.TemperatureAndTintFilter(0.2, -0.4); } }
    public class WarpFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.WarpFilter(WarpEffect.Twister, 0.3f); } }
    public class WatercolorFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.WatercolorFilter(0.3f, 0.3f); } }
    public class WhiteBalanceFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.WhiteBalanceFilter(WhitePointCalculationMode.Mean); } }
    public class WhiteboardEnhancementFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Adjustments.WhiteboardEnhancementFilter(WhiteboardEnhancementMode.Hard); } }
    public class VignettingFilter : Filter { public override IFilter GetFilter() { return new Lumia.Imaging.Artistic.VignettingFilter(0.5f, Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0x00)); } }
}
