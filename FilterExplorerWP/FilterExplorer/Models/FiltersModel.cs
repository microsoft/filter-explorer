using System;
using System.Collections.Generic;

namespace ImageProcessingApp.Models
{
    public class FiltersModel
    {
        #region Properties

        public List<FilterModel> ArtisticFilters { get; private set; }
        public List<FilterModel> EnhancementFilters { get; private set; }

        #endregion

        public FiltersModel()
        {
            LoadArtisticFilters();
            LoadEnhancementFilters();
        }

        public FilterModel RandomFilter()
        {
            int seed = unchecked((int)DateTime.Now.Ticks);

            Random random = new Random(seed);

            int index = random.Next(ArtisticFilters.Count + EnhancementFilters.Count - 1);

            return index < ArtisticFilters.Count ? ArtisticFilters[index] : EnhancementFilters[index - ArtisticFilters.Count];
        }

        #region Private methods

        private void LoadArtisticFilters()
        {
            ArtisticFilters = new List<FilterModel>();

            ArtisticFilters.Add(new AntiqueFilterModel());
            /// todo Blend
            //ArtisticFilters.Add(new BlurFilterModel()); // depends on the source image size
            ArtisticFilters.Add(new BrightnessFilterModel());
            //ArtisticFilters.Add(new CartoonFilterModel()); // depends heavily of the rendering size
            ArtisticFilters.Add(new ColorAdjustRedFilterModel());
            ArtisticFilters.Add(new ColorAdjustGreenFilterModel());
            ArtisticFilters.Add(new ColorAdjustBlueFilterModel());
            /// todo Colorization
            /// todo ColorSwap
            ArtisticFilters.Add(new ContrastFilterModel());
            /// todo Curves
            //ArtisticFilters.Add(new DespeckleFilterModel()); // depends on the source image size
            //ArtisticFilters.Add(new EmbossFilterModel()); // does not look good
            ArtisticFilters.Add(new FlipFilterModel());
            /// todo Frame
            /// todo FreeRotation
            ArtisticFilters.Add(new GrayscaleFilterModel());
            /// todo GrayscaleNegative !
            ArtisticFilters.Add(new HueSaturationFilterModel());
            /// todo ImageFusion
            ArtisticFilters.Add(new LomoFilterModel());
            //ArtisticFilters.Add(new MagicPenFilterModel()); // does not look good
            ArtisticFilters.Add(new MilkyFilterModel());
            ArtisticFilters.Add(new MirrorFilterModel());
            ArtisticFilters.Add(new MonocolorRedFilterModel());
            ArtisticFilters.Add(new MonocolorGreenFilterModel());
            ArtisticFilters.Add(new MonocolorBlueFilterModel());
            ArtisticFilters.Add(new MoonlightFilterModel());
            ArtisticFilters.Add(new NegativeFilterModel());
            //ArtisticFilters.Add(new OilyFilterModel()); // slow
            ArtisticFilters.Add(new PaintFilterModel());
            ArtisticFilters.Add(new PosterizeFilterModel());
            ArtisticFilters.Add(new SepiaFilterModel());
            ArtisticFilters.Add(new SharpnessFilterModel());
            ArtisticFilters.Add(new SketchFilterModel());
            //ArtisticFilters.Add(new SolarizeFilterModel()); // does not look good
            /// todo SplitTone
            /// todo Spotlight
            //ArtisticFilters.Add(new StampFilterModel()); // does not look good
            ArtisticFilters.Add(new StepRotationLeftFilterModel());
            ArtisticFilters.Add(new StepRotationRightFilterModel());
            ArtisticFilters.Add(new WarpTwisterFilterModel());
            ArtisticFilters.Add(new WatercolorFilterModel());
            ArtisticFilters.Add(new VignettingFilterModel());
        }

        private void LoadEnhancementFilters()
        {
            EnhancementFilters = new List<FilterModel>();

            EnhancementFilters.Add(new AutoEnhanceFilterModel());
            EnhancementFilters.Add(new AutoLevelsFilterModel());
            EnhancementFilters.Add(new ColorBoostFilterModel());
            EnhancementFilters.Add(new ExposureFilterModel());
            EnhancementFilters.Add(new FoundationFilterModel());
            EnhancementFilters.Add(new LevelsFilterModel());
            EnhancementFilters.Add(new LocalBoostFilterModel());
            EnhancementFilters.Add(new TemperatureAndTintFilterModel());
            /// todo WhiteBalance
            EnhancementFilters.Add(new WhiteboardEnhancementFilterModel());
        }

        #endregion
    }
}
