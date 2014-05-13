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
            ArtisticFilters = new List<FilterModel>
            {
                new AntiqueFilterModel(),
                new BrightnessFilterModel(),
                new ColorAdjustRedFilterModel(),
                new ColorAdjustGreenFilterModel(),
                new ColorAdjustBlueFilterModel(),
                new ContrastFilterModel(),
                new FlipFilterModel(),
                new GrayscaleFilterModel(),
                new HueSaturationFilterModel(),
                new LomoFilterModel(),
                new MilkyFilterModel(),
                new MirrorFilterModel(),
                new MonocolorRedFilterModel(),
                new MonocolorGreenFilterModel(),
                new MonocolorBlueFilterModel(),
                new MoonlightFilterModel(),
                new NegativeFilterModel(),
                new PaintFilterModel(),
                new PosterizeFilterModel(),
                new SepiaFilterModel(),
                new SharpnessFilterModel(),
                new SketchFilterModel(),
                new StepRotationLeftFilterModel(),
                new StepRotationRightFilterModel(),
                new WarpTwisterFilterModel(),
                new WatercolorFilterModel(),
                new VignettingFilterModel()
            };

            /// todo Blend
            //ArtisticFilters.Add(new BlurFilterModel()); // depends on the source image size
            //ArtisticFilters.Add(new CartoonFilterModel()); // depends heavily of the rendering size
            /// todo Colorization
            /// todo ColorSwap
            /// todo Curves
            //ArtisticFilters.Add(new DespeckleFilterModel()); // depends on the source image size
            //ArtisticFilters.Add(new EmbossFilterModel()); // does not look good
            /// todo Frame
            /// todo FreeRotation
            /// todo GrayscaleNegative !
            /// todo ImageFusion
            //ArtisticFilters.Add(new MagicPenFilterModel()); // does not look good
            //ArtisticFilters.Add(new OilyFilterModel()); // slow
            //ArtisticFilters.Add(new SolarizeFilterModel()); // does not look good
            /// todo SplitTone
            /// todo Spotlight
            //ArtisticFilters.Add(new StampFilterModel()); // does not look good
        }

        private void LoadEnhancementFilters()
        {
            EnhancementFilters = new List<FilterModel>
            {
                new AutoEnhanceFilterModel(),
                new AutoLevelsFilterModel(),
                new ColorBoostFilterModel(),
                new ExposureFilterModel(),
                new FoundationFilterModel(),
                new LevelsFilterModel(),
                new LocalBoostFilterModel(),
                new TemperatureAndTintFilterModel(),
                new WhiteboardEnhancementFilterModel()
            };

            /// todo WhiteBalance
        }

        #endregion
    }
}
