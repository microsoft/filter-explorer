using Lumia.Imaging;
using Lumia.Imaging.Transforms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace ImageProcessingApp.Models
{
    /// <summary>
    /// Photo model is the central piece for keeping the state of the image being edited.
    /// It contains the main image editing session and the filters that have been applied
    /// to the image.
    /// </summary>
    [XmlRootAttribute("PhotoModel", Namespace = "ImageProcessingApp.Models")]
    public class PhotoModel : IDisposable
    {
        #region Members

        private IBuffer _buffer = null;
        private List<IFilter> _components = new List<IFilter>();

        #endregion

        #region Properties

        /// <summary>
        /// Get and set image data buffer.
        /// </summary>
        [XmlIgnore]
        public IBuffer Buffer
        {
            get
            {
                using (var source = new BufferImageSource(_buffer))
                using (var renderer = new JpegRenderer(source))
                {
                    IBuffer buffer = renderer.RenderAsync().AsTask().GetAwaiter().GetResult();

                    return buffer;
                }
            }

            set
            {
                if (_buffer != value)
                {
                    _buffer = value;
                }
            }
        }

        [XmlArray]
        // art filters
        [XmlArrayItem("AntiqueFilterModel", Type = typeof(AntiqueFilterModel))]
        [XmlArrayItem("BlurFilterModel", Type = typeof(BlurFilterModel))]
        [XmlArrayItem("BrightnessFilterModel", Type = typeof(BrightnessFilterModel))]
        [XmlArrayItem("CartoonFilterModel", Type = typeof(CartoonFilterModel))]
        [XmlArrayItem("ColorAdjustRedFilterModel", Type = typeof(ColorAdjustRedFilterModel))]
        [XmlArrayItem("ColorAdjustGreenFilterModel", Type = typeof(ColorAdjustGreenFilterModel))]
        [XmlArrayItem("ColorAdjustBlueFilterModel", Type = typeof(ColorAdjustBlueFilterModel))]
        [XmlArrayItem("ContrastFilterModel", Type = typeof(ContrastFilterModel))]
        [XmlArrayItem("DespeckleFilterModel", Type = typeof(DespeckleFilterModel))]
        [XmlArrayItem("EmbossFilterModel", Type = typeof(EmbossFilterModel))]
        [XmlArrayItem("FlipFilterModel", Type = typeof(FlipFilterModel))]
        [XmlArrayItem("GrayscaleFilterModel", Type = typeof(GrayscaleFilterModel))]
        [XmlArrayItem("HueSaturationFilterModel", Type = typeof(HueSaturationFilterModel))]
        [XmlArrayItem("LomoFilterModel", Type = typeof(LomoFilterModel))]
        [XmlArrayItem("MagicPenFilterModel", Type = typeof(MagicPenFilterModel))]
        [XmlArrayItem("MilkyFilterModel", Type = typeof(MilkyFilterModel))]
        [XmlArrayItem("MirrorFilterModel", Type = typeof(MirrorFilterModel))]
        [XmlArrayItem("MonocolorRedFilterModel", Type = typeof(MonocolorRedFilterModel))]
        [XmlArrayItem("MonocolorGreenFilterModel", Type = typeof(MonocolorGreenFilterModel))]
        [XmlArrayItem("MonocolorBlueFilterModel", Type = typeof(MonocolorBlueFilterModel))]
        [XmlArrayItem("MoonlightFilterModel", Type = typeof(MoonlightFilterModel))]
        [XmlArrayItem("NegativeFilterModel", Type = typeof(NegativeFilterModel))]
        [XmlArrayItem("OilyFilterModel", Type = typeof(OilyFilterModel))]
        [XmlArrayItem("PaintFilterModel", Type = typeof(PaintFilterModel))]
        [XmlArrayItem("PosterizeFilterModel", Type = typeof(PosterizeFilterModel))]
        [XmlArrayItem("SepiaFilterModel", Type = typeof(SepiaFilterModel))]
        [XmlArrayItem("SharpnessFilterModel", Type = typeof(SharpnessFilterModel))]
        [XmlArrayItem("SketchFilterModel", Type = typeof(SketchFilterModel))]
        [XmlArrayItem("SolarizeFilterModel", Type = typeof(SolarizeFilterModel))]
        [XmlArrayItem("StampFilterModel", Type = typeof(StampFilterModel))]
        [XmlArrayItem("StepRotationLeftFilterModel", Type = typeof(StepRotationLeftFilterModel))]
        [XmlArrayItem("StepRotationRightFilterModel", Type = typeof(StepRotationRightFilterModel))]
        [XmlArrayItem("WarpTwisterFilterModel", Type = typeof(WarpTwisterFilterModel))]
        [XmlArrayItem("WatercolorFilterModel", Type = typeof(WatercolorFilterModel))]
        // enhancement filters
        [XmlArrayItem("AutoEnhanceFilterModel", Type = typeof(AutoEnhanceFilterModel))]
        [XmlArrayItem("AutoLevelsFilterModel", Type = typeof(AutoLevelsFilterModel))]
        [XmlArrayItem("ColorBoostFilterModel", Type = typeof(ColorBoostFilterModel))]
        [XmlArrayItem("ExposureFilterModel", Type = typeof(ExposureFilterModel))]
        [XmlArrayItem("FoundationFilterModel", Type = typeof(FoundationFilterModel))]
        [XmlArrayItem("LevelsFilterModel", Type = typeof(LevelsFilterModel))]
        [XmlArrayItem("LocalBoostFilterModel", Type = typeof(LocalBoostFilterModel))]
        [XmlArrayItem("TemperatureAndTintFilterModel", Type = typeof(TemperatureAndTintFilterModel))]
        [XmlArrayItem("WhiteboardEnhancementFilterModel", Type = typeof(WhiteboardEnhancementFilterModel))]
        [XmlArrayItem("VignettingFilterModel", Type = typeof(VignettingFilterModel))]
        public List<FilterModel> AppliedFilters { get; set; }

        [XmlAttribute]
        public bool Dirty { get; set; }

        [XmlAttribute]
        public string Path { get; set; }

        [XmlAttribute]
        public bool Captured { get; set; }

        /// <summary>
        /// Check if there are filters applied that can be removed.
        /// </summary>
        [XmlIgnore]
        public bool CanUndoFilter
        {
            get
            {
                return AppliedFilters.Count > 0;
            }
        }

        #endregion

        public PhotoModel()
        {
            AppliedFilters = new List<FilterModel>();
            Dirty = false;
            Captured = false;
        }

        public void Dispose()
        {
            _buffer = null;
            _components = null;
        }

        /// <summary>
        /// Renders current image with applied filters to the given bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap to render to</param>
        public async Task RenderBitmapAsync(WriteableBitmap bitmap)
        {
            using (var source = new BufferImageSource(_buffer))
            using (var effect = new FilterEffect(source) { Filters = _components })
            using (var renderer = new WriteableBitmapRenderer(effect, bitmap))
            {
                await renderer.RenderAsync();

                bitmap.Invalidate();
            }
        }

        /// <summary>
        /// Renders current image with applied filters to a buffer and returns it.
        /// Meant to be used where the filtered image is for example going to be
        /// saved to a file.
        /// </summary>
        /// <returns>Buffer containing the filtered image data</returns>
        public async Task<IBuffer> RenderFullBufferAsync()
        {
            using (var source = new BufferImageSource(_buffer))
            using (var effect = new FilterEffect(source) { Filters = _components })
            using (var renderer = new JpegRenderer(effect))
            {
                return await renderer.RenderAsync();
            }
        }

        public async Task<Windows.Foundation.Size> GetImageSizeAsync()
        {
            using (var source = new BufferImageSource(_buffer))
            {
                return (await source.GetInfoAsync()).ImageSize;
            }
        }

        /// <summary>
        /// Renders a thumbnail of requested size from the center of the current image with
        /// filters applied.
        /// </summary>
        /// <param name="side">Side length of square thumbnail to render</param>
        /// <returns>Rendered thumbnail bitmap</returns>
        public async Task<Bitmap> RenderThumbnailBitmapAsync(int side)
        {
            Windows.Foundation.Size dimensions = await GetImageSizeAsync();

            int minSide = (int)Math.Min(dimensions.Width, dimensions.Height);

            Windows.Foundation.Rect rect = new Windows.Foundation.Rect()
            {
                Width = minSide,
                Height = minSide,
                X = (dimensions.Width - minSide) / 2,
                Y = (dimensions.Height - minSide) / 2,
            };

            _components.Add(new CropFilter(rect));

            Bitmap bitmap = new Bitmap(new Windows.Foundation.Size(side, side), ColorMode.Ayuv4444);

            using (BufferImageSource source = new BufferImageSource(_buffer))
            using (FilterEffect effect = new FilterEffect(source) { Filters = _components })
            using (BitmapRenderer renderer = new BitmapRenderer(effect, bitmap, OutputOption.Stretch))
            {
                await renderer.RenderAsync();
            }

            _components.RemoveAt(_components.Count - 1);

            return bitmap;
        }

        /// <summary>
        /// Apply filter to image. Notice that FilterModel may consist of many IFilter components.
        /// </summary>
        /// <param name="filter">Filter to apply</param>
        public void ApplyFilter(FilterModel filter)
        {
            AppliedFilters.Add(filter);

            foreach (IFilter f in filter.Components)
            {
                _components.Add(f);
            }
        }

        /// <summary>
        /// Undo last applied filter (if any).
        /// </summary>
        public void UndoFilter()
        {
            if (CanUndoFilter)
            {
                FilterModel filter = AppliedFilters[AppliedFilters.Count - 1];

                for (int i = 0; i < filter.Components.Count; i++)
                {
                    _components.RemoveAt(_components.Count - 1);
                }

                AppliedFilters.RemoveAt(AppliedFilters.Count - 1);
            }
        }

        public void UndoAllFilters()
        {
            if (CanUndoFilter)
            {
                AppliedFilters.Clear();

                _components.Clear();
            }
        }
    }
}
