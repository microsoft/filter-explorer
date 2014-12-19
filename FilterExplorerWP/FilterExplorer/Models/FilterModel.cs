using Lumia.Imaging;
using Lumia.Imaging.Adjustments;
using Lumia.Imaging.Artistic;
using Lumia.Imaging.Transforms;
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
        abstract public Queue<IFilter> Components { get; }

        #endregion
    }

    #region Art filters

    public class AntiqueFilterModel : FilterModel
    {
        public AntiqueFilterModel()
        {
            Name = "Antique";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new AntiqueFilter());

                return components;
            }
        }
    }

    public class BlurFilterModel : FilterModel
    {
        public BlurFilterModel()
        {
            Name = "Blur";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new BlurFilter(5));

                return components;
            }
        }
    }

    public class BrightnessFilterModel : FilterModel
    {
        public BrightnessFilterModel()
        {
            Name = "Brightness";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new BrightnessFilter(0.35f));

                return components;
            }
        }
    }

    public class CartoonFilterModel : FilterModel
    {
        public CartoonFilterModel()
        {
            Name = "Cartoon";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new CartoonFilter(true));

                return components;
            }
        }
    }

    public class ColorAdjustRedFilterModel : FilterModel
    {
        public ColorAdjustRedFilterModel()
        {
            Name = "Color Adjust Red";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new ColorAdjustFilter(1.0f, 0.0f, 0.0f));

                return components;
            }
        }
    }

    public class ColorAdjustGreenFilterModel : FilterModel
    {
        public ColorAdjustGreenFilterModel()
        {
            Name = "Color Adjust Green";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new ColorAdjustFilter(0.0f, 1.0f, 0.0f));

                return components;
            }
        }
    }

    public class ColorAdjustBlueFilterModel : FilterModel
    {
        public ColorAdjustBlueFilterModel()
        {
            Name = "Color Adjust Blue";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new ColorAdjustFilter(0.0f, 0.0f, 1.0f));

                return components;
            }
        }
    }

    public class ContrastFilterModel : FilterModel
    {
        public ContrastFilterModel()
        {
            Name = "Contrast";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new ContrastFilter(0.8f));

                return components;
            }
        }
    }

    public class DespeckleFilterModel : FilterModel
    {
        public DespeckleFilterModel()
        {
            Name = "Despeckle";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new DespeckleFilter(DespeckleLevel.High));

                return components;
            }
        }
    }

    public class EmbossFilterModel : FilterModel
    {
        public EmbossFilterModel()
        {
            Name = "Emboss";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new EmbossFilter(1.0f));

                return components;
            }
        }
    }

    public class FlipFilterModel : FilterModel
    {
        public FlipFilterModel()
        {
            Name = "Flip";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new FlipFilter(FlipMode.Horizontal));

                return components;
            }
        }
    }

    public class GrayscaleFilterModel : FilterModel
    {
        public GrayscaleFilterModel()
        {
            Name = "Grayscale";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new GrayscaleFilter());

                return components;
            }
        }
    }

    public class HueSaturationFilterModel : FilterModel
    {
        public HueSaturationFilterModel()
        {
            Name = "Hue Saturation";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new HueSaturationFilter(0.9, 0.9));

                return components;
            }
        }
    }

    public class LomoFilterModel : FilterModel
    {
        public LomoFilterModel()
        {
            Name = "Lomo";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new LomoFilter(0.5f, 0.75f, LomoVignetting.Medium, LomoStyle.Neutral));

                return components;
            }
        }
    }

    public class MagicPenFilterModel : FilterModel
    {
        public MagicPenFilterModel()
        {
            Name = "Magic Pen";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new MagicPenFilter());

                return components;
            }
        }
    }

    public class MilkyFilterModel : FilterModel
    {
        public MilkyFilterModel()
        {
            Name = "Milky";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new MilkyFilter());

                return components;
            }
        }
    }

    public class MirrorFilterModel : FilterModel
    {
        public MirrorFilterModel()
        {
            Name = "Mirror";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new MirrorFilter());

                return components;
            }
        }
    }

    public class MonocolorRedFilterModel : FilterModel
    {
        public MonocolorRedFilterModel()
        {
            Name = "Monocolor Red";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new MonoColorFilter(new Windows.UI.Color() { R = 0xff, G = 0x00, B = 0x00 }, 0.3));

                return components;
            }
        }
    }

    public class MonocolorGreenFilterModel : FilterModel
    {
        public MonocolorGreenFilterModel()
        {
            Name = "Monocolor Green";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new MonoColorFilter(new Windows.UI.Color() { R = 0x00, G = 0xff, B = 0x00 }, 0.3));

                return components;
            }
        }
    }

    public class MonocolorBlueFilterModel : FilterModel
    {
        public MonocolorBlueFilterModel()
        {
            Name = "Monocolor Blue";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new MonoColorFilter(new Windows.UI.Color() { R = 0x00, G = 0x00, B = 0xff }, 0.3));

                return components;
            }
        }
    }


    public class MoonlightFilterModel : FilterModel
    {
        public MoonlightFilterModel()
        {
            Name = "Moonlight";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new MoonlightFilter(21));

                return components;
            }
        }
    }

    public class NegativeFilterModel : FilterModel
    {
        public NegativeFilterModel()
        {
            Name = "Negative";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new NegativeFilter());

                return components;
            }
        }
    }

    public class OilyFilterModel : FilterModel
    {
        public OilyFilterModel()
        {
            Name = "Oily";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new OilyFilter());

                return components;
            }
        }
    }

    public class PaintFilterModel : FilterModel
    {
        public PaintFilterModel()
        {
            Name = "Paint";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new PaintFilter(1));

                return components;
            }
        }
    }

    public class PosterizeFilterModel : FilterModel
    {
        public PosterizeFilterModel()
        {
            Name = "Posterize";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new PosterizeFilter(8));

                return components;
            }
        }
    }

    public class SepiaFilterModel : FilterModel
    {
        public SepiaFilterModel()
        {
            Name = "Sepia";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new SepiaFilter());

                return components;
            }
        }
    }

    public class SharpnessFilterModel : FilterModel
    {
        public SharpnessFilterModel()
        {
            Name = "Sharpness";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new SharpnessFilter(4));

                return components;
            }
        }
    }

    public class SketchFilterModel : FilterModel
    {
        public SketchFilterModel()
        {
            Name = "Sketch";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new SketchFilter(SketchMode.Color));

                return components;
            }
        }
    }

    public class SolarizeFilterModel : FilterModel
    {
        public SolarizeFilterModel()
        {
            Name = "Solarize";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new SolarizeFilter(0.5f));

                return components;
            }
        }
    }

    public class StampFilterModel : FilterModel
    {
        public StampFilterModel()
        {
            Name = "Stamp";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new StampFilter(3, 60));

                return components;
            }
        }
    }

    public class StepRotationLeftFilterModel : FilterModel
    {
        public StepRotationLeftFilterModel()
        {
            Name = "Rotate left";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new RotationFilter(270));

                return components;
            }
        }
    }

    public class StepRotationRightFilterModel : FilterModel
    {
        public StepRotationRightFilterModel()
        {
            Name = "Rotate right";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new RotationFilter(90));

                return components;
            }
        }
    }

    public class WarpTwisterFilterModel : FilterModel
    {
        public WarpTwisterFilterModel()
        {
            Name = "Warp Twister";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new WarpFilter(WarpEffect.Twister, 0.3f));

                return components;
            }
        }
    }

    public class WatercolorFilterModel : FilterModel
    {
        public WatercolorFilterModel()
        {
            Name = "Watercolor";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new WatercolorFilter(0.8f, 0.5f));

                return components;
            }
        }
    }

    public class VignettingFilterModel : FilterModel
    {
        public VignettingFilterModel()
        {
            Name = "Vignetting";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new VignettingFilter(0.4f, Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0x00)));

                return components;
            }
        }
    }

#endregion

    #region Enhancement filters

    public class AutoEnhanceFilterModel : FilterModel
    {
        public AutoEnhanceFilterModel()
        {
            Name = "Auto Enhance";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new AutoEnhanceFilter(true, true));

                return components;
            }
        }
    }

    public class AutoLevelsFilterModel : FilterModel
    {
        public AutoLevelsFilterModel()
        {
            Name = "Auto Levels";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new AutoLevelsFilter());

                return components;
            }
        }
    }

    public class ColorBoostFilterModel : FilterModel
    {
        public ColorBoostFilterModel()
        {
            Name = "Color Boost";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new ColorBoostFilter(3.0f));

                return components;
            }
        }
    }

    public class ExposureFilterModel : FilterModel
    {
        public ExposureFilterModel()
        {
            Name = "Exposure";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new ExposureFilter(ExposureMode.Natural, 0.9f));

                return components;
            }
        }
    }

    public class FoundationFilterModel : FilterModel
    {
        public FoundationFilterModel()
        {
            Name = "Foundation";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new FoundationFilter());

                return components;
            }
        }
    }

    public class LevelsFilterModel : FilterModel
    {
        public LevelsFilterModel()
        {
            Name = "Levels";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new LevelsFilter(0.7f, 0.2f, 0.4f));

                return components;
            }
        }
    }

    public class LocalBoostFilterModel : FilterModel
    {
        public LocalBoostFilterModel()
        {
            Name = "Local Boost";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new LocalBoostAutomaticFilter());

                return components;
            }
        }
    }

    public class TemperatureAndTintFilterModel : FilterModel
    {
        public TemperatureAndTintFilterModel()
        {
            Name = "Temperature & Tint";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new TemperatureAndTintFilter(0.8, -0.4));

                return components;
            }
        }
    }

    public class WhiteboardEnhancementFilterModel : FilterModel
    {
        public WhiteboardEnhancementFilterModel()
        {
            Name = "Whiteboard";
        }

        [XmlIgnore]
        public override Queue<IFilter> Components
        {
            get
            {
                Queue<IFilter> components = new Queue<IFilter>();

                components.Enqueue(new WhiteboardEnhancementFilter(WhiteboardEnhancementMode.Soft));

                return components;
            }
        }
    }

    #endregion
}
