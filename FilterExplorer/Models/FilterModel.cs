using Nokia.Graphics.Imaging;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ImageProcessingApp.Models
{
    public abstract class FilterModel
    {
        #region Properties

        [XmlIgnore]
        public string Name { get; set; }

        [XmlIgnore]
        public Queue<IFilter> Components { get; set; }

        #endregion

        public FilterModel()
        {
            Components = new Queue<IFilter>();
        }
    }

    #region Art filters

    public class AntiqueFilterModel : FilterModel
    {
        public AntiqueFilterModel()
        {
            Name = "Antique";
            Components.Enqueue(FilterFactory.CreateAntiqueFilter());
        }
    }

    public class BlurFilterModel : FilterModel
    {
        public BlurFilterModel()
        {
            Name = "Blur";
            Components.Enqueue(FilterFactory.CreateBlurFilter(BlurLevel.Blur5));
        }
    }

    public class BrightnessFilterModel : FilterModel
    {
        public BrightnessFilterModel()
        {
            Name = "Brightness";
            Components.Enqueue(FilterFactory.CreateBrightnessFilter(0.35f));
        }
    }

    public class CartoonFilterModel : FilterModel
    {
        public CartoonFilterModel()
        {
            Name = "Cartoon";
            Components.Enqueue(FilterFactory.CreateCartoonFilter(true));
        }
    }

    public class ColorAdjustRedFilterModel : FilterModel
    {
        public ColorAdjustRedFilterModel()
        {
            Name = "Color Adjust Red";
            Components.Enqueue(FilterFactory.CreateColorAdjustFilter(1.0f, 0.0f, 0.0f));
        }
    }

    public class ColorAdjustGreenFilterModel : FilterModel
    {
        public ColorAdjustGreenFilterModel()
        {
            Name = "Color Adjust Green";
            Components.Enqueue(FilterFactory.CreateColorAdjustFilter(0.0f, 1.0f, 0.0f));
        }
    }

    public class ColorAdjustBlueFilterModel : FilterModel
    {
        public ColorAdjustBlueFilterModel()
        {
            Name = "Color Adjust Blue";
            Components.Enqueue(FilterFactory.CreateColorAdjustFilter(0.0f, 0.0f, 1.0f));
        }
    }

    public class ContrastFilterModel : FilterModel
    {
        public ContrastFilterModel()
        {
            Name = "Contrast";
            Components.Enqueue(FilterFactory.CreateContrastFilter(0.8f));
        }
    }

    public class DespeckleFilterModel : FilterModel
    {
        public DespeckleFilterModel()
        {
            Name = "Despeckle";
            Components.Enqueue(FilterFactory.CreateDespeckleFilter(DespeckleLevel.High));
        }
    }

    public class EmbossFilterModel : FilterModel
    {
        public EmbossFilterModel()
        {
            Name = "Emboss";
            Components.Enqueue(FilterFactory.CreateEmbossFilter(1.0f));
        }
    }

    public class FlipFilterModel : FilterModel
    {
        public FlipFilterModel()
        {
            Name = "Flip";
            Components.Enqueue(FilterFactory.CreateFlipFilter(FlipMode.Horizontal));
        }
    }

    public class GrayscaleFilterModel : FilterModel
    {
        public GrayscaleFilterModel()
        {
            Name = "Grayscale";
            Components.Enqueue(FilterFactory.CreateGrayscaleFilter());
        }
    }

    public class HueSaturationFilterModel : FilterModel
    {
        public HueSaturationFilterModel()
        {
            Name = "Hue Saturation";
            Components.Enqueue(FilterFactory.CreateHueSaturationFilter(228, 228));
        }
    }

    public class LomoFilterModel : FilterModel
    {
        public LomoFilterModel()
        {
            Name = "Lomo";
            Components.Enqueue(FilterFactory.CreateLomoFilter(0.5f, 0.75f, LomoVignetting.Medium, LomoStyle.Neutral));
        }
    }

    public class MagicPenFilterModel : FilterModel
    {
        public MagicPenFilterModel()
        {
            Name = "Magic Pen";
            Components.Enqueue(FilterFactory.CreateMagicPenFilter());
        }
    }

    public class MilkyFilterModel : FilterModel
    {
        public MilkyFilterModel()
        {
            Name = "Milky";
            Components.Enqueue(FilterFactory.CreateMilkyFilter());
        }
    }

    public class MirrorFilterModel : FilterModel
    {
        public MirrorFilterModel()
        {
            Name = "Mirror";
            Components.Enqueue(FilterFactory.CreateMirrorFilter());
        }
    }

    public class MonocolorRedFilterModel : FilterModel
    {
        public MonocolorRedFilterModel()
        {
            Name = "Monocolor Red";
            Components.Enqueue(FilterFactory.CreateMonoColorFilter(new Windows.UI.Color() { R = 0xff, G = 0x00, B = 0x00 }, 64));
        }
    }

    public class MonocolorGreenFilterModel : FilterModel
    {
        public MonocolorGreenFilterModel()
        {
            Name = "Monocolor Green";
            Components.Enqueue(FilterFactory.CreateMonoColorFilter(new Windows.UI.Color() { R = 0x00, G = 0xff, B = 0x00 }, 64));
        }
    }

    public class MonocolorBlueFilterModel : FilterModel
    {
        public MonocolorBlueFilterModel()
        {
            Name = "Monocolor Blue";
            Components.Enqueue(FilterFactory.CreateMonoColorFilter(new Windows.UI.Color() { R = 0x00, G = 0x00, B = 0xff }, 64));
        }
    }


    public class MoonlightFilterModel : FilterModel
    {
        public MoonlightFilterModel()
        {
            Name = "Moonlight";
            Components.Enqueue(FilterFactory.CreateMoonlightFilter(24));
        }
    }

    public class NegativeFilterModel : FilterModel
    {
        public NegativeFilterModel()
        {
            Name = "Negative";
            Components.Enqueue(FilterFactory.CreateNegativeFilter());
        }
    }

    public class OilyFilterModel : FilterModel
    {
        public OilyFilterModel()
        {
            Name = "Oily";
            Components.Enqueue(FilterFactory.CreateOilyFilter());
        }
    }

    public class PaintFilterModel : FilterModel
    {
        public PaintFilterModel()
        {
            Name = "Paint";
            Components.Enqueue(FilterFactory.CreatePaintFilter(1));
        }
    }

    public class PosterizeFilterModel : FilterModel
    {
        public PosterizeFilterModel()
        {
            Name = "Posterize";
            Components.Enqueue(FilterFactory.CreatePosterizeFilter(8));
        }
    }

    public class SepiaFilterModel : FilterModel
    {
        public SepiaFilterModel()
        {
            Name = "Sepia";
            Components.Enqueue(FilterFactory.CreateSepiaFilter());
        }
    }

    public class SharpnessFilterModel : FilterModel
    {
        public SharpnessFilterModel()
        {
            Name = "Sharpness";
            Components.Enqueue(FilterFactory.CreateSharpnessFilter(SharpnessLevel.Level4));
        }
    }

    public class SketchFilterModel : FilterModel
    {
        public SketchFilterModel()
        {
            Name = "Sketch";
            Components.Enqueue(FilterFactory.CreateSketchFilter(SketchMode.Color));
        }
    }

    public class SolarizeFilterModel : FilterModel
    {
        public SolarizeFilterModel()
        {
            Name = "Solarize";
            Components.Enqueue(FilterFactory.CreateSolarizeFilter(0.5f));
        }
    }

    public class StampFilterModel : FilterModel
    {
        public StampFilterModel()
        {
            Name = "Stamp";
            Components.Enqueue(FilterFactory.CreateStampFilter(3, 60));
        }
    }

    public class StepRotationLeftFilterModel : FilterModel
    {
        public StepRotationLeftFilterModel()
        {
            Name = "Rotate left";
            Components.Enqueue(FilterFactory.CreateStepRotationFilter(Rotation.Rotate270));
        }
    }

    public class StepRotationRightFilterModel : FilterModel
    {
        public StepRotationRightFilterModel()
        {
            Name = "Rotate right";
            Components.Enqueue(FilterFactory.CreateStepRotationFilter(Rotation.Rotate90));
        }
    }

    public class WarpTwisterFilterModel : FilterModel
    {
        public WarpTwisterFilterModel()
        {
            Name = "Warp Twister";
            Components.Enqueue(FilterFactory.CreateWarpFilter(WarpEffect.Twister, 0.3f));
        }
    }

    public class WatercolorFilterModel : FilterModel
    {
        public WatercolorFilterModel()
        {
            Name = "Watercolor";
            Components.Enqueue(FilterFactory.CreateWatercolorFilter(0.8f, 0.5f));
        }
    }

    public class VignettingFilterModel : FilterModel
    {
        public VignettingFilterModel()
        {
            Name = "Vignetting";
            Components.Enqueue(FilterFactory.CreateVignettingFilter(0.4f, Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0x00)));
        }
    }

#endregion

    #region Enhancement filters

    public class AutoEnhanceFilterModel : FilterModel
    {
        public AutoEnhanceFilterModel()
        {
            Name = "Auto Enhance";

            AutoEnhanceConfiguration configuration =  new AutoEnhanceConfiguration();
            configuration.ApplyAutomaticContrastAndBrightness();
            configuration.ApplyAutomaticLocalBoost();

            Components.Enqueue(FilterFactory.CreateAutoEnhanceFilter(configuration));
        }
    }

    public class AutoLevelsFilterModel : FilterModel
    {
        public AutoLevelsFilterModel()
        {
            Name = "Auto Levels";
            Components.Enqueue(FilterFactory.CreateAutoLevelsFilter());
        }
    }

    public class ColorBoostFilterModel : FilterModel
    {
        public ColorBoostFilterModel()
        {
            Name = "Color Boost";
            Components.Enqueue(FilterFactory.CreateColorBoostFilter(3.0f));
        }
    }

    public class ExposureFilterModel : FilterModel
    {
        public ExposureFilterModel()
        {
            Name = "Exposure";
            Components.Enqueue(FilterFactory.CreateExposureFilter(ExposureMode.Natural, 0.9f));
        }
    }

    public class FoundationFilterModel : FilterModel
    {
        public FoundationFilterModel()
        {
            Name = "Foundation";
            Components.Enqueue(FilterFactory.CreateFoundationFilter());
        }
    }

    public class LevelsFilterModel : FilterModel
    {
        public LevelsFilterModel()
        {
            Name = "Levels";
            Components.Enqueue(FilterFactory.CreateLevelsFilter(0.7f, 0.2f, 0.4f));
        }
    }

    public class LocalBoostFilterModel : FilterModel
    {
        public LocalBoostFilterModel()
        {
            Name = "Local Boost";
            Components.Enqueue(FilterFactory.CreateLocalBoostFilter(10));
        }
    }

    public class TemperatureAndTintFilterModel : FilterModel
    {
        public TemperatureAndTintFilterModel()
        {
            Name = "Temperature & Tint";
            Components.Enqueue(FilterFactory.CreateTemperatureAndTintFilter(80, -40));
        }
    }

    public class WhiteboardEnhancementFilterModel : FilterModel
    {
        public WhiteboardEnhancementFilterModel()
        {
            Name = "Whiteboard";
            Components.Enqueue(FilterFactory.CreateWhiteboardEnhancementFilter(true));
        }
    }

    #endregion
}
