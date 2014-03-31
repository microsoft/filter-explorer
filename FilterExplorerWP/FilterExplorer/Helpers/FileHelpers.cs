namespace ImageProcessingApp.Helpers
{
    public class FileHelpers
    {
        public static bool IsValidPicture(string filename)
        {
            string n = filename.ToLower();

            return n.EndsWith(".jpg") || n.EndsWith(".jpeg");
        }
    }
}
